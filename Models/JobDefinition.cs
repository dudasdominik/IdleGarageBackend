namespace IdleGarageBackend.Models;

public class JobDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int BaseSeconds { get; set; }
    public int BaseReward { get; set; }
    public int RequiredLevel { get; set; }
}