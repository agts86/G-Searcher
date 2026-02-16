# API Agent Guide

このファイルは `API/` 配下専用のルールを定義する。

## 1. 技術スタック前提

- ASP.NET Core Web API (`net10.0`)
- EF Core + PostgreSQL (`UseNpgsql`)
- Migration は `dotnet-ef` を使用

## 2. 実装ルール

- 依存方向は `Controller -> Service -> Repository` を維持
- `Host/Program.cs` に DI 設定を集約
- プロジェクト参照は `Presentation -> Application`, `Infrastructure -> Application`, `Host -> (Presentation, Application, Infrastructure)` を維持
- `Application` / `Presentation` から `DbContext` を直接参照しない
- DB アクセスは `LineWebHookContext` 経由で統一
- 設定キーは `ConnectionStrings:PostgreSQLConnection` を使用
- 環境別設定は `appsettings.Development.json` を使用

## 3. Migration ルール

- Migration ファイルの手編集は禁止
- 変更は `dotnet tool run dotnet-ef migrations add/remove` で実施
- 変更後は `LineWebHookContextModelSnapshot` の整合を確認

## 4. チェックコマンド

- `dotnet build API/Host/Host.csproj`
- `dotnet test API/Test/Test.csproj`

## 5. 関連仕様

- 詳細仕様: `API/docs/INSTRUCTION.md`
- コマンド辞書: `docs/agent/commands.md`
- タグ辞書: `docs/agent/tags.md`
