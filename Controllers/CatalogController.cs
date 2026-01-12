using IdleGarageBackend.Models;
using IdleGarageBackend.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IdleGarageBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IReferenceDataRepository _refData;

    public CatalogController(IReferenceDataRepository refData)
    {
        _refData = refData;
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<IReadOnlyList<JobDefinition>>> GetJobs(CancellationToken ct)
    {
        var jobs = await _refData.GetJobDefinitionsAsync(ct);
        return Ok(jobs);
    }

    [HttpGet("upgrades")]
    public async Task<ActionResult<IReadOnlyList<UpgradeDefinition>>> GetUpgrades(CancellationToken ct)
    {
        var upgrades = await _refData.GetUpgradeDefinitionsAsync(ct);
        return Ok(upgrades);
    }
}