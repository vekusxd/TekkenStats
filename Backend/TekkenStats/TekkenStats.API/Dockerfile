FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TekkenStats.API/TekkenStats.API.csproj", "TekkenStats.API/"]
COPY ["TekkenStats.DataAccess/TekkenStats.DataAccess.csproj", "TekkenStats.DataAccess/"]
COPY ["TekkenStats.Core/TekkenStats.Core.csproj", "TekkenStats.Core/"]
COPY ["TekkenStats.Application/TekkenStats.Application.csproj", "TekkenStats.Application/"]
RUN dotnet restore "TekkenStats.API/TekkenStats.API.csproj"
COPY . .
WORKDIR "/src/TekkenStats.API"
RUN dotnet build "TekkenStats.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TekkenStats.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TekkenStats.API.dll"]
