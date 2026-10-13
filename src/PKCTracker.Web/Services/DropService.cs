using PKCTracker.Web.Interfaces.Repositories;
using PKCTracker.Web.Interfaces.Services;
using PKCTracker.Web.Models.Entities;

public class DropService : IDropService
{
    private readonly IDropRepository _dropRepository;

    public DropService(IDropRepository dropRepository)
    {
        _dropRepository = dropRepository;
    }

    public Task<List<Drop>> GetDropsByGuildAsync(ulong guildId)
    {
        return _dropRepository.GetDropsByGuildIdAsync(guildId);
    }
}