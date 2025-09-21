#!/bin/bash
cd $(dirname $0)

rm -rf ./Releases
mkdir -p ./Releases

cd ./Commons
./clean.sh

cd ..
cd ./SeamothDepthUpgrade
./clean.sh

cd ..
cd ./TeleportVehicle
./clean.sh

cd ..
cd ./UraninitePowerCell
./clean.sh

cd ..
cd ./VehicleSpeedUpgrade
./clean.sh

echo clean completed