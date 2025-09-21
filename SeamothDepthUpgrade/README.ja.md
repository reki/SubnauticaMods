## SeamothDepthUpgrade

[English is here](./README.md)

### 概要
シーモス用の MK4・MK5 深度アップグレードモジュールを追加し、極深度での探索能力を拡張します。標準の MK3 モジュールを超える深度での探索が可能になります。

### 機能
- **シーモス深度モジュール MK4**: 1300m深度まで潜水可能（MK3の900mから向上）
- **シーモス深度モジュール MK5**: 1700m深度まで潜水可能（最大深度能力）
- **段階的クラフト**: MK4にはMK3が必要、MK5にはMK4が必要
- **改造ステーションでのクラフト**: 両モジュールとも改造ステーションで作成
- **スキャンでアンロック**: MK3をスキャンしてMK4をアンロック、MK4をスキャンしてMK5をアンロック

### レシピ
**MK4 深度モジュール:**
- ビークル船体モジュール MK3 x1
- ニッケル鉱石 x2
- プラスチール・インゴット x1

**MK5 深度モジュール:**
- シーモス深度モジュール MK4 x1
- カイアナイト x2
- プラスチール・インゴット x1

### 必要環境
- BepInEx 5.x（Subnautica 用）
- Nautilus
- Subnautica（PC 版）

### 導入
`SeamothDepthUpgrade.dll` と `Localization` フォルダを `Subnautica/BepInEx/plugins/SeamothDepthUpgrade/` に配置します。

### ビルド
```bash
./build.sh
```
出力：`bin/Release/net472/SeamothDepthUpgrade.dll`


