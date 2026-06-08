FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Moniquette.Server/Moniquette.Server.csproj", "Moniquette.Server/"]
COPY ["Moniquette.Common/Moniquette.Common.csproj", "Moniquette.Common/"]
COPY ["Moniquette.Elastic/Moniquette.Elastic.csproj", "Moniquette.Elastic/"]
RUN dotnet restore "Moniquette.Server/Moniquette.Server.csproj"
COPY . .
WORKDIR "/src/Moniquette.Server"
RUN dotnet build "./Moniquette.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Moniquette.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Moniquette.Server.dll"]
