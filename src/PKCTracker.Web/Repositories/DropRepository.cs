
using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Data;
using PKCTracker.Web.Interfaces.Repositories;
using PKCTracker.Web.Models.Entities;

namespace PKCTracker.Web.Repositories;

public class DropRepository : IDropRepository
{
    private readonly TrackerDbContext _db;

    public DropRepository(TrackerDbContext db)
    {
        _db = db;
    }
    public Task<List<Drop>> GetDropsByGuildIdAsync(ulong guildId)
    {
        return _db.Drops
            .Where(d => d.GuildId == guildId)
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }
}