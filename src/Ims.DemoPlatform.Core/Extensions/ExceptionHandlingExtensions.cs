using Ims.DemoPlatform.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Ims.DemoPlatform.Core.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}