namespace IdleGarageBackend.Models;

public class UpgradeDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public UpgradeType Type { get; set; }
    public int BaseCost { get; set; }
}