# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore
COPY ["Ims.DemoPlatform.Tasks.API/Ims.DemoPlatform.Tasks.API.csproj", "Ims.DemoPlatform.Tasks.API/"]
COPY ["Ims.DemoPlatform.Core/Ims.DemoPlatform.Core.csproj", "Ims.DemoPlatform.Core/"]
COPY ["Ims.DemoPlatform.WebApi.Core/Ims.DemoPlatform.WebApi.Core.csproj", "Ims.DemoPlatform.WebApi.Core/"]
RUN dotnet restore "Ims.DemoPlatform.Tasks.API/Ims.DemoPlatform.Tasks.API.csproj"

# Publish
COPY . .
WORKDIR /src/Ims.DemoPlatform.Tasks.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.Tasks.API.dll"]

