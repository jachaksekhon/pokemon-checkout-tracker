using PKCTracker.Web.Interfaces;
using PKCTracker.Web.Data;
using PKCTracker.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime;
using PKCTracker.Web.Models.Entities;

public class CheckoutService : ICheckoutService
{
    private readonly TrackerDbContext _db;

    public CheckoutService(TrackerDbContext context)
    {
        _db = context;
    }

    public async Task ProcessCheckoutAsync(CheckoutResult checkout)
    {
        var currentDrop = await _db.Drops
            .FirstOrDefaultAsync(d => d.GuildId == checkout.GuildId && d.IsActive);
        
        if (currentDrop == null)
        {
            return;
        }

        // Found drop, now insert profile but only if it doesn't already exist
        var existingProfile = await _db.ProfileResults
            .FirstOrDefaultAsync(p => p.DropId == currentDrop.Id && p.Profile == checkout.Profile);

        // if profile exists and succes goes from failure -> success, update the existing record
        if (existingProfile != null)
        {
            if (!existingProfile.Success && checkout.Success)
            {
                existingProfile.Success = true;
                existingProfile.OrderNumber = checkout.OrderNumber;
                existingProfile.OrderLink = checkout.OrderLink;
                existingProfile.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                return;
            }
        }
        else
        {
            var now = DateTime.UtcNow;
            var profileResult = new ProfileResult
            {
                DropId = currentDrop.Id,
                Profile = checkout.Profile,
                Success = checkout.Success,
                OrderNumber = null,
                OrderLink = null,
                CreatedAt = now,
                UpdatedAt = now,
                Drop = currentDrop
            };

            if (checkout.Success)
            {
                profileResult.OrderNumber = checkout.OrderNumber;
                profileResult.OrderLink = checkout.OrderLink;
            }
            
            _db.ProfileResults.Add(profileResult);
        }

        await _db.SaveChangesAsync();
    }

}