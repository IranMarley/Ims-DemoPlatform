FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Ims.DemoPlatform.Gateway.API"
RUN dotnet restore "Ims.DemoPlatform.Gateway.API.csproj"
RUN dotnet build "Ims.DemoPlatform.Gateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ims.DemoPlatform.Gateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ims.DemoPlatform.Gateway.API.dll"]
