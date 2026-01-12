using IdleGarageBackend.Models;

namespace IdleGarageBackend.Services.Interface;

public interface IWorkshopRepository
{
    Task<Workshop?> GetAsync(Guid workshopId, bool track = true, CancellationToken ct = default);

    Task<WorkshopJob?> GetActiveJobAsync(Guid workshopId, bool track = true, CancellationToken ct = default);

    Task AddJobAsync(WorkshopJob job, CancellationToken ct = default);

    Task<IReadOnlyList<WorkshopUpgrade>> GetUpgradesAsync(Guid workshopId, bool track = false, CancellationToken ct = default);

    Task<WorkshopUpgrade?> GetUpgradeAsync(Guid workshopId, Guid upgradeDefinitionId, bool track = true, CancellationToken ct = default);

    Task AddUpgradeAsync(WorkshopUpgrade upgrade, CancellationToken ct = default);
    
    Task<Workshop?> GetByUserIdAsync(Guid userId, bool track = true, CancellationToken ct = default);

}