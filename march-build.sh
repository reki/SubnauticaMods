#!/bin/bash
cd $(dirname $0)

rm -rf ./Releases
mkdir -p ./Releases

cd ./SeamothDepthUpgrade
./march-build.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./SeamothDepthUpgrade ./../Releases
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./TeleportVehicle ./../Releases
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./UraninitePowerCell ./../Releases
fi

echo -e "\n================================================\n"

cd ..
cd ./VehicleSpeedUpgrade
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./VehicleSpeedUpgrade ./../Releases
fi
