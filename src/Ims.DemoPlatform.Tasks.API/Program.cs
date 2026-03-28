using Ims.DemoPlatform.Tasks.API.Configuration;
using Ims.DemoPlatform.WebApi.Core.Extensions;

SerilogExtensions.RunWithSerilog(() =>
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogLogging();
    builder.AddApiServices();
    builder.AddApiConfiguration();
    builder.AddServiceBusConfiguration();

    builder.AddSwaggerConfiguration(
        apiTitle: "IMS Demo Platform Task API",
        apiDescription: "API for managing tasks"
    );
    
    builder.AddJwtAuthentication(requireHttpsMetadata: false);

    var app = builder.Build();

    app.UseSwaggerConfiguration(app.Environment);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}, "Task API");
