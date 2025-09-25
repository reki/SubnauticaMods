#!/bin/bash
echo "VehicleSpeedUpgrade Build started:" $(date)

# Parse command line arguments
KEEP_CONFIG=false
for arg in "$@"; do
    case $arg in
        --keep-config)
            KEEP_CONFIG=true
            shift
            ;;
        *)
            ;;
    esac
done

rm -rf bin/ obj/ VehicleSpeedUpgrade/Debug/
dotnet build -c Debug -p:Configuration=March2023
if [ $? -eq 0 ]; then
    rm -rf VehicleSpeedUpgrade
    mkdir -p VehicleSpeedUpgrade
    cp bin/Debug/net472/VehicleSpeedUpgrade.dll VehicleSpeedUpgrade
    cp -r Localization VehicleSpeedUpgrade
    cp -r Assets VehicleSpeedUpgrade
    cp config.json VehicleSpeedUpgrade
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        # Backup existing config if --keep-config is specified
        if [ "$KEEP_CONFIG" = true ] && [ -f "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade/config.json" ]; then
            cp "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade/config.json" "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade_config_backup.json"
        fi
        
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade
        cp -r ./VehicleSpeedUpgrade $SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade
        
        # Restore backed up config if --keep-config is specified
        if [ "$KEEP_CONFIG" = true ] && [ -f "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade_config_backup.json" ]; then
            cp "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade_config_backup.json" "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade/config.json"
            rm "$SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade_config_backup.json"
        fi
    fi
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi