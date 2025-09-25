#!/bin/bash
cd $(dirname $0)

rm -rf ./MarchReleases

cd ./SeamothDepthUpgrade
./march-build.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../MarchReleases
    cp -r ./SeamothDepthUpgrade ./../MarchReleases
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchReleases
    cp -r ./TeleportVehicle ./../MarchReleases
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchReleases
    cp -r ./UraninitePowerCell ./../MarchReleases
fi

echo -e "\n================================================\n"

cd ..
cd ./VehicleSpeedUpgrade
./march-build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../MarchReleases
    cp -r ./VehicleSpeedUpgrade ./../MarchReleases
fi
