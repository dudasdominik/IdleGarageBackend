using System.Security.Claims;
using IdleGarageBackend.Models.DTOs;
using IdleGarageBackend.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleGarageBackend.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkshopController : ControllerBase
{
    private readonly IWorkshopService _workshopService;
    private readonly IWorkshopRepository _workshopRepository;

    public WorkshopController(IWorkshopService workshopService, IWorkshopRepository workshopRepository)
    {
        _workshopService = workshopService;
        _workshopRepository = workshopRepository;
    }
    
    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            throw new InvalidOperationException("Hiányzó user azonosító a tokenben.");

        return Guid.Parse(userIdStr);
    }

    private async Task<Guid> GetWorkshopIdForCurrentUser(CancellationToken ct)
    {
        var userId = GetUserId();
        var workshop = await _workshopRepository.GetByUserIdAsync(userId, track: false, ct);
        if (workshop is null)
            throw new InvalidOperationException("Nincs workshop ehhez a userhez (regisztrációkor kéne létrejönnie).");

        return workshop.Id;
    }

    [HttpGet("state")]
    public async Task<ActionResult<WorkshopStateDto>> GetState(CancellationToken ct)
    {
        try
        {
            var workshopId = await GetWorkshopIdForCurrentUser(ct);
            var state = await _workshopService.GetStateAsync(workshopId, ct);
            return Ok(state);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("start-job")]
    public async Task<IActionResult> StartJob([FromBody] StartJobRequest request, CancellationToken ct)
    {
        try
        {
            var workshopId = await GetWorkshopIdForCurrentUser(ct);
            await _workshopService.StartJobAsync(workshopId, request.JobDefinitionId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("claim")]
    public async Task<IActionResult> Claim(CancellationToken ct)
    {
        try
        {
            var workshopId = await GetWorkshopIdForCurrentUser(ct);
            var reward = await _workshopService.ClaimAsync(workshopId, ct);
            return Ok(new { reward });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("buy-upgrade")]
    public async Task<IActionResult> BuyUpgrade([FromBody] BuyUpgradeRequest request, CancellationToken ct)
    {
        try
        {
            var workshopId = await GetWorkshopIdForCurrentUser(ct);
            await _workshopService.BuyUpgradeAsync(workshopId, request.UpgradeDefinitionId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }
}