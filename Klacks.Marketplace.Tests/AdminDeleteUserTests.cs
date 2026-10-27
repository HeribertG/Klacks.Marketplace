// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;
using Klacks.Marketplace.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Erasing a customer account: tickets and messages are deleted, the account is anonymized and can no longer sign in,
/// the row stays for published packages, and admins are protected.
/// </summary>
[TestFixture]
public class AdminDeleteUserTests
{
    [Test]
    public async Task DeleteUserAsync_removes_tickets_and_messages_and_anonymizes_the_account()
    {
        using var database = await TestDatabase.CreateAsync();
        var db = database.Context;
        var admin = new User { Email = "admin@test.invalid", PasswordHash = "x", DisplayName = "Admin", IsAdmin = true };
        var customer = new User { Email = "customer@test.invalid", PasswordHash = "salt.hash", DisplayName = "Customer" };
        db.Users.AddRange(admin, customer);
        await db.SaveChangesAsync();
        var ticket = new Ticket { UserId = customer.Id, Subject = "Help" };
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();
        db.TicketMessages.AddRange(
            new TicketMessage { TicketId = ticket.Id, AuthorId = customer.Id, Body = "question" },
            new TicketMessage { TicketId = ticket.Id, AuthorId = admin.Id, Body = "answer", IsStaffReply = true });
        db.Packages.Add(new LanguagePackage { Code = "xx", Name = "Test", AuthorId = customer.Id });
        await db.SaveChangesAsync();

        await new AdminService(db).DeleteUserAsync(customer.Id);

        var stored = await db.Users.AsNoTracking().SingleAsync(u => u.Id == customer.Id);
        stored.Email.ShouldBe(string.Format(AppConstants.DeletedUserEmailTemplate, customer.Id));
        stored.DisplayName.ShouldBe(AppConstants.DeletedUserDisplayName);
        stored.PasswordHash.ShouldBeEmpty();
        (await db.Tickets.CountAsync()).ShouldBe(0);
        (await db.TicketMessages.CountAsync()).ShouldBe(0);
        (await db.Packages.CountAsync(p => p.AuthorId == customer.Id)).ShouldBe(1);
        (await new AuthService(db).LoginAsync(stored.Email, "anything")).ShouldBeNull();
    }

    [Test]
    public async Task DeleteUserAsync_refuses_admins()
    {
        using var database = await TestDatabase.CreateAsync();
        var db = database.Context;
        var admin = new User { Email = "admin@test.invalid", PasswordHash = "x", DisplayName = "Admin", IsAdmin = true };
        db.Users.Add(admin);
        await db.SaveChangesAsync();

        await Should.ThrowAsync<InvalidOperationException>(() => new AdminService(db).DeleteUserAsync(admin.Id));

        (await db.Users.AsNoTracking().SingleAsync(u => u.Id == admin.Id)).Email.ShouldBe("admin@test.invalid");
    }

    [Test]
    public async Task DeleteUserAsync_refuses_unknown_users()
    {
        using var database = await TestDatabase.CreateAsync();

        await Should.ThrowAsync<InvalidOperationException>(() => new AdminService(database.Context).DeleteUserAsync(999));
    }
}
