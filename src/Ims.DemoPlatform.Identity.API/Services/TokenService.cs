using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Services;

public class TokenService
{
    private readonly JwtOptions _opt;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthDbContext _db;

    public TokenService(IOptions<JwtOptions> opt, UserManager<ApplicationUser> userManager, AuthDbContext db)
    {
        _opt = opt.Value;
        _userManager = userManager;
        _db = db;
    }

    public async Task<(string access, string refresh)> IssueTokenPairAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

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

        return (access, refresh.Token);
    }

    public async Task<(string access, string refresh)?> RefreshAsync(string refreshToken)
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
