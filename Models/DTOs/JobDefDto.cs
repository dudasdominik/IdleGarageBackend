namespace IdleGarageBackend.Models.DTOs;

public record JobDefDto(
    Guid Id,
    string Name,
    int BaseSeconds,
    int BaseReward,
    int RequiredLevel
    );