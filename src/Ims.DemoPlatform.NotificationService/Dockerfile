# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Ims.DemoPlatform.NotificationService.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet","Ims.DemoPlatform.NotificationService.dll"]
