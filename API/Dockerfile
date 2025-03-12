FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# SQLite 用のフォルダを作成
RUN mkdir -p /app/Data

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["API/LineWebHookAPI/LineWebHookAPI.db", "/app/Data/"]
COPY ["API/LineWebHookAPI/LineWebHookAPI.csproj", "LineWebHookAPI/"]
RUN dotnet restore "LineWebHookAPI/LineWebHookAPI.csproj"

COPY ["API/LineWebHookAPI/", "LineWebHookAPI/"]
WORKDIR "/src/LineWebHookAPI"
RUN dotnet build "LineWebHookAPI.csproj" -c publish -o /app/build

FROM build AS publish
RUN dotnet publish "LineWebHookAPI.csproj" -c publish -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# SQLite のデータベースを /app/Data に配置
VOLUME /app/Data

ENTRYPOINT ["dotnet", "LineWebHookAPI.dll"]