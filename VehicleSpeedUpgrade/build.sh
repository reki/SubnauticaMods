#!/bin/bash
echo "VehicleSpeedUpgrade Build started:" $(date)
rm -rf bin/ obj/ VehicleSpeedUpgrade/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf VehicleSpeedUpgrade
    mkdir -p VehicleSpeedUpgrade
    cp bin/Release/net472/VehicleSpeedUpgrade.dll VehicleSpeedUpgrade
    cp -r Localization VehicleSpeedUpgrade
    cp -r Assets VehicleSpeedUpgrade
    cp config.json VehicleSpeedUpgrade
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade
        cp -r ./VehicleSpeedUpgrade $SUBNAUTICA_HOME/BepInEx/plugins/VehicleSpeedUpgrade
    fi
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi