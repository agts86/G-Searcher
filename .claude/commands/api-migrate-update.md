以下を実行してください。

```bash
dotnet tool run dotnet-ef database update \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj
```
