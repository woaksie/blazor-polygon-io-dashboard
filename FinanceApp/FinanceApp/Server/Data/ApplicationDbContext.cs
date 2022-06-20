﻿using Duende.IdentityServer.EntityFramework.Options;
using FinanceApp.Server.Models;
using FinanceApp.Server.Models.DailyOpenClose;
using FinanceApp.Server.Models.Logo;
using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Server.Models.Tickers;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceApp.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    public DbSet<TickerResults> Results => Set<TickerResults>();
    public DbSet<TickerListItem> TickerListItems => Set<TickerListItem>();
    public DbSet<Logo> Logos => Set<Logo>();
    public DbSet<DailyOpenClose> DailyOpenCloses => Set<DailyOpenClose>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Address>(e => { e.HasKey(a => a.Address1); });

        builder.Entity<Branding>(e => { e.HasKey(b => b.LogoUrl); });

        builder.Entity<TickerResults>(e =>
        {
            e.HasKey(tr => tr.Ticker);
            e.Property(tr => tr.Ticker).HasMaxLength(4);
            e.ToTable("TickerResults");

            e.HasMany(td => td.ApplicationUsers)
                .WithMany(td => td.TickerWatchlist)
                .UsingEntity(j => j.ToTable("ApplicationUserTickerResults"));

            e.HasOne(tr => tr.Logo)
                .WithOne(l => l.TickerResults)
                .HasForeignKey<Logo>(l => l.Ticker);

            e.HasMany(tr => tr.DailyOpenCloses)
                .WithOne(doc => doc.TickerResults)
                .HasForeignKey(doc => doc.Ticker);
        });

        builder.Entity<TickerListItem>(e =>
        {
            e.HasKey(tli => tli.Ticker);
            e.ToTable("TickerListItem");
        });

        builder.Entity<Logo>(e =>
        {
            e.HasKey(l => l.Ticker);
            e.ToTable("Logo");
        });

        builder.Entity<DailyOpenClose>(e =>
        {
            e.HasKey(doc => new { doc.Ticker, doc.From });
            e.Property(doc => doc.Ticker).HasMaxLength(4);
            e.ToTable("DailyOpenClose");
        });
    }
}