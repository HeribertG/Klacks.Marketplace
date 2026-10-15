// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Single plain-text message inside a support ticket thread.
/// </summary>
/// <param name="AuthorId">User who wrote the message</param>
/// <param name="IsStaffReply">True when an admin wrote the message</param>
namespace Klacks.Marketplace.Models;

public class TicketMessage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Body { get; set; } = string.Empty;
    public bool IsStaffReply { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
