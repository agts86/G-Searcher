# Agent Commands (Pseudo Slash Commands)

このファイルは擬似スラッシュコマンドの辞書。  
実行時はコマンド文をそのまま CLI で実行する。

## Repo

- `/repo:status` -> `git status --short`
- `/repo:diff` -> `git diff --stat`

## API

- `/api:build` -> `dotnet build API/Host/Host.csproj`
- `/api:test` -> `dotnet test API/Test/Test.csproj`
- `/api:migrate-add <Name>` -> `dotnet tool run dotnet-ef migrations add <Name> --project API/Infrastructure/Infrastructure.csproj --startup-project API/Host/Host.csproj --output-dir Migrations`
- `/api:migrate-remove` -> `dotnet tool run dotnet-ef migrations remove --project API/Infrastructure/Infrastructure.csproj --startup-project API/Host/Host.csproj`
- `/api:migrate-update` -> `dotnet tool run dotnet-ef database update --project API/Infrastructure/Infrastructure.csproj --startup-project API/Host/Host.csproj`
