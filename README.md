## build

```sh
# Assets に移動して実施
dotnet new classlib -n OthelloEnv --no-restore

# OthelloEnv.csproj の <ItemGroup> を修正

cd OthelloEnv
dotnet build -c Release
# dotnet build OthelloEnv.csproj -c Release
```