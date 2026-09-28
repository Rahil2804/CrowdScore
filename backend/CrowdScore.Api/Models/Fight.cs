namespace CrowdScore.Api.Models;

public sealed class Fight
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int FighterAId { get; set; }
    public int FighterBId { get; set; }
    public int CardPosition { get; set; }
    public int ScheduledRounds { get; set; }
    public FightStatus Status { get; set; }

    public Event Event { get; set; } = null!;
    public Fighter FighterA { get; set; } = null!;
    public Fighter FighterB { get; set; } = null!;
}
