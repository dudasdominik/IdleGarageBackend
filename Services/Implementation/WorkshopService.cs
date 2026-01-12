using IdleGarageBackend.Data;
using IdleGarageBackend.Models;
using IdleGarageBackend.Models.DTOs;
using IdleGarageBackend.Services.Interface;

namespace IdleGarageBackend.Services.Implementation;

public class WorkshopService : IdleGarageService, IWorkshopService
{
    private readonly IWorkshopRepository _workshops;
    private readonly IReferenceDataRepository _refData;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    
    public WorkshopService(
        IWorkshopRepository workshops,
        IReferenceDataRepository refData,
        IUnitOfWork uow,
        IClock clock,
        IdleGarageDbContext context) : base(context)
    {
        _workshops = workshops;
        _refData = refData;
        _uow = uow;
        _clock = clock;
    }

    public async Task<WorkshopStateDto> GetStateAsync(Guid workshopId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;

        var workshop = await _workshops.GetAsync(workshopId, track: false, ct)
                       ?? throw new InvalidOperationException("Workshop nem található.");

        var activeJob = await _workshops.GetActiveJobAsync(workshopId, track: false, ct);

        var jobDefs = await _refData.GetJobDefinitionsAsync(ct);
        var upgradeDefs = await _refData.GetUpgradeDefinitionsAsync(ct);
        var workshopUpgrades = await _workshops.GetUpgradesAsync(workshopId, track: false, ct);
        ActiveJobDto? activeDto = null;
        if (activeJob is not null)
        {
            var status = now >= activeJob.CompletesAtUtc ? "Completed" : "Running";
            var remaining = Math.Max(0, (int)Math.Ceiling((activeJob.CompletesAtUtc - now).TotalSeconds));

            activeDto = new ActiveJobDto(
                activeJob.Id,
                activeJob.JobDefinition.Name,
                status,
                activeJob.StartedAtUtc,
                activeJob.CompletesAtUtc,
                remaining,
                activeJob.RewardAtStart
            );
        }

        var jobsDto = jobDefs.Select(j => new JobDefDto
        (
            j.Id,
            j.Name,
            j.BaseSeconds,
            j.BaseReward,
            j.RequiredLevel
        )).ToList();

        var upgradesDto = upgradeDefs.Select(def =>
        {
            var level = workshopUpgrades.FirstOrDefault(x => x.UpgradeDefinitionId == def.Id)?.Level ?? 0;
            var nextCost = def.BaseCost * (level + 1);

            return new UpgradeDto
            (
                def.Id,
                def.Name,
                def.Type.ToString(),
                level,
                nextCost
            );
        }).ToList();

        return new WorkshopStateDto
        (
            workshop.Id,
            workshop.Money,
            workshop.Level,
            workshop.Exp,
            activeDto,
            jobsDto,
            upgradesDto
        );
    }

    public async Task StartJobAsync(Guid workshopId, Guid jobDefinitionId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;

        await using var tx = await _uow.BeginTransactionAsync(ct);

        var workshop = await _workshops.GetAsync(workshopId, track: true, ct)
                       ?? throw new InvalidOperationException("Workshop nem található.");

        var existing = await _workshops.GetActiveJobAsync(workshopId, track: false, ct);
        if (existing is not null)
            throw new InvalidOperationException("Már van aktív vagy claim nélküli munka.");

        var def = await _refData.GetJobDefinitionAsync(jobDefinitionId, ct)
                  ?? throw new InvalidOperationException("Nincs ilyen munka definíció.");

        if (workshop.Level < def.RequiredLevel)
            throw new InvalidOperationException("Nincs elég szint ehhez a munkához.");

        var upgrades = await _workshops.GetUpgradesAsync(workshopId, track: false, ct);
        var speedLevel = GetUpgradeLevel(upgrades, UpgradeType.Speed);
        var rewardLevel = GetUpgradeLevel(upgrades, UpgradeType.Reward);

        
        var speedFactor = Math.Max(0.30, 1.0 - 0.05 * speedLevel);
        var durationSeconds = (int)Math.Ceiling(def.BaseSeconds * speedFactor);
        
        var rewardFactor = 1.0 + 0.10 * rewardLevel;
        var reward = (int)Math.Ceiling(def.BaseReward * rewardFactor);

        var job = new WorkshopJob
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshopId,
            JobDefinitionId = def.Id,
            StartedAtUtc = now,
            CompletesAtUtc = now.AddSeconds(durationSeconds),
            RewardAtStart = reward,
            DurationSecondsAtStart = durationSeconds,
            ClaimedAtUtc = null
        };

        await _workshops.AddJobAsync(job, ct);

        workshop.LastSeenAtUtc = now;

        await _uow.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    public async Task<int> ClaimAsync(Guid workshopId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;

        await using var tx = await _uow.BeginTransactionAsync(ct);

        var workshop = await _workshops.GetAsync(workshopId, track: true, ct)
                       ?? throw new InvalidOperationException("Workshop nem található.");

        var job = await _workshops.GetActiveJobAsync(workshopId, track: true, ct);
        if (job is null)
            throw new InvalidOperationException("Nincs claimelhető munka.");

        if (now < job.CompletesAtUtc)
            throw new InvalidOperationException("A munka még nem készült el.");

        job.ClaimedAtUtc = now;
        workshop.Money += job.RewardAtStart;
        
        workshop.Exp += 1;
        var need = 5 * workshop.Level;
        if (workshop.Exp >= need)
        {
            workshop.Level += 1;
            workshop.Exp = 0;
        }

        workshop.LastSeenAtUtc = now;

        await _uow.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return job.RewardAtStart;
    }

    public async Task BuyUpgradeAsync(Guid workshopId, Guid upgradeDefinitionId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;

        await using var tx = await _uow.BeginTransactionAsync(ct);

        var workshop = await _workshops.GetAsync(workshopId, track: true, ct)
                       ?? throw new InvalidOperationException("Workshop nem található.");

        var def = await _refData.GetUpgradeDefinitionAsync(upgradeDefinitionId, ct)
                  ?? throw new InvalidOperationException("Nincs ilyen upgrade definíció.");

        var existing = await _workshops.GetUpgradeAsync(workshopId, upgradeDefinitionId, track: true, ct);
        var currentLevel = existing?.Level ?? 0;

        var nextCost = def.BaseCost * (currentLevel + 1);
        if (workshop.Money < nextCost)
            throw new InvalidOperationException("Nincs elég pénzed.");

        workshop.Money -= nextCost;

        if (existing is null)
        {
            await _workshops.AddUpgradeAsync(new WorkshopUpgrade
            {
                WorkshopId = workshopId,
                UpgradeDefinitionId = def.Id,
                Level = 1
            }, ct);
        }
        else
        {
            existing.Level += 1;
        }

        workshop.LastSeenAtUtc = now;

        await _uow.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
    
    private static int GetUpgradeLevel(IEnumerable<WorkshopUpgrade> upgrades, UpgradeType type)
        => upgrades.Where(u => u.UpgradeDefinition.Type == type).Sum(u => u.Level);
}