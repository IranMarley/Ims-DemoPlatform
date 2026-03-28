using Ims.DemoPlatform.Identity.API.Configuration;
using Ims.DemoPlatform.WebApi.Core.Extensions;

SerilogExtensions.RunWithSerilog(() =>
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogLogging();
    builder.AddApiServices();
    builder.AddIdentityConfiguration();
    builder.AddApiConfiguration();
    builder.AddServiceBusConfiguration();
    
    builder.AddSwaggerConfiguration(
        apiTitle: "IMS Demo Platform Identity API",
        apiDescription: "IMS Demo Platform Identity API"
    );
    
    builder.AddJwtAuthentication(requireHttpsMetadata: false);

    var app = builder.Build();

    app.UseSwaggerConfiguration(app.Environment);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}, "Identity API");
