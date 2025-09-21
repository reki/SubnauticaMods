## Commons ライブラリ

[English is here](./README.md)

### 役割
`Commons` は本リポジトリ内の Subnautica 向け Mod が共通で利用するヘルパーライブラリです。以下の重要なユーティリティを提供します：
- **LocalizationManager**: 階層的なローカライゼーションシステム（多言語対応、英語へのフォールバック）
- **ConfigTemplate**: バリデーション関数付きのJSON設定管理
- **Funcs**: 設定値の共通バリデーション関数
- **Commons**: UI状態検出やカスタムアイコン読み込みのユーティリティ関数

### 機能
- **システムUI検出**: `IsSystemUiOpen()` でポーズ・設定・Modメニューがアクティブかを検出
- **カスタムアイコン読み込み**: `LoadCustomIcon()` でModディレクトリからカスタムスプライトを読み込み
- **階層的ローカライゼーション**: ネストしたJSONキーをサポート（例: "VehicleTeleporter.Messages.Success"）
- **変数置換**: ローカライズ文字列でのプレースホルダー置換をサポート

### 使い方
- 各 Mod プロジェクトから `Commons` へプロジェクト参照を追加します。
- 各 Mod の `Localization/english.json` と `Localization/japanese.json` に翻訳を配置します。
- `LocalizationManager.GetLocalizedString()` を使用してローカライズ文字列を取得（オプションで変数置換も可能）。
- `Commons.IsSystemUiOpen()` を使用して入力処理前にシステムUIがアクティブかをチェック。
- `Commons.LoadCustomIcon()` を使用してModのAssetsディレクトリからカスタムスプライトを読み込み。

### ビルド
```bash
dotnet build -c Release
```
出力：`Commons/bin/Release/net472/`


