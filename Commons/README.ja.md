## Commons ライブラリ

[English is here](./README.md)

### 役割
`Commons` は本リポジトリ内の Subnautica 向け Mod が共通で利用するヘルパーライブラリです。ローカライズ機能（`LocalizationManager.cs`）などを提供します。

### 使い方
- 各 Mod プロジェクトから `Commons` へプロジェクト参照を追加します。
- 各 Mod の `Localization/English.json` と `Localization/Japanese.json` に翻訳を配置します。
- 実行時にローカライズ済み文字列を取得する際は、提供されるローカライズマネージャを利用します。

### ビルド
```bash
dotnet build -c Release
```
出力：`Commons/bin/Release/net472/`


