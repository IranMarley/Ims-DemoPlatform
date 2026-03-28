# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore
COPY ["Ims.DemoPlatform.Projects.API/Ims.DemoPlatform.Projects.API.csproj", "Ims.DemoPlatform.Projects.API/"]
COPY ["Ims.DemoPlatform.Core/Ims.DemoPlatform.Core.csproj", "Ims.DemoPlatform.Core/"]
COPY ["Ims.DemoPlatform.WebApi.Core/Ims.DemoPlatform.WebApi.Core.csproj", "Ims.DemoPlatform.WebApi.Core/"]
RUN dotnet restore "Ims.DemoPlatform.Projects.API/Ims.DemoPlatform.Projects.API.csproj"

# Publish
COPY . .
WORKDIR /src/Ims.DemoPlatform.Projects.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.Projects.API.dll"]
