using CrowdScore.Api.Models;

namespace CrowdScore.Api.DTOs;

public sealed record FightResponse(
    int Id,
    int EventId,
    int CardPosition,
    FighterResponse FighterA,
    FighterResponse FighterB,
    int ScheduledRounds,
    FightStatus Status);
