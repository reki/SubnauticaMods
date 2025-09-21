## Commons Library

[日本語はこちら](./README.ja.md)

### Role
`Commons` is a shared helper library for the Subnautica mods in this repository. It provides essential utilities including:
- **LocalizationManager**: Hierarchical localization system supporting multiple languages with fallback to English
- **ConfigTemplate**: JSON-based configuration management with validation functions
- **Funcs**: Common validation functions for configuration values
- **Commons**: Utility functions for UI state detection and custom icon loading

### Features
- **System UI Detection**: `IsSystemUiOpen()` detects when pause/settings/mods menus are active
- **Custom Icon Loading**: `LoadCustomIcon()` loads custom sprites from mod directories
- **Hierarchical Localization**: Supports nested JSON keys (e.g., "VehicleTeleporter.Messages.Success")
- **Variable Substitution**: Supports placeholder replacement in localized strings

### Usage
- Add a project reference from each mod project to `Commons`.
- Put translation files under each mod's `Localization/english.json` and `Localization/japanese.json`.
- Use `LocalizationManager.GetLocalizedString()` to fetch localized strings with optional variable substitution.
- Use `Commons.IsSystemUiOpen()` to check if system UI is active before processing input.
- Use `Commons.LoadCustomIcon()` to load custom sprites from the mod's Assets directory.

### Build
```bash
dotnet build -c Release
```
Output: `Commons/bin/Release/net472/`.


