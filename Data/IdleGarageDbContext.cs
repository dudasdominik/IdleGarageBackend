using IdleGarageBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdleGarageBackend.Data;

public class IdleGarageDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public IdleGarageDbContext(DbContextOptions<IdleGarageDbContext> options) : base(options) { }

    public DbSet<JobDefinition> JobDefinitions { get; set; } = null!;
    public DbSet<UpgradeDefinition> UpgradeDefinitions { get; set; } = null!;
    public DbSet<Workshop> Workshops { get; set; } = null!;
    public DbSet<WorkshopJob> WorkshopJobs { get; set; } = null!;
    public DbSet<WorkshopUpgrade> WorkshopUpgrades { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Workshop>()
            .HasIndex(w => w.UserId)
            .IsUnique();

        modelBuilder.Entity<WorkshopUpgrade>()
            .HasKey(x => new { x.WorkshopId, x.UpgradeDefinitionId });

        modelBuilder.Entity<Workshop>()
            .HasOne(w => w.User)
            .WithOne()
            .HasForeignKey<Workshop>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // JOBS (seed) ----------------------------------------------------------

        var oilChangeId     = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        var brakesId        = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
        var alignmentId     = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
        var clutchId        = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");

        var airFilterId     = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5");
        var tirePressureId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6");
        var carWashId       = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7");
        var wiperId         = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa8");

        var obdId           = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa9");
        var batteryId       = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10");
        var coolantId       = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa11");
        var sparkPlugsId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa12");

        var brakeFluidId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa13");
        var wheelBearingId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa14");
        var suspensionId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa15");
        var alternatorId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa16");

        var timingBeltId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa17");
        var injectorId      = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa18");
        var fuelPumpId      = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa19");
        var turboHoseId     = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa20");

        var radiatorId      = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa21");
        var engineMountId   = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa22");
        var transServiceId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa23");
        var headGasketId    = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa24");

        var turboRebuildId  = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa25");
        var fullServiceId   = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa26");

        modelBuilder.Entity<JobDefinition>().HasData(
            // L1
            new JobDefinition { Id = oilChangeId,    Name = "Olajcsere",               BaseSeconds = 20,  BaseReward = 60,   RequiredLevel = 1 },
            new JobDefinition { Id = brakesId,       Name = "Fékbetét csere",          BaseSeconds = 35,  BaseReward = 110,  RequiredLevel = 1 },
            new JobDefinition { Id = airFilterId,    Name = "Levegőszűrő csere",       BaseSeconds = 18,  BaseReward = 55,   RequiredLevel = 1 },
            new JobDefinition { Id = tirePressureId, Name = "Guminyomás ellenőrzés",   BaseSeconds = 12,  BaseReward = 35,   RequiredLevel = 1 },

            // L2
            new JobDefinition { Id = carWashId,      Name = "Külső-belső takarítás",   BaseSeconds = 45,  BaseReward = 140,  RequiredLevel = 2 },
            new JobDefinition { Id = wiperId,        Name = "Ablaktörlő csere",        BaseSeconds = 22,  BaseReward = 80,   RequiredLevel = 2 },
            new JobDefinition { Id = obdId,          Name = "OBD diagnosztika",        BaseSeconds = 28,  BaseReward = 120,  RequiredLevel = 2 },
            new JobDefinition { Id = alignmentId,    Name = "Futómű állítás",          BaseSeconds = 50,  BaseReward = 170,  RequiredLevel = 2 },

            // L3
            new JobDefinition { Id = batteryId,      Name = "Akkumulátor teszt/csere", BaseSeconds = 55,  BaseReward = 210,  RequiredLevel = 3 },
            new JobDefinition { Id = coolantId,      Name = "Hűtőfolyadék csere",      BaseSeconds = 60,  BaseReward = 230,  RequiredLevel = 3 },
            new JobDefinition { Id = sparkPlugsId,   Name = "Gyertyacsere",            BaseSeconds = 48,  BaseReward = 200,  RequiredLevel = 3 },
            new JobDefinition { Id = clutchId,       Name = "Kuplung csere",           BaseSeconds = 80,  BaseReward = 260,  RequiredLevel = 3 },

            // L4
            new JobDefinition { Id = brakeFluidId,   Name = "Fékfolyadék csere",       BaseSeconds = 75,  BaseReward = 300,  RequiredLevel = 4 },
            new JobDefinition { Id = wheelBearingId, Name = "Kerékcsapágy csere",      BaseSeconds = 95,  BaseReward = 380,  RequiredLevel = 4 },
            new JobDefinition { Id = suspensionId,   Name = "Futómű szilent csere",    BaseSeconds = 110, BaseReward = 430,  RequiredLevel = 4 },
            new JobDefinition { Id = alternatorId,   Name = "Generátor csere",         BaseSeconds = 120, BaseReward = 470,  RequiredLevel = 4 },

            // L5
            new JobDefinition { Id = timingBeltId,   Name = "Vezérlés csere",          BaseSeconds = 140, BaseReward = 560,  RequiredLevel = 5 },
            new JobDefinition { Id = injectorId,     Name = "Injektor tisztítás",      BaseSeconds = 150, BaseReward = 610,  RequiredLevel = 5 },
            new JobDefinition { Id = fuelPumpId,     Name = "Üzemanyagpumpa csere",    BaseSeconds = 165, BaseReward = 680,  RequiredLevel = 5 },
            new JobDefinition { Id = turboHoseId,    Name = "Turbócső csere",          BaseSeconds = 175, BaseReward = 720,  RequiredLevel = 5 },

            // L6
            new JobDefinition { Id = radiatorId,     Name = "Hűtő csere",              BaseSeconds = 210, BaseReward = 850,  RequiredLevel = 6 },
            new JobDefinition { Id = engineMountId,  Name = "Motortartó bak csere",    BaseSeconds = 195, BaseReward = 800,  RequiredLevel = 6 },
            new JobDefinition { Id = transServiceId, Name = "Váltó olajcsere",         BaseSeconds = 200, BaseReward = 820,  RequiredLevel = 6 },
            new JobDefinition { Id = headGasketId,   Name = "Hengerfej tömítés csere", BaseSeconds = 320, BaseReward = 1200, RequiredLevel = 7 },

            // L8+
            new JobDefinition { Id = turboRebuildId, Name = "Turbó felújítás",         BaseSeconds = 420, BaseReward = 1650, RequiredLevel = 8 },
            new JobDefinition { Id = fullServiceId,  Name = "Teljes szerviz csomag",   BaseSeconds = 520, BaseReward = 2100, RequiredLevel = 9 }
        );

        // UPGRADES (seed) ------------------------------------------------------

        var speedUpId   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
        var rewardUpId  = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");

        var speed2Id    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
        var reward2Id   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");

        var speed3Id    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5");
        var reward3Id   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6");

        var speed4Id    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb7");
        var reward4Id   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb8");

        var speed5Id    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb9");
        var reward5Id   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb10");

        var speed6Id    = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb11");
        var reward6Id   = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb12");

        modelBuilder.Entity<UpgradeDefinition>().HasData(
            new UpgradeDefinition { Id = speedUpId,  Name = "Gyorsabb szerelés I",   Type = UpgradeType.Speed,  BaseCost = 120 },
            new UpgradeDefinition { Id = rewardUpId, Name = "Nagyobb bevétel I",     Type = UpgradeType.Reward, BaseCost = 150 },

            new UpgradeDefinition { Id = speed2Id,   Name = "Pro szerszámok",        Type = UpgradeType.Speed,  BaseCost = 350 },
            new UpgradeDefinition { Id = reward2Id,  Name = "Drágább munkadíj",      Type = UpgradeType.Reward, BaseCost = 420 },

            new UpgradeDefinition { Id = speed3Id,   Name = "Emelő + kompresszor",   Type = UpgradeType.Speed,  BaseCost = 900 },
            new UpgradeDefinition { Id = reward3Id,  Name = "Prémium ügyfelek",      Type = UpgradeType.Reward, BaseCost = 1100 },

            new UpgradeDefinition { Id = speed4Id,   Name = "Műhely optimalizálás",  Type = UpgradeType.Speed,  BaseCost = 2200 },
            new UpgradeDefinition { Id = reward4Id,  Name = "Flotta szerződések",    Type = UpgradeType.Reward, BaseCost = 2600 },

            new UpgradeDefinition { Id = speed5Id,   Name = "Versenyszintű workflow",Type = UpgradeType.Speed,  BaseCost = 5000 },
            new UpgradeDefinition { Id = reward5Id,  Name = "VIP ügyfélkör",         Type = UpgradeType.Reward, BaseCost = 6500 },

            new UpgradeDefinition { Id = speed6Id,   Name = "Gyári diagnosztikai eszközök", Type = UpgradeType.Speed,  BaseCost = 12000 },
            new UpgradeDefinition { Id = reward6Id,  Name = "Exkluzív projektek",          Type = UpgradeType.Reward, BaseCost = 15000 }
        );
    }
}
