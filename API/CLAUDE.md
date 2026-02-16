# API Claude Code Guide

このファイルは `API/` 配下専用のルールを定義する。
ルート [CLAUDE.md](../CLAUDE.md) の全体ルールより優先する。

## 詳細仕様（自動インポート）

@API/docs/INSTRUCTION.md

## Migration コマンド

```bash
# Migration 追加
dotnet tool run dotnet-ef migrations add <Name> \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj \
  --output-dir Migrations

# Migration 削除
dotnet tool run dotnet-ef migrations remove \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj

# DB 適用
dotnet tool run dotnet-ef database update \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj
```

## チェックコマンド

```bash
dotnet build API/Host/Host.csproj
dotnet test API/Test/Test.csproj
```
