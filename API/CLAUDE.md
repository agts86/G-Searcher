# API Claude Code Guide

このファイルは `API/` 配下専用のルールを定義する。
ルート [CLAUDE.md](../CLAUDE.md) の全体ルールより優先する。

## 詳細仕様（自動インポート）

@API/docs/INSTRUCTION.md

## Migration コマンド

```bash
# Migration 追加
dotnet tool run dotnet-ef migrations add <Name> \
  --project API/LineWebHookAPI/LineWebHookAPI.csproj \
  --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj \
  --output-dir Migrations

# Migration 削除
dotnet tool run dotnet-ef migrations remove \
  --project API/LineWebHookAPI/LineWebHookAPI.csproj \
  --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj

# DB 適用
dotnet tool run dotnet-ef database update \
  --project API/LineWebHookAPI/LineWebHookAPI.csproj \
  --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj
```

## チェックコマンド

```bash
dotnet build API/LineWebHookAPI/LineWebHookAPI.csproj
dotnet test API/LineWebHookAPITest/LineWebHookAPITest.csproj
```
