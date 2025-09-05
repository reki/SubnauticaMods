#!/bin/bash
cd $(dirname $0)

rm -rf ./Releases
mkdir -p ./Releases

cd ./SeamothDepthUpgrade
./build.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./SeamothDepthUpgrade ./../Releases
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/SeamothDepthUpgrade
        cp -r ./SeamothDepthUpgrade $SUBNAUTICA_HOME/BepInEx/plugins/SeamothDepthUpgrade
    fi
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./TeleportVehicle ./../Releases
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/TeleportVehicle
        cp -r ./TeleportVehicle $SUBNAUTICA_HOME/BepInEx/plugins/TeleportVehicle
    fi
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./UraninitePowerCell ./../Releases
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/UraninitePowerCell
        cp -r ./UraninitePowerCell $SUBNAUTICA_HOME/BepInEx/plugins/UraninitePowerCell
    fi
fi
