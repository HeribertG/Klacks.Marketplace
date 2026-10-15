// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface ITicketService
{
    Task<Ticket> CreateAsync(int userId, string subject, string body);
    Task<List<Ticket>> GetUserTicketsAsync(int userId);
    Task<List<Ticket>> GetAllTicketsAsync(TicketStatus? status);
    Task<Ticket?> GetTicketAsync(int ticketId, int userId, bool isAdmin);
    Task<bool> ReplyAsync(int ticketId, int userId, bool isAdmin, string body);
    Task<bool> CloseAsync(int ticketId, int userId, bool isAdmin);
    Task<int> CountOpenAsync();
}
