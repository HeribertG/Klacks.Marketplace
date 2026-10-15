// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Constants;

public static class TicketConstants
{
    public const int MaxSubjectLength = 150;
    public const int MaxBodyLength = 4000;
    public const int MaxOpenTicketsPerUser = 5;
    public const int MaxMessagesPerTicket = 200;
    public const int MaxTicketsListed = 200;
    public const string TooManyOpenTicketsMessage = "Too many open tickets";
    public const string MessageLimitReachedMessage = "Ticket has reached the message limit";
    public const string TicketsRoute = "tickets";
    public const string NewTicketRoute = "tickets/new";
}
