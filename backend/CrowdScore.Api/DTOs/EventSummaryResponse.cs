namespace CrowdScore.Api.DTOs;

public sealed record EventSummaryResponse(
    int Id,
    string Name,
    DateTimeOffset EventDate,
    string? Location);
