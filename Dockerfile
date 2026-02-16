# ベースイメージ
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80;https://+:443
EXPOSE 80
EXPOSE 443

# UI ビルド
FROM node:24-bookworm-slim AS ui-build
WORKDIR /src/UI
ENV PNPM_HOME=/pnpm
ENV PATH=${PNPM_HOME}:${PATH}
RUN corepack enable && corepack prepare pnpm@10.26.0 --activate
COPY UI/package.json UI/pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY UI/ ./
RUN pnpm build

# SDKイメージ
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# csproj のみをコピーしてリストア
COPY ["API/LineWebHookAPI/LineWebHookAPI.csproj", "API/LineWebHookAPI/"]
RUN dotnet restore "API/LineWebHookAPI/LineWebHookAPI.csproj"

# 残りのファイルをコピーしてビルド
COPY ["API/LineWebHookAPI/", "API/LineWebHookAPI/"]
WORKDIR "/src/API/LineWebHookAPI"
RUN dotnet build "LineWebHookAPI.csproj" -c publish -o /app/build

# パブリッシュ
FROM build AS publish
RUN dotnet publish "LineWebHookAPI.csproj" -c publish -o /app/publish /p:UseAppHost=false

# 実行環境
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=ui-build /src/UI/out ./wwwroot
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "LineWebHookAPI.dll"]
