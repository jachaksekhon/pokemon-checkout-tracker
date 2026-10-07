namespace PKCTracker.Web.Models;
public class CheckoutResult
{
    public ulong GuildId { get; set; }
    public string Profile { get; set; }
    public string? OrderNumber { get; set; }
    public bool Success { get; set; }
    public string? OrderLink { get; set; }
}