namespace CrowdScore.Api.DTOs;

public sealed record EventDetailsResponse(
    int Id,
    string Name,
    DateTimeOffset EventDate,
    string? Location,
    IReadOnlyList<FightResponse> Fights);
