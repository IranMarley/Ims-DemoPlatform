using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ims.DemoPlatform.Identity.API.Data;
using Ims.DemoPlatform.Identity.API.Models;
using Ims.DemoPlatform.Identity.API.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ims.DemoPlatform.Identity.API.Services;

public interface ITokenService
{
    Task<TokenPair> IssueTokenPairAsync(ApplicationUser user);
    Task<TokenPair?> RefreshAsync(string refreshToken);
    Task<bool> RevokeAsync(string refreshToken);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _opt;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AuthDbContext _db;

    public TokenService(IOptions<JwtOptions> opt, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        AuthDbContext db)
    {
        _opt = opt.Value;
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task<TokenPair> IssueTokenPairAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Role, roles.FirstOrDefault() ?? ""),
        };
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);
        
        foreach (var role in roles)
        {
            var identityRole = await _roleManager.FindByNameAsync(role);
            
            if (identityRole is null) continue;
            
            var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
            claims.AddRange(roleClaims);
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
            signingCredentials: creds);
        var access = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refresh = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays)
        };
        
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        return new TokenPair(access, refresh.Token, refresh.ExpiresAtUtc);
    }

    public async Task<TokenPair?> RefreshAsync(string refreshToken)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.Revoked && x.ExpiresAtUtc > DateTime.UtcNow);
        if (rt is null) return null;
        var user = await _db.Users.FindAsync(rt.UserId);
        if (user is null) return null;

        rt.Revoked = true;
        await _db.SaveChangesAsync();

        return await IssueTokenPairAsync(user);
    }

    public async Task<bool> RevokeAsync(string refreshToken)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && !x.Revoked);
        if (rt is null) return false;
        rt.Revoked = true;
        await _db.SaveChangesAsync();
        return true;
    }
}
