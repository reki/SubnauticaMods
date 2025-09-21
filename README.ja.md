## Subnautica Mods コレクション

[English is here](./README.md)

### 概要
このリポジトリは Subnautica 向けの BepInEx Mod 集です：
- **SeamothDepthUpgrade**：シーモス用の MK4・MK5 深度アップグレードモジュールを追加し、最大1700mまでの探索を可能にします。
- **TeleportVehicle**：左右クリック操作でビークルを選択・テレポートできるビークルテレポーターツールを提供します。
- **UraninitePowerCell**：閃ウラン鉱から作成される大容量パワーセル・バッテリーを追加し、標準品の50倍の容量を提供します。
- **VehicleSpeedUpgrade**：シーモス・プラウンスーツ・サイクロプス用の汎用速度アップグレードモジュール（MK1・MK2）を追加し、設定可能な倍率とバッテリー消費機能を提供します。

共通ライブラリ `Commons` はローカライゼーション管理やカスタムアイコン読み込みなどの共通機能を提供します。
このプロジェクトのソースコードはAIで生成されたコードも含まれています。

### 必要環境
- Subnautica（PC 版・安定版）
- BepInEx 5.x for Subnautica
- Nautilus（Subnautica 用モッディングフレームワーク）
- .NET Framework 4.7.2 Developer Pack（ビルド用）
- Git Bash などの bash 互換シェル（`build.sh` 実行用）

### 開発環境のセットアップ
1) Visual Studio 2022 以降をインストールし、以下のワークロードを追加：
   - 「.NET デスクトップ開発」
   - 「.NET Framework 4.7.2 Developer Pack」
2) Git と bash 互換シェル（例：Git Bash）をインストール（`build.sh` 実行用）。
3) Subnautica（PC 版）と BepInEx 5.x（Subnautica 用）をテスト用に導入。
4) このリポジトリをクローン：
```bash
git clone https://github.com/reki/SubnauticaMods.git
cd SubnauticaMods
```
5) NuGet パッケージを復元（VS 自動、または以下）：
```bash
dotnet restore
```
6) すべてのプロジェクトをビルド：
```bash
./build.sh
```
7) テストのためにビルド成果物をゲームへ配置（「ソースからビルド」参照）。

### ソースからビルド
一括ビルド：
```bash
./build.sh
```
個別ビルド：
```bash
cd SeamothDepthUpgrade && ./build.sh
cd TeleportVehicle && ./build.sh
cd UraninitePowerCell && ./build.sh
cd VehicleSpeedUpgrade && ./build.sh
```
成果物：各プロジェクトの `bin/Release/net472/`。DLL と `Localization` を `BepInEx/plugins/<ModName>` に配置します。

### Mod の導入（ビルド済みを使用）
1) 各 Mod のビルド済み DLL と `Localization` フォルダを取得します。
2) `Subnautica/BepInEx/plugins/<ModName>/` にコピーします。
3) 依存関係として `Nautilus` が `BepInEx/plugins` に導入済みであることを確認します。
4) ゲームを起動します。

### 開発メモ
- 構成：各 Mod は独立した `csproj`。`Commons` にローカライズ等の共通処理があります。
- ローカライズ：各 Mod の `Localization/English.json` と `Localization/Japanese.json` を利用します。
- ログ：BepInEx の `Logger.Log*` を利用します。


### ライセンス・クレジット
- ライセンス：`LICENSE` があればそれに従います。無い場合は作者の全権利留保とします。
- Subnautica モッディングコミュニティ、BepInEx、Nautilus に感謝します。


