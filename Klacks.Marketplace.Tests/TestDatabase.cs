// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// A migrated, private in-memory SQLite database for one test. The connection stays open for the lifetime of the
/// object because an in-memory SQLite database disappears with its connection.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    private TestDatabase(SqliteConnection connection, MarketplaceDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public MarketplaceDbContext Context { get; }

    public static async Task<TestDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MarketplaceDbContext>().UseSqlite(connection).Options;
        var context = new MarketplaceDbContext(options);
        await context.Database.MigrateAsync();

        return new TestDatabase(connection, context);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
