// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Creates, reads and answers support tickets and enforces that members only reach their own tickets while admins reach all.
/// </summary>
/// <param name="userId">Id of the signed-in member taken from the authentication claim</param>
/// <param name="isAdmin">True when the signed-in member holds the Admin role</param>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class TicketService : ITicketService
{
    private readonly MarketplaceDbContext _db;

    public TicketService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<Ticket> CreateAsync(int userId, string subject, string body)
    {
        var cleanSubject = ValidateText(subject, TicketConstants.MaxSubjectLength, "Subject");
        var cleanBody = ValidateText(body, TicketConstants.MaxBodyLength, "Message");

        var openCount = await _db.Tickets.CountAsync(t => t.UserId == userId && t.Status != TicketStatus.Closed);
        if (openCount >= TicketConstants.MaxOpenTicketsPerUser)
        {
            throw new InvalidOperationException(TicketConstants.TooManyOpenTicketsMessage);
        }

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            UserId = userId,
            Subject = cleanSubject,
            Status = TicketStatus.Open,
            CreatedAt = now,
            UpdatedAt = now,
            Messages =
            [
                new TicketMessage { AuthorId = userId, Body = cleanBody, IsStaffReply = false, CreatedAt = now }
            ]
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        return ticket;
    }

    public async Task<List<Ticket>> GetUserTicketsAsync(int userId)
    {
        return await _db.Tickets
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.UpdatedAt)
            .Take(TicketConstants.MaxTicketsListed)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetAllTicketsAsync(TicketStatus? status)
    {
        var query = _db.Tickets.AsNoTracking().Include(t => t.User).AsQueryable();
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        return await query
            .OrderByDescending(t => t.UpdatedAt)
            .Take(TicketConstants.MaxTicketsListed)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketAsync(int ticketId, int userId, bool isAdmin)
    {
        return await _db.Tickets
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.Messages.OrderBy(m => m.CreatedAt))
                .ThenInclude(m => m.Author)
            .FirstOrDefaultAsync(t => t.Id == ticketId && (isAdmin || t.UserId == userId));
    }

    public async Task<bool> ReplyAsync(int ticketId, int userId, bool isAdmin, string body)
    {
        var cleanBody = ValidateText(body, TicketConstants.MaxBodyLength, "Message");

        var exists = await _db.Tickets
            .AsNoTracking()
            .AnyAsync(t => t.Id == ticketId && (isAdmin || t.UserId == userId));
        if (!exists)
        {
            return false;
        }

        var messageCount = await _db.TicketMessages.CountAsync(m => m.TicketId == ticketId);
        if (messageCount >= TicketConstants.MaxMessagesPerTicket)
        {
            throw new InvalidOperationException(TicketConstants.MessageLimitReachedMessage);
        }

        var now = DateTime.UtcNow;
        _db.TicketMessages.Add(new TicketMessage
        {
            TicketId = ticketId,
            AuthorId = userId,
            Body = cleanBody,
            IsStaffReply = isAdmin,
            CreatedAt = now
        });

        var newStatus = isAdmin ? TicketStatus.Answered : TicketStatus.Open;
        await _db.Tickets
            .Where(t => t.Id == ticketId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Status, newStatus)
                .SetProperty(t => t.UpdatedAt, now));
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseAsync(int ticketId, int userId, bool isAdmin)
    {
        var now = DateTime.UtcNow;
        var affected = await _db.Tickets
            .Where(t => t.Id == ticketId && (isAdmin || t.UserId == userId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Status, TicketStatus.Closed)
                .SetProperty(t => t.UpdatedAt, now));
        return affected > 0;
    }

    public async Task<int> CountOpenAsync()
    {
        return await _db.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
    }

    private static string ValidateText(string value, int maxLength, string fieldName)
    {
        var clean = (value ?? string.Empty).Trim();
        if (clean.Length == 0)
        {
            throw new InvalidOperationException($"{fieldName} is required");
        }

        if (clean.Length > maxLength)
        {
            throw new InvalidOperationException($"{fieldName} must not exceed {maxLength} characters");
        }

        return clean;
    }
}
