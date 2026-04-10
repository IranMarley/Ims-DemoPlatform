using System.ComponentModel.DataAnnotations;

namespace Ims.DemoPlatform.Identity.API.Models;

public record UserDto([Required] string Id, [Required] string Name, [Required, EmailAddress] string Email, [Required] string Role, [Required] string Password);

public record CreateUserDto([Required, EmailAddress] string Email, [Required] string Password, [Required] string Role);

public record UpdateUserDto([Required, EmailAddress] string Email, [Required] string Role);

public record UserResponseDto(string Id, string Email, IList<string> Roles, bool EmailConfirmed);
