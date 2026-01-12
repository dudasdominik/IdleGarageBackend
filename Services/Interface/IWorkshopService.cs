using IdleGarageBackend.Models.DTOs;

namespace IdleGarageBackend.Services.Interface;

public interface IWorkshopService
{
    Task<WorkshopStateDto> GetStateAsync(Guid workshopId, CancellationToken ct = default);
    
    Task StartJobAsync(Guid workshopId, Guid jobDefinitionId, CancellationToken ct = default);
    
    Task<int> ClaimAsync(Guid workshopId, CancellationToken ct = default);

    Task BuyUpgradeAsync(Guid workshopId, Guid upgradeDefinitionId, CancellationToken ct = default);
}