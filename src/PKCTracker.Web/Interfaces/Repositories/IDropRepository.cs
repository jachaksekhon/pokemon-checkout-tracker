
using PKCTracker.Web.Models.Entities;

namespace PKCTracker.Web.Interfaces.Repositories;
public interface IDropRepository
{
    Task<List<Drop>> GetDropsByGuildIdAsync(ulong guildId);
}