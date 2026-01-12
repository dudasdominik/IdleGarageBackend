namespace IdleGarageBackend.Models;

public class WorkshopUpgrade
{
    public Guid WorkshopId { get; set; }
    public Workshop Workshop { get; set; } = null!;

    public Guid UpgradeDefinitionId { get; set; }
    public UpgradeDefinition UpgradeDefinition { get; set; } = null!;

    public int Level { get; set; }
}