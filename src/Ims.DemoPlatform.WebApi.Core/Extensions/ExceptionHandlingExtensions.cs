using Ims.DemoPlatform.WebApi.Core.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}