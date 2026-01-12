using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdleGarageBackend.Data;
using IdleGarageBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IdleGarageBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IdleGarageDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IdleGarageDbContext db,
        IConfiguration cfg)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _cfg = cfg;
    }

    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(req.Email);
        if (existing is not null)
            return Conflict(new { error = "Ezzel az emaillel már létezik felhasználó." });

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = req.Email,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description).ToArray() });

        _db.Workshops.Add(new Workshop
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Money = 200,
            Level = 1,
            Exp = 0,
            LastSeenAtUtc = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync(ct);

        var token = CreateJwt(user);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
            return Unauthorized(new { error = "Hibás email vagy jelszó." });

        var ok = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: false);
        if (!ok.Succeeded)
            return Unauthorized(new { error = "Hibás email vagy jelszó." });

        var token = CreateJwt(user);
        return Ok(new { token });
    }

    private string CreateJwt(AppUser user)
    {
        var issuer = _cfg["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer") ?? "IdleGarageBackend";
        var audience = _cfg["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience") ?? "IdleGarageFrontend";
        var key = _cfg["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key");

        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing Jwt__Key");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}