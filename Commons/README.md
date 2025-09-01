## Commons Library

[日本語はこちら](./README.ja.md)

### Role
`Commons` is a shared helper library for the Subnautica mods in this repository. It provides localization utilities (see `LocalizationManager.cs`) and may include other helpers.

### Usage
- Add a project reference from each mod project to `Commons`.
- Put translation files under each mod’s `Localization/English.json` and `Localization/Japanese.json`.
- Use the localization manager to fetch localized strings at runtime.

### Build
```bash
dotnet build -c Release
```
Output: `Commons/bin/Release/net472/`.


