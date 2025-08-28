# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore
COPY ["Ims.DemoPlatform.Core/Ims.DemoPlatform.Core.csproj", "Ims.DemoPlatform.Core/"]
COPY ["Ims.DemoPlatform.NotificationService/Ims.DemoPlatform.NotificationService.csproj", "Ims.DemoPlatform.NotificationService/"]
RUN dotnet restore "Ims.DemoPlatform.NotificationService/Ims.DemoPlatform.NotificationService.csproj"

# Publish
COPY . .
WORKDIR /src/Ims.DemoPlatform.NotificationService
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.NotificationService.dll"]
