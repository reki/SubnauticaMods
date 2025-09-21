## Subnautica Mods Collection

[日本語はこちら](./README.ja.md)

### Overview
This repository contains a collection of BepInEx mods for Subnautica:
- **SeamothDepthUpgrade**: Adds MK4 and MK5 depth upgrade modules for the Seamoth, allowing exploration at depths up to 1700m.
- **TeleportVehicle**: Provides a vehicle teleporter tool that allows you to select and teleport vehicles to your location using left/right click controls.
- **UraninitePowerCell**: Adds high-capacity power cells and batteries crafted from Uraninite Crystal, providing 50x more capacity than standard cells.
- **VehicleSpeedUpgrade**: Adds universal speed upgrade modules (MK1 & MK2) for Seamoth, Exosuit, and Cyclops with configurable multipliers and battery consumption.

The shared library `Commons` provides cross-mod utilities including localization management and custom icon loading.
The source code for this project includes AI-generated code.

### Requirements
- Subnautica (PC, latest stable)
- BepInEx 5.x for Subnautica
- Nautilus (Subnautica modding framework)
- .NET Framework 4.7.2 Developer Pack (for building)
- Git Bash or any bash-compatible shell (for `build.sh`)

### Development environment setup
1) Install Visual Studio 2022 (or newer) with the 
   - ".NET desktop development" workload, and
   - ".NET Framework 4.7.2 Developer Pack".
2) Install Git and a bash-compatible shell (e.g., Git Bash) to run `build.sh`.
3) Install Subnautica (PC) and BepInEx 5.x for Subnautica for testing.
4) Clone this repository:
```bash
git clone https://github.com/reki/SubnauticaMods.git
cd SubnauticaMods
```
5) Restore NuGet packages (VS will do this automatically, or run):
```bash
dotnet restore
```
6) Build all projects:
```bash
./build.sh
```
7) Copy built artifacts to your game for testing (see "Build from source").

### Build from source
Build all:
```bash
./build.sh
```
Build individually:
```bash
cd SeamothDepthUpgrade && ./build.sh
cd TeleportVehicle && ./build.sh
cd UraninitePowerCell && ./build.sh
cd VehicleSpeedUpgrade && ./build.sh
```
Artifacts: `bin/Release/net472/` under each project. Copy the DLL and `Localization` to `BepInEx/plugins/<ModName>`.

### Mod installation (prebuilt)
1) Download each mod's prebuilt DLL and its `Localization` folder from your release distribution.
2) Copy them into `Subnautica/BepInEx/plugins/<ModName>/`.
3) Ensure `Nautilus` is installed under `BepInEx/plugins`.
4) Launch the game.

### Development notes
- Structure: each mod is a separate `csproj`. `Commons` hosts shared helpers like `LocalizationManager`.
- Localization: place strings under `Localization/English.json` and `Localization/Japanese.json` per mod.
- Logging: use standard BepInEx logging (`Logger.Log*`).

### License and credits
- License: See `LICENSE` if present. Otherwise, all rights reserved by the author.
- Thanks to the Subnautica modding community, BepInEx, and Nautilus.


