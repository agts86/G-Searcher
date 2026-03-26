引数を Migration 名として以下を実行してください。

```bash
dotnet tool run dotnet-ef migrations add $ARGUMENTS \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj \
  --output-dir Migrations
```
