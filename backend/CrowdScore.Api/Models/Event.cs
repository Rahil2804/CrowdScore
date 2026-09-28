namespace CrowdScore.Api.Models;

public sealed class Event
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public string? Location { get; set; }

    public ICollection<Fight> Fights { get; } = [];
}
