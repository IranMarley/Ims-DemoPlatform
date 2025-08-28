# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore
COPY ["Ims.DemoPlatform.Identity.API/Ims.DemoPlatform.Identity.API.csproj", "Ims.DemoPlatform.Identity.API/"]
COPY ["Ims.DemoPlatform.Core/Ims.DemoPlatform.Core.csproj", "Ims.DemoPlatform.Core/"]
COPY ["Ims.DemoPlatform.WebApi.Core/Ims.DemoPlatform.WebApi.Core.csproj", "Ims.DemoPlatform.WebApi.Core/"]
RUN dotnet restore "Ims.DemoPlatform.Identity.API/Ims.DemoPlatform.Identity.API.csproj"

# Publish
COPY . .
WORKDIR /src/Ims.DemoPlatform.Identity.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.Identity.API.dll"]
