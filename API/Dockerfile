# ベースイメージ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# SDKイメージ
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# csproj のみをコピーしてリストア
COPY ["LineWebHookAPI/LineWebHookAPI.csproj", "LineWebHookAPI/"]
RUN dotnet restore "LineWebHookAPI/LineWebHookAPI.csproj"

# 残りのファイルをコピーしてビルド
COPY ["LineWebHookAPI/", "LineWebHookAPI/"]
WORKDIR "/src/LineWebHookAPI"
RUN dotnet build "LineWebHookAPI.csproj" -c publish -o /app/build

# パブリッシュ
FROM build AS publish
RUN dotnet publish "LineWebHookAPI.csproj" -c publish -o /app/publish /p:UseAppHost=false

# 実行環境
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "LineWebHookAPI.dll"]