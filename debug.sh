#!/bin/bash
cd $(dirname $0)

rm -rf ./Debugs

cd ./SeamothDepthUpgrade
./debug.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../Debugs
    cp -r ./SeamothDepthUpgrade ./../Debugs
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Debugs
    cp -r ./TeleportVehicle ./../Debugs
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Debugs
    cp -r ./UraninitePowerCell ./../Debugs
fi

echo -e "\n================================================\n"

cd ..
cd ./VehicleSpeedUpgrade
./debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Debugs
    cp -r ./VehicleSpeedUpgrade ./../Debugs
fi
