using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Data;
using PKCTracker.Web.Models;
using PKCTracker.Web.Models.Entities;
using PKCTracker.Web.Services;

namespace PKCTracker.Tests;

public class CheckoutServiceTests
{
    private static TrackerDbContext CreateTestDbContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<TrackerDbContext>()
            .UseSqlite(connection)
            .Options;

        return new TrackerDbContext(options);
    }

    private static async Task<(TrackerDbContext Db, Drop drop, SqliteConnection connection)> CreateTestDbContextWithDropAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var db = CreateTestDbContext(connection);

        await db.Database.EnsureCreatedAsync();

        var drop = new Drop
        {
            GuildId = 12345,
            IsActive = true,
            Name = "Test Drop",
            CreatedAt = DateTime.UtcNow
        };
        db.Drops.Add(drop);
        await db.SaveChangesAsync();

        return (db, drop, connection);
    }


    [Fact]
    public async Task ProcessCheckoutAsync_NewSuccessfulCheckout_CreatesSuccessfulProfile()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;
        
        var checkout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            OrderNumber = "p14191929",
            Success = true,
            OrderLink = "https://www.pokemoncenter.com/order/p14191929"
        };

        var service = new CheckoutService(db);

        await service.ProcessCheckoutAsync(checkout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");
        
        Assert.NotNull(profileResult);
        Assert.True(profileResult.Success);
        Assert.NotNull(profileResult.OrderNumber);
        Assert.Equal("p14191929", profileResult.OrderNumber);
        Assert.NotNull(profileResult.OrderLink);
        Assert.Equal("https://www.pokemoncenter.com/order/p14191929", profileResult.OrderLink);

    }

    [Fact]
    public async Task ProcessCheckoutAsync_NewSuccessfulCheckout_UpdatesPreviouslyFailedCheckout()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;

        var failedCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = false,
            OrderNumber = null,
            OrderLink = null
        };

        var service = new CheckoutService(db);
        await service.ProcessCheckoutAsync(failedCheckout);

        var successfulCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = true,
            OrderNumber = "p14191929",
            OrderLink = "https://www.pokemoncenter.com/order/p14191929"
        };

        await service.ProcessCheckoutAsync(successfulCheckout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        var count = await verificationDb.ProfileResults
            .CountAsync(p =>p.DropId == drop.Id && p.Profile == "TestProfile");

        Assert.Equal(1, count);
        Assert.NotNull(profileResult);
        Assert.True(profileResult.Success);
        Assert.Equal("p14191929", profileResult.OrderNumber);
        Assert.Equal("https://www.pokemoncenter.com/order/p14191929", profileResult.OrderLink);
    }

    [Fact]
    public async Task ProcessCheckoutAsync_NewSuccessfulCheckout_DoesNotUpdateExistingSuccessfulCheckout()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;

        var successfulCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = true,
            OrderNumber = "p14191929",
            OrderLink = "https://www.pokemoncenter.com/order/p14191929"
        };

        var service = new CheckoutService(db);
        await service.ProcessCheckoutAsync(successfulCheckout);

        var anotherSuccessfulCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = true,
            OrderNumber = "p14191930",
            OrderLink = "https://www.pokemoncenter.com/order/p14191930"
        };

        await service.ProcessCheckoutAsync(anotherSuccessfulCheckout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        Assert.NotNull(profileResult);
        Assert.True(profileResult.Success);
        Assert.Equal("p14191929", profileResult.OrderNumber);
        Assert.Equal("https://www.pokemoncenter.com/order/p14191929", profileResult.OrderLink);
    }

    [Fact]
    public async Task ProcessCheckoutAsync_NewFailedCheckout_CreatesNewFailedProfile()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;

        var failedCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = false,
            OrderNumber = null,
            OrderLink = null
        };

        var service = new CheckoutService(db);
        await service.ProcessCheckoutAsync(failedCheckout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        Assert.NotNull(profileResult);
        Assert.False(profileResult.Success);
        Assert.Null(profileResult.OrderNumber);
        Assert.Null(profileResult.OrderLink);
    }

    [Fact]
    public async Task ProcessCheckoutAsync_NewFailedCheckout_DoesNotCreateDuplicateExistingProfile()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;

        var failedCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = false,
            OrderNumber = null,
            OrderLink = null
        };

        var service = new CheckoutService(db);
        await service.ProcessCheckoutAsync(failedCheckout);

        var duplicateFailedCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = false,
            OrderNumber = null,
            OrderLink = null
        };

        var originalProfile = await db.ProfileResults
                .SingleAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        var originalId = originalProfile.Id;
        var originalUpdatedAt = originalProfile.UpdatedAt;

        await service.ProcessCheckoutAsync(duplicateFailedCheckout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p =>
                p.DropId == drop.Id &&
                p.Profile == "TestProfile");

        Assert.NotNull(profileResult);
        Assert.Equal(originalId, profileResult.Id);
        Assert.Equal(originalUpdatedAt, profileResult.UpdatedAt);
        Assert.False(profileResult.Success);
        Assert.Null(profileResult.OrderNumber);
        Assert.Null(profileResult.OrderLink);

    }

    [Fact]
    public async Task ProcessCheckoutAsync_NewFailedCheckout_DoesNotUpdateSuccessfulProfile()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        using var _connection = connection;
        using var _db = db;

        var successfulCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = true,
            OrderNumber = "12345",
            OrderLink = "https://example.com/order/12345"
        };

        var service = new CheckoutService(db);
        await service.ProcessCheckoutAsync(successfulCheckout);

        var newfailedCheckout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = false,
            OrderNumber = null,
            OrderLink = null
        };

        var originalProfile = await db.ProfileResults
                .SingleAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        var originalId = originalProfile.Id;
        var originalUpdatedAt = originalProfile.UpdatedAt;

        await service.ProcessCheckoutAsync(newfailedCheckout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p =>
                p.DropId == drop.Id &&
                p.Profile == "TestProfile");

        Assert.NotNull(profileResult);
        Assert.Equal(originalId, profileResult.Id);
        Assert.Equal(originalUpdatedAt, profileResult.UpdatedAt);
        Assert.True(profileResult.Success);
        Assert.NotNull(profileResult.OrderNumber);
        Assert.NotNull(profileResult.OrderLink);

    }

    [Fact]

    public async Task ProcessCheckoutAsync_NoActiveDrop_DoesNotCreateProfile()
    {
        var (db, drop, connection) = await CreateTestDbContextWithDropAsync();

        // Deactivate the drop
        drop.IsActive = false;
        await db.SaveChangesAsync();

        using var _connection = connection;
        using var _db = db;

        var checkout = new CheckoutResult
        {
            GuildId = 12345,
            Profile = "TestProfile",
            Success = true,
            OrderNumber = "p14191929",
            OrderLink = "https://www.pokemoncenter.com/order/p14191929"
        };

        var service = new CheckoutService(db);

        await service.ProcessCheckoutAsync(checkout);

        using var verificationDb = CreateTestDbContext(connection);

        var profileResult = await verificationDb.ProfileResults
            .SingleOrDefaultAsync(p => p.DropId == drop.Id && p.Profile == "TestProfile");

        Assert.Null(profileResult);
    }
}

    