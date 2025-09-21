## SeamothDepthUpgrade

[日本語はこちら](./README.ja.md)

### Overview
Adds MK4 and MK5 depth upgrade modules for the Seamoth, extending exploration capabilities to extreme depths. These modules allow deeper exploration than the standard MK3 module.

### Features
- **Seamoth Depth Module MK4**: Allows diving to 1300m depth (upgraded from MK3's 900m)
- **Seamoth Depth Module MK5**: Allows diving to 1700m depth (maximum depth capability)
- **Progressive Crafting**: MK4 requires MK3, MK5 requires MK4
- **Workbench Crafting**: Both modules are crafted at the Modification Station
- **Scan-to-Unlock**: MK4 unlocks by scanning MK3, MK5 unlocks by scanning MK4

### Recipes
**MK4 Depth Module:**
- Vehicle Hull Module MK3 x1
- Nickel Ore x2
- Plasteel Ingot x1

**MK5 Depth Module:**
- Seamoth Depth Module MK4 x1
- Kyanite x2
- Plasteel Ingot x1

### Requirements
- BepInEx 5.x for Subnautica
- Nautilus
- Subnautica (PC)

### Install
Copy `SeamothDepthUpgrade.dll` and the `Localization` folder into `Subnautica/BepInEx/plugins/SeamothDepthUpgrade/`.

### Build
```bash
./build.sh
```
Output: `bin/Release/net472/SeamothDepthUpgrade.dll`.


