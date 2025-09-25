#!/bin/bash
cd $(dirname $0)

rm -rf ./MarchDebugs

cd ./SeamothDepthUpgrade
./3-debug.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../MarchDebugs
    cp -r ./SeamothDepthUpgrade ./../MarchDebugs
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./3-debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchDebugs
    cp -r ./TeleportVehicle ./../MarchDebugs
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./3-debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchDebugs
    cp -r ./UraninitePowerCell ./../MarchDebugs
fi

echo -e "\n================================================\n"

cd ..
cd ./VehicleSpeedUpgrade
./3-debug.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchDebugs
    cp -r ./VehicleSpeedUpgrade ./../MarchDebugs
fi
