## VehicleSpeedUpgrade

[日本語はこちら](./README.ja.md)

### Overview
A comprehensive vehicle speed enhancement mod that adds universal speed upgrade modules for all Subnautica vehicles with configurable multipliers and battery consumption.

### Features

This mod adds four speed upgrade modules:

- **Vehicle Speed Upgrade MK1**: For Seamoth and Exosuit (configurable multiplier, default 2.0x)
- **Vehicle Speed Upgrade MK2**: For Seamoth and Exosuit (configurable multiplier, default 2.5x)
- **Cyclops Speed Upgrade MK1**: For Cyclops submarine (configurable per engine mode)
- **Cyclops Speed Upgrade MK2**: For Cyclops submarine (higher performance)

### Key Features

- **Universal Support**: Compatible with Seamoth, Exosuit (Prawn Suit), and Cyclops
- **Configurable Multipliers**: All speed values can be adjusted via JSON configuration
- **Battery Consumption**: Optional energy consumption system for balanced gameplay
- **Engine Mode Aware**: Cyclops modules adjust based on engine speed (Slow/Standard/Fast)
- **Damage Defense**: Optional enhanced damage resistance when speed modules are active

### Recipes

**Vehicle Speed Upgrade MK1:**
- Titanium x2
- Quartz x1

**Vehicle Speed Upgrade MK2:**
- Vehicle Speed Upgrade MK1 x1
- Titanium x2
- Quartz x2
- Nickel x1
- Diamond x1

**Cyclops Speed Upgrade MK1:**
- Titanium x3
- Quartz x2
- Copper x2

**Cyclops Speed Upgrade MK2:**
- Cyclops Speed Upgrade MK1 x1
- Titanium x3
- Quartz x3
- Nickel x2
- Diamond x1

### Crafting Locations
- **MK1**: Vehicle Upgrade Console (SeamothUpgrades > CommonModules)
- **MK2**: Modification Station (Workbench)
- **Cyclops MK1**: Cyclops Fabricator (Modules)
- **Cyclops MK2**: Modification Station (Workbench)

### Configuration
The mod includes a `config.json` file with extensive customization options:
- **Speed Multipliers**: Adjust force multipliers for all vehicles and directions
- **Energy Consumption**: Configure battery drain rates for each module
- **Debug Options**: Enable detailed logging for troubleshooting
- **Damage Defense**: Toggle enhanced damage resistance

### Usage Notes
- Install modules in your vehicle's upgrade slots
- Only one speed module per vehicle (higher tier overrides lower)
- Cyclops modules adjust performance based on current engine mode
- Energy consumption scales with movement (configurable)
- All settings can be modified in the config file

### Requirements
- BepInEx 5.x for Subnautica
- Nautilus
- Subnautica (PC)

### Install
Copy `VehicleSpeedUpgrade.dll` and the `Localization` folder into `Subnautica/BepInEx/plugins/VehicleSpeedUpgrade/`.

### Build
```bash
./build.sh
```
Output: `bin/Release/net472/VehicleSpeedUpgrade.dll`.
