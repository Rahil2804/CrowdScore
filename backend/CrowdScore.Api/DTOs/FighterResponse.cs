namespace CrowdScore.Api.DTOs;

public sealed record FighterResponse(
    int Id,
    string FirstName,
    string LastName,
    string? Nickname,
    string? Country);
