#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["API/LineWebHookAPI/", "LineWebHookAPI/"]
RUN dotnet restore "LineWebHookAPI/LineWebHookAPI.csproj"
COPY . .
WORKDIR "/src/LineWebHookAPI"
RUN dotnet build "LineWebHookAPI/LineWebHookAPI.csproj" -c publish -o /app/build

FROM build AS publish
RUN dotnet publish "LineWebHookAPI/LineWebHookAPI.csproj" -c publish -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf && \
    sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf

ENTRYPOINT ["dotnet", "LineWebHookAPI.dll"]