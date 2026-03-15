## install
- UniTask
- Inference Engine
  - Package Manager > Unity Registryにして「Inference Engine」検索によりインストール

## dotnet build

```sh
# プロジェクト配下で実施
dotnet new classlib -n OthelloEnv --no-restore

# OthelloEnv.csproj の <ItemGroup> に使用するcsharpを指定

cd OthelloEnv
dotnet build -c Release
```

## 流れ
- dotnetビルド（C#側の更新があれば）
- train.pyを実行 : C#のEnvSystemを環境として学習してくれる
- export_onnxを実行 : Assets/Modelsにonnxファイル出力
- Unityにて、AIInputSystemにonnxをアタッチ
- Unityプレイ