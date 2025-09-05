#!/bin/bash
echo "TeleportVehicle Build started:" $(date)
rm -rf bin/ obj/ TeleportVehicle/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf TeleportVehicle
    mkdir -p TeleportVehicle
    cp bin/Release/net472/TeleportVehicle.dll TeleportVehicle
    cp -r Localization TeleportVehicle/
    echo "Build successful!"
else
    echo "Build failed!"
    exit 1
fi