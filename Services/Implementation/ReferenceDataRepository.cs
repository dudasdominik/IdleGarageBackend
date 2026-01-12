using IdleGarageBackend.Data;
using IdleGarageBackend.Models;
using IdleGarageBackend.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace IdleGarageBackend.Services.Implementation;

public class ReferenceDataRepository : IdleGarageService ,IReferenceDataRepository
{
    public ReferenceDataRepository(IdleGarageDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<JobDefinition>> GetJobDefinitionsAsync(CancellationToken ct = default)
    {
        return await _context.JobDefinitions.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task<JobDefinition?> GetJobDefinitionAsync(Guid jobDefinitionId, CancellationToken ct = default)
    {
        return await _context.JobDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == jobDefinitionId, ct);
    }

    public async Task<IReadOnlyList<UpgradeDefinition>> GetUpgradeDefinitionsAsync(CancellationToken ct = default)
    {
        return await _context.UpgradeDefinitions.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task<UpgradeDefinition?> GetUpgradeDefinitionAsync(Guid upgradeDefinitionId, CancellationToken ct = default)
    {
        return await _context.UpgradeDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == upgradeDefinitionId, ct);
    }
}