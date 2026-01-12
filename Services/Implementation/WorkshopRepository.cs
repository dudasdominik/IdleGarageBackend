using IdleGarageBackend.Data;
using IdleGarageBackend.Models;
using IdleGarageBackend.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace IdleGarageBackend.Services.Implementation;

public class WorkshopRepository : IdleGarageService, IWorkshopRepository
{
    public WorkshopRepository(IdleGarageDbContext context) : base(context)
    {
    }

    public async Task<Workshop?> GetAsync(Guid workshopId, bool track = true, CancellationToken ct = default)
    {
        var q = _context.Workshops.AsQueryable();
        if (!track) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(w => w.Id == workshopId, ct);
    }

    public async Task<WorkshopJob?> GetActiveJobAsync(Guid workshopId, bool track = true, CancellationToken ct = default)
    {
        var q = _context.WorkshopJobs
            .Include(j => j.JobDefinition)
            .Where(j => j.WorkshopId == workshopId && j.ClaimedAtUtc == null)
            .OrderByDescending(j => j.StartedAtUtc)
            .AsQueryable();
        if (!track) q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(ct);
    }

    public Task AddJobAsync(WorkshopJob job, CancellationToken ct = default)
    {
       _context.WorkshopJobs.Add(job);
       return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<WorkshopUpgrade>> GetUpgradesAsync(Guid workshopId, bool track = false, CancellationToken ct = default)
    {
        var q = _context.WorkshopUpgrades
            .Include(x => x.UpgradeDefinition)
            .Where(x => x.WorkshopId == workshopId)
            .AsQueryable();

        if (!track) q = q.AsNoTracking();

        return await q.ToListAsync(ct);
    }

    public async Task<WorkshopUpgrade?> GetUpgradeAsync(Guid workshopId, Guid upgradeDefinitionId, bool track = true, CancellationToken ct = default)
    {
        var q = _context.WorkshopUpgrades
            .Include(x => x.UpgradeDefinition)
            .Where(x => x.WorkshopId == workshopId && x.UpgradeDefinitionId == upgradeDefinitionId)
            .AsQueryable();

        if (!track) q = q.AsNoTracking();

        return await q.FirstOrDefaultAsync(ct);
    }
    
    public async Task<Workshop?> GetByUserIdAsync(Guid userId, bool track = true, CancellationToken ct = default)
    {
        var q = _context.Workshops.AsQueryable();
        if (!track) q = q.AsNoTracking();
        return await q.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public Task AddUpgradeAsync(WorkshopUpgrade upgrade, CancellationToken ct = default)
    {
        _context.WorkshopUpgrades.Add(upgrade);
        return Task.CompletedTask;
    }
}