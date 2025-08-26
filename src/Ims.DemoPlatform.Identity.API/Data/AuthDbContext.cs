using Ims.DemoPlatform.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ims.DemoPlatform.Identity.API.Data;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}

public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
