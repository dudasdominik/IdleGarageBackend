using Microsoft.AspNetCore.Identity;

namespace IdleGarageBackend.Models;

public class AppUser : IdentityUser<Guid>
{
   public string? DisplayName { get; set; }

}