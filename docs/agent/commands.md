# Agent Commands (Pseudo Slash Commands)

このファイルは擬似スラッシュコマンドの辞書。  
実行時はコマンド文をそのまま CLI で実行する。

## Repo

- `/repo:status` -> `git status --short`
- `/repo:diff` -> `git diff --stat`

## API

- `/api:build` -> `dotnet build API/LineWebHookAPI/LineWebHookAPI.csproj`
- `/api:test` -> `dotnet test API/LineWebHookAPITest/LineWebHookAPITest.csproj`
- `/api:migrate-add <Name>` -> `dotnet tool run dotnet-ef migrations add <Name> --project API/LineWebHookAPI/LineWebHookAPI.csproj --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj --output-dir Migrations`
- `/api:migrate-remove` -> `dotnet tool run dotnet-ef migrations remove --project API/LineWebHookAPI/LineWebHookAPI.csproj --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj`
- `/api:migrate-update` -> `dotnet tool run dotnet-ef database update --project API/LineWebHookAPI/LineWebHookAPI.csproj --startup-project API/LineWebHookAPI/LineWebHookAPI.csproj`

