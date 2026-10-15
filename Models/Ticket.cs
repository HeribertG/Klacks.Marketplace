// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Support ticket opened by a registered marketplace member.
/// </summary>
/// <param name="UserId">Member who opened the ticket and may read and answer it</param>
/// <param name="Status">Open after a member message, Answered after an admin reply, Closed when finished</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Subject { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<TicketMessage> Messages { get; set; } = [];
}
