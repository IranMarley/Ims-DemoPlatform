using System.ComponentModel.DataAnnotations;

namespace Ims.DemoPlatform.Identity.API.Models;

public record UserDto([Required] string Id, [Required] string Name, [Required, EmailAddress] string Email, [Required] string Role, [Required] string Password);
