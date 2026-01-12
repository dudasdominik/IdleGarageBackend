using IdleGarageBackend.Models;

namespace IdleGarageBackend.Services.Interface;

public interface IReferenceDataRepository
{
    Task<IReadOnlyList<JobDefinition>> GetJobDefinitionsAsync(CancellationToken ct = default);
    Task<JobDefinition?> GetJobDefinitionAsync(Guid jobDefinitionId, CancellationToken ct = default);

    Task<IReadOnlyList<UpgradeDefinition>> GetUpgradeDefinitionsAsync(CancellationToken ct = default);
    Task<UpgradeDefinition?> GetUpgradeDefinitionAsync(Guid upgradeDefinitionId, CancellationToken ct = default);
}