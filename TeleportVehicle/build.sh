#!/bin/bash

# TeleportVehicle モッドのビルドスクリプト

echo "TeleportVehicle Build started:" $(date)

# プロジェクトをビルド
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo "Build successful!"
    
    # ビルド成果物をコピー
    rm -rf TeleportVehicle
    mkdir -p TeleportVehicle
    cp bin/Release/net472/TeleportVehicle.dll TeleportVehicle/
    cp -r Localization TeleportVehicle/
    
    echo "Files copied to TeleportVehicle directory"
    echo "Mod is ready for installation!"
else
    echo "Build failed!"
    exit 1
fi