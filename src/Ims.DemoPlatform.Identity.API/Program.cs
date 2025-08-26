using Ims.DemoPlatform.Identity.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentityConfiguration();
builder.AddApiConfiguration();
builder.AddServiceBusConfiguration();
builder.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration();
app.UseSwaggerConfiguration(app.Environment);

app.Run();
