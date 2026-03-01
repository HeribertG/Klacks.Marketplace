// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Data;

public class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<LanguagePackage> Packages => Set<LanguagePackage>();
    public DbSet<PackageVersion> PackageVersions => Set<PackageVersion>();
    public DbSet<DownloadLog> DownloadLogs => Set<DownloadLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<LanguagePackage>(entity =>
        {
            entity.HasIndex(e => e.Code);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.HasOne(e => e.Author)
                  .WithMany()
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PackageVersion>(entity =>
        {
            entity.HasOne(e => e.Package)
                  .WithMany(p => p.Versions)
                  .HasForeignKey(e => e.PackageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DownloadLog>(entity =>
        {
            entity.HasOne(e => e.Package)
                  .WithMany(p => p.DownloadLogs)
                  .HasForeignKey(e => e.PackageId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
        });
    }
}
