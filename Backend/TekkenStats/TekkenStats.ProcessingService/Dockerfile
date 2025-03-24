FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TekkenStats.ProcessingService/TekkenStats.ProcessingService.csproj", "TekkenStats.ProcessingService/"]
COPY ["TekkenStats.Application/TekkenStats.Application.csproj", "TekkenStats.Application/"]
COPY ["TekkenStats.Core/TekkenStats.Core.csproj", "TekkenStats.Core/"]
COPY ["TekkenStats.DataAccess/TekkenStats.DataAccess.csproj", "TekkenStats.DataAccess/"]
RUN dotnet restore "TekkenStats.ProcessingService/TekkenStats.ProcessingService.csproj"
COPY . .
WORKDIR "/src/TekkenStats.ProcessingService"
RUN dotnet build "TekkenStats.ProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TekkenStats.ProcessingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TekkenStats.ProcessingService.dll"]
