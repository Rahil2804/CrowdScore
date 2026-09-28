using CrowdScore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrowdScore.Api.Data;

public static class DevelopmentDataSeeder
{
    private const string SeedEventName = "CrowdScore Fight Night: Toronto";
    private static readonly DateTimeOffset SeedEventDate =
        new(2026, 10, 17, 23, 0, 0, TimeSpan.Zero);

    public static async Task SeedAsync(
        CrowdScoreDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var eventExists = await dbContext.Events.AnyAsync(
            eventEntity => eventEntity.Name == SeedEventName
                && eventEntity.EventDate == SeedEventDate,
            cancellationToken);

        if (eventExists)
        {
            return;
        }

        var mayaChen = new Fighter
        {
            FirstName = "Maya",
            LastName = "Chen",
            Nickname = "North Star",
            Country = "Canada"
        };
        var sofiaRamirez = new Fighter
        {
            FirstName = "Sofia",
            LastName = "Ramirez",
            Nickname = "El Fuego",
            Country = "Mexico"
        };
        var malikJohnson = new Fighter
        {
            FirstName = "Malik",
            LastName = "Johnson",
            Nickname = "Voltage",
            Country = "United States"
        };
        var thiagoSilva = new Fighter
        {
            FirstName = "Thiago",
            LastName = "Silva",
            Nickname = "Aco",
            Country = "Brazil"
        };
        var erinOConnor = new Fighter
        {
            FirstName = "Erin",
            LastName = "O'Connor",
            Nickname = "Tempest",
            Country = "Ireland"
        };
        var yunaSato = new Fighter
        {
            FirstName = "Yuna",
            LastName = "Sato",
            Nickname = "Phoenix",
            Country = "Japan"
        };

        var eventEntity = new Event
        {
            Name = SeedEventName,
            EventDate = SeedEventDate,
            Location = "Toronto, Ontario, Canada"
        };

        eventEntity.Fights.Add(new Fight
        {
            FighterA = mayaChen,
            FighterB = sofiaRamirez,
            CardPosition = 1,
            ScheduledRounds = 5,
            Status = FightStatus.Scheduled
        });
        eventEntity.Fights.Add(new Fight
        {
            FighterA = malikJohnson,
            FighterB = thiagoSilva,
            CardPosition = 2,
            ScheduledRounds = 3,
            Status = FightStatus.Scheduled
        });
        eventEntity.Fights.Add(new Fight
        {
            FighterA = erinOConnor,
            FighterB = yunaSato,
            CardPosition = 3,
            ScheduledRounds = 3,
            Status = FightStatus.Scheduled
        });

        await dbContext.Events.AddAsync(eventEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
