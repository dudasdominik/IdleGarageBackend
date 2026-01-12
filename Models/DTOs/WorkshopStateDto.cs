namespace IdleGarageBackend.Models.DTOs;

public record WorkshopStateDto(
    Guid WorkshopId,
    int Money,
    int Level,
    int Exp,
    ActiveJobDto? ActiveJob,
    List<JobDefDto> Jobs,
    List<UpgradeDto> Upgrades
)
{
    public static WorkshopStateDto From(
        Workshop workshop,
        WorkshopJob? activeJob,
        DateTimeOffset now,
        List<JobDefinition> jobDefs,
        List<UpgradeDefinition> upgradeDefs,
        List<WorkshopUpgrade> upgradeLevels)
    {
        ActiveJobDto? active = null;

        if (activeJob is not null)
        {
            var status = now >= activeJob.CompletesAtUtc ? "Completed" : "Running";
            var remainingSeconds = Math.Max(0, (int)Math.Ceiling((activeJob.CompletesAtUtc - now).TotalSeconds));

            active = new ActiveJobDto(
                activeJob.Id,
                activeJob.JobDefinition.Name,
                status,
                activeJob.StartedAtUtc,
                activeJob.CompletesAtUtc,
                remainingSeconds,
                activeJob.RewardAtStart
            );
        }

        var jobs = jobDefs.Select(j => new JobDefDto(
            j.Id, j.Name, j.BaseSeconds, j.BaseReward, j.RequiredLevel
        )).ToList();

        var upgrades = upgradeDefs.Select(u =>
        {
            var level = upgradeLevels.FirstOrDefault(x => x.UpgradeDefinitionId == u.Id)?.Level ?? 0;
            var nextCost = u.BaseCost * (level + 1);
            return new UpgradeDto(u.Id, u.Name, u.Type.ToString(), level, nextCost);
        }).ToList();

        return new WorkshopStateDto(
            workshop.Id,
            workshop.Money,
            workshop.Level,
            workshop.Exp,
            active,
            jobs,
            upgrades
        );
    }
}