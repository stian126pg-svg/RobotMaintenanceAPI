FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY RobotMaintenanceAPI.csproj ./
RUN dotnet restore RobotMaintenanceAPI.csproj

COPY . ./

RUN dotnet publish RobotMaintenanceAPI.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

COPY --from=build /app/publish ./

USER $APP_UID

ENTRYPOINT ["dotnet", "RobotMaintenanceAPI.dll"]