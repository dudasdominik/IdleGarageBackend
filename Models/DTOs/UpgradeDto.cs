namespace IdleGarageBackend.Models.DTOs;

public record UpgradeDto(
    Guid Id,
    string Name,
    string Type,
    int Level,
    int NextCost
)
{
   
}