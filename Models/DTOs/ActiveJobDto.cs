namespace IdleGarageBackend.Models.DTOs;

public record ActiveJobDto(
    Guid JobId,
    string Name,
    string Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset CompletedAtUtc,
    int RemainingSeconds,
    int Reward)
{
   
}