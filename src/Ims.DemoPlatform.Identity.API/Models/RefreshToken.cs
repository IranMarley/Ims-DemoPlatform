using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public class RefreshToken
{
    [Key] public int Id { get; set; }
    [Required] public string Token { get; set; } = default!;
    [Required] public string UserId { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool Revoked { get; set; }
}
