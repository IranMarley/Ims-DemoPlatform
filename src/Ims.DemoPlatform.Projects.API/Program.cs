using Ims.DemoPlatform.Projects.API.Configuration;
using Ims.DemoPlatform.WebApi.Core.Extensions;

SerilogExtensions.RunWithSerilog(() =>
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogLogging();
    builder.AddApiServices();
    builder.AddApiConfiguration();
    builder.AddServiceBusConfiguration();
    
    builder.AddSwaggerConfiguration(
        apiTitle: "IMS Demo Platform Project API",
        apiDescription: "API for managing projects"
    );
    
    builder.AddJwtAuthentication(requireHttpsMetadata: false);

    var app = builder.Build();

    app.UseSwaggerConfiguration(app.Environment);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}, "Project API");
