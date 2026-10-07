namespace PKCTracker.Web.Models.Entities;

public class Drop
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProfileResult> Profiles { get; set; } = [];
}