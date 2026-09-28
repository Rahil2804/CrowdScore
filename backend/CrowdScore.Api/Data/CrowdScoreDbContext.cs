using CrowdScore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrowdScore.Api.Data;

public sealed class CrowdScoreDbContext(DbContextOptions<CrowdScoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Fighter> Fighters => Set<Fighter>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Fight> Fights => Set<Fight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fighter>(fighter =>
        {
            fighter.Property(entity => entity.FirstName).HasMaxLength(100);
            fighter.Property(entity => entity.LastName).HasMaxLength(100);
            fighter.Property(entity => entity.Nickname).HasMaxLength(100);
            fighter.Property(entity => entity.Country).HasMaxLength(100);
        });

        modelBuilder.Entity<Event>(eventEntity =>
        {
            eventEntity.Property(entity => entity.Name).HasMaxLength(200);
            eventEntity.Property(entity => entity.Location).HasMaxLength(200);

            eventEntity.HasMany(entity => entity.Fights)
                .WithOne(fight => fight.Event)
                .HasForeignKey(fight => fight.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Fight>(fight =>
        {
            fight.ToTable(tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Fights_DifferentFighters",
                    "\"FighterAId\" <> \"FighterBId\"");
                tableBuilder.HasCheckConstraint(
                    "CK_Fights_PositiveCardPosition",
                    "\"CardPosition\" > 0");
                tableBuilder.HasCheckConstraint(
                    "CK_Fights_PositiveScheduledRounds",
                    "\"ScheduledRounds\" > 0");
            });

            fight.Property(entity => entity.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            fight.HasIndex(entity => new { entity.EventId, entity.CardPosition })
                .IsUnique();

            fight.HasOne(entity => entity.FighterA)
                .WithMany(fighter => fighter.FightsAsFighterA)
                .HasForeignKey(entity => entity.FighterAId)
                .OnDelete(DeleteBehavior.Restrict);

            fight.HasOne(entity => entity.FighterB)
                .WithMany(fighter => fighter.FightsAsFighterB)
                .HasForeignKey(entity => entity.FighterBId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
