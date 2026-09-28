using CrowdScore.Api.Data;
using CrowdScore.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrowdScore.Api.Controllers;

[ApiController]
[Route("api/fights")]
public sealed class FightsController(CrowdScoreDbContext dbContext) : ControllerBase
{
    [HttpGet("{fightId:int}")]
    [ProducesResponseType<FightResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FightResponse>> GetFight(
        int fightId,
        CancellationToken cancellationToken)
    {
        var fight = await dbContext.Fights
            .AsNoTracking()
            .Where(fightEntity => fightEntity.Id == fightId)
            .Select(fightEntity => new FightResponse(
                fightEntity.Id,
                fightEntity.EventId,
                fightEntity.CardPosition,
                new FighterResponse(
                    fightEntity.FighterA.Id,
                    fightEntity.FighterA.FirstName,
                    fightEntity.FighterA.LastName,
                    fightEntity.FighterA.Nickname,
                    fightEntity.FighterA.Country),
                new FighterResponse(
                    fightEntity.FighterB.Id,
                    fightEntity.FighterB.FirstName,
                    fightEntity.FighterB.LastName,
                    fightEntity.FighterB.Nickname,
                    fightEntity.FighterB.Country),
                fightEntity.ScheduledRounds,
                fightEntity.Status))
            .SingleOrDefaultAsync(cancellationToken);

        return fight is null ? NotFound() : Ok(fight);
    }
}
