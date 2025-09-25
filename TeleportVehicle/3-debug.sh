#!/bin/bash
echo "TeleportVehicle Build started:" $(date)
rm -rf bin/ obj/ TeleportVehicle/Debug
dotnet build -c Debug -p:Configuration=March2023
if [ $? -eq 0 ]; then
    rm -rf TeleportVehicle
    mkdir -p TeleportVehicle
    cp bin/Debug/net472/TeleportVehicle.dll TeleportVehicle
    cp -r Localization TeleportVehicle/
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/TeleportVehicle
        cp -r ./TeleportVehicle $SUBNAUTICA_HOME/BepInEx/plugins/TeleportVehicle
    fi
    echo "Build successful!"
else
    echo "Build failed!"
    exit 1
fi