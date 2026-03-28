# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore
COPY ["Ims.DemoPlatform.NotificationsService/Ims.DemoPlatform.NotificationsService.csproj", "Ims.DemoPlatform.NotificationsService/"]
COPY ["Ims.DemoPlatform.Core/Ims.DemoPlatform.Core.csproj", "Ims.DemoPlatform.Core/"]
RUN dotnet restore "Ims.DemoPlatform.NotificationsService/Ims.DemoPlatform.NotificationsService.csproj"

# Publish
COPY . .
WORKDIR /src/Ims.DemoPlatform.NotificationsService
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.NotificationsService.dll"]
