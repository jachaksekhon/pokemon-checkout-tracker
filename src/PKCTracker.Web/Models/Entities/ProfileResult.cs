namespace PKCTracker.Web.Models.Entities;

public class ProfileResult
{
    public int Id { get; set; }
    public int DropId { get; set; }
    public required string Profile { get; set; }
    public bool Success { get; set; }
    public string? OrderNumber { get; set; }
    public string? OrderLink { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required Drop Drop { get; set; }
}
