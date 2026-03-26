以下を実行してください。

```bash
dotnet tool run dotnet-ef migrations remove \
  --project API/Infrastructure/Infrastructure.csproj \
  --startup-project API/Host/Host.csproj
```
