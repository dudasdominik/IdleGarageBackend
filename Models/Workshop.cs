namespace IdleGarageBackend.Models;

public class Workshop
{
    public Guid Id { get; set; }
    public int Money { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; } 
    public DateTimeOffset LastSeenAtUtc { get; set; }
    
    public AppUser? User { get; set; }
    public Guid UserId { get; set; }

    public List<WorkshopJob> Jobs { get; set; } = [];
    public List<WorkshopUpgrade> Upgrades { get; set; } = [];
}