using Ims.DemoPlatform.Identity.API.Configuration;
using Ims.DemoPlatform.WebApi.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddGlobalExceptionHandling();
builder.AddApiConfiguration();
builder.AddIdentityConfiguration();
builder.AddSwaggerConfiguration();
builder.AddServiceBusConfiguration();

var app = builder.Build();

app.UseApiConfiguration();
app.UseSwaggerConfiguration(app.Environment);

app.Run();
