namespace IdleGarageBackend.Models;

public class WorkshopJob
{
    public Guid Id { get; set; }
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public Guid JobDefinitionId { get; set; }
    public JobDefinition JobDefinition { get; set; } = null!;

    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset CompletesAtUtc { get; set; }
    public DateTimeOffset? ClaimedAtUtc { get; set; }
    
    public int RewardAtStart { get; set; }
    public int DurationSecondsAtStart { get; set; }
}