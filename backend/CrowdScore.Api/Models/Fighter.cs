namespace CrowdScore.Api.Models;

public sealed class Fighter
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Nickname { get; set; }
    public string? Country { get; set; }

    public ICollection<Fight> FightsAsFighterA { get; } = [];
    public ICollection<Fight> FightsAsFighterB { get; } = [];
}
