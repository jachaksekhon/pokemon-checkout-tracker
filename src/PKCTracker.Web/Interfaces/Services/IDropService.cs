using PKCTracker.Web.Models.Entities;

namespace PKCTracker.Web.Interfaces.Services;

public interface IDropService
{
    Task<List<Drop>> GetDropsByGuildAsync(ulong guildId);
}