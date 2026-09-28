using CrowdScore.Api.Data;
using CrowdScore.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrowdScore.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(CrowdScoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<EventSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventSummaryResponse>>> GetEvents(
        CancellationToken cancellationToken)
    {
        var events = await dbContext.Events
            .AsNoTracking()
            .OrderBy(eventEntity => eventEntity.EventDate)
            .Select(eventEntity => new EventSummaryResponse(
                eventEntity.Id,
                eventEntity.Name,
                eventEntity.EventDate,
                eventEntity.Location))
            .ToListAsync(cancellationToken);

        return Ok(events);
    }

    [HttpGet("{eventId:int}")]
    [ProducesResponseType<EventDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDetailsResponse>> GetEvent(
        int eventId,
        CancellationToken cancellationToken)
    {
        var eventResponse = await dbContext.Events
            .AsNoTracking()
            .Where(eventEntity => eventEntity.Id == eventId)
            .Select(eventEntity => new EventDetailsResponse(
                eventEntity.Id,
                eventEntity.Name,
                eventEntity.EventDate,
                eventEntity.Location,
                eventEntity.Fights
                    .OrderBy(fight => fight.CardPosition)
                    .Select(fight => new FightResponse(
                        fight.Id,
                        fight.EventId,
                        fight.CardPosition,
                        new FighterResponse(
                            fight.FighterA.Id,
                            fight.FighterA.FirstName,
                            fight.FighterA.LastName,
                            fight.FighterA.Nickname,
                            fight.FighterA.Country),
                        new FighterResponse(
                            fight.FighterB.Id,
                            fight.FighterB.FirstName,
                            fight.FighterB.LastName,
                            fight.FighterB.Nickname,
                            fight.FighterB.Country),
                        fight.ScheduledRounds,
                        fight.Status))
                    .ToList()))
            .SingleOrDefaultAsync(cancellationToken);

        return eventResponse is null ? NotFound() : Ok(eventResponse);
    }
}
