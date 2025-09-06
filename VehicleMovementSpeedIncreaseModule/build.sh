#!/bin/bash
echo "VehicleMovementSpeedIncreaseModule Build started:" $(date)
rm -rf bin/ obj/ VehicleMovementSpeedIncreaseModule/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf VehicleMovementSpeedIncreaseModule
    mkdir -p VehicleMovementSpeedIncreaseModule
    cp bin/Release/net472/VehicleMovementSpeedIncreaseModule.dll VehicleMovementSpeedIncreaseModule
    cp -r Localization VehicleMovementSpeedIncreaseModule
    cp -r Assets VehicleMovementSpeedIncreaseModule
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/VehicleMovementSpeedIncreaseModule
        cp -r ./VehicleMovementSpeedIncreaseModule $SUBNAUTICA_HOME/BepInEx/plugins/VehicleMovementSpeedIncreaseModule
    fi
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi