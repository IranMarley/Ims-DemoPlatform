using Microsoft.AspNetCore.Identity;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

public static class IdentityExtensions
{
    public static string[] GetErrors(this IdentityResult result) =>
        result.Errors.Select(e => e.Description).ToArray();
}