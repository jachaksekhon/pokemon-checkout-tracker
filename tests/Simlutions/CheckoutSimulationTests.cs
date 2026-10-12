using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Data;
using PKCTracker.Web.Models;
using PKCTracker.Web.Models.Entities;
using PKCTracker.Web.Services;
using PKCTracker.Tests.Constants;

namespace PKCTracker.Tests.Simulations;

public class CheckoutSimulationTests
{
    private Drop CreateActiveDrop(ulong guildId)
    {
        return new Drop
        {
            GuildId = guildId,
            Name = CheckoutSimulationConstants.DropName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private CheckoutResult CreateCheckoutResult(ulong guildId, string profile, bool success, string? orderNumber = null, string? orderLink = null)
    {
        return new CheckoutResult
        {
            GuildId = guildId,
            Profile = profile,
            Success = success,
            OrderNumber = orderNumber,
            OrderLink = orderLink
        };
    }

    [Fact]
    public async Task MultipleGuilds_ProcessCheckouts_KeepResultsSeparate()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TrackerDbContext>()
            .UseSqlite(connection)
            .Options;

        using var db = new TrackerDbContext(options);

        await db.Database.EnsureCreatedAsync();

        var guild1Drop = CreateActiveDrop(1);
        var guild2Drop = CreateActiveDrop(2);

        db.Drops.AddRange(guild1Drop, guild2Drop);
        await db.SaveChangesAsync();

        var savedDrops = await db.Drops
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(2, savedDrops.Count);

        var checkoutList = new List<CheckoutResult>
        {
            CreateCheckoutResult(1, "ProfileA", true, "Order123", "http://orderlink.com/123"),
            CreateCheckoutResult(1, "ProfileB", false),
            CreateCheckoutResult(2, "ProfileB", true, "Order456", "http://orderlink.com/456"),
            CreateCheckoutResult(2, "ProfileD", false)
        };

        var checkoutService = new CheckoutService(db);

        foreach (var checkout in checkoutList)
        {
            await checkoutService.ProcessCheckoutAsync(checkout);
        }

        var results = await db.ProfileResults
            .AsNoTracking()
            .Include(p => p.Drop)
            .ToListAsync();

        var guild1Results = results
            .Where(r => r.Drop.GuildId == 1)
            .ToList();
        
        var guild2Results = results
            .Where(r => r.Drop.GuildId == 2)
            .ToList();

        Assert.Equal(2, guild1Results.Count);
        Assert.Equal(2, guild2Results.Count);

        var guild1ProfileA = guild1Results.Single(r => r.Profile == "ProfileA");
        var guild1ProfileB = guild1Results.Single(r => r.Profile == "ProfileB");

        var guild2ProfileB = guild2Results.Single(r => r.Profile == "ProfileB");
        var guild2ProfileD = guild2Results.Single(r => r.Profile == "ProfileD");

        // Guild 1
        Assert.True(guild1ProfileA.Success);
        Assert.Equal("Order123", guild1ProfileA.OrderNumber);
        Assert.Equal("http://orderlink.com/123", guild1ProfileA.OrderLink);

        Assert.False(guild1ProfileB.Success);
        Assert.Null(guild1ProfileB.OrderNumber);
        Assert.Null(guild1ProfileB.OrderLink);

        // Guild 2
        Assert.True(guild2ProfileB.Success);
        Assert.Equal("Order456", guild2ProfileB.OrderNumber);
        Assert.Equal("http://orderlink.com/456", guild2ProfileB.OrderLink);

        Assert.False(guild2ProfileD.Success);
        Assert.Null(guild2ProfileD.OrderNumber);
        Assert.Null(guild2ProfileD.OrderLink);

    }
}