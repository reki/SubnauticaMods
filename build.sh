#!/bin/bash
cd $(dirname $0)

rm -rf ./Releases
mkdir -p ./Releases

cd ./SeamothDepthUpgrade
./build.sh

if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./SeamothDepthUpgrade ./../Releases
fi

echo -e "\n================================================\n"

cd ..
cd ./TeleportVehicle
./build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./TeleportVehicle ./../Releases
fi

echo -e "\n================================================\n"

cd ..
cd ./UraninitePowerCell
./build.sh
if [ $? -eq 0 ]; then
    mkdir -p ./../Releases
    cp -r ./UraninitePowerCell ./../Releases
fi
