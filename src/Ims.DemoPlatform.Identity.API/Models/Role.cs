using System.ComponentModel.DataAnnotations;

namespace Ims.DemoPlatform.Identity.API.Models;

public record RoleDto([Required] string Name);

public record RoleAssignDto([Required, EmailAddress] string Email, [Required] string Role);
