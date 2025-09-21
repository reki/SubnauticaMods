## TeleportVehicle

[日本語はこちら](./README.ja.md)

### Overview
Provides a Vehicle Teleporter tool that allows you to select and teleport vehicles to your location. Perfect for recovering lost or distant vehicles without the need to travel long distances.

### Features
- **Vehicle Teleporter Tool**: Craftable handheld device using Scanner components
- **Right-Click Selection**: Cycle through available vehicles (Seamoth and Exosuit)
- **Left-Click Teleport**: Teleport the selected vehicle to your location
- **Smart Detection**: Only shows undocked, deployable vehicles
- **Visual/Audio Effects**: Warper-style teleportation effects with sound
- **Safety Checks**: Prevents teleporting docked vehicles

### Recipe
**Vehicle Teleporter:**
- Scanner x1
- Diamond x2
- Lithium x2
- Power Cell x2

### Usage
1. Craft the Vehicle Teleporter from the Fabricator (Personal > Tools)
2. Equip the tool in your hand
3. **Right-click** to cycle through available vehicles
4. **Left-click** to teleport the selected vehicle to your location
5. The vehicle will appear 3 meters in front of you, 1 meter above

### Requirements
- BepInEx 5.x for Subnautica
- Nautilus
- Subnautica (PC)

### Install
Copy `TeleportVehicle.dll` and the `Localization` folder into `Subnautica/BepInEx/plugins/TeleportVehicle/`.

### Build
```bash
./build.sh
```
Output: `bin/Release/net472/TeleportVehicle.dll`.


