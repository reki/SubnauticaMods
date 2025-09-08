#!/bin/bash
echo "UraninitePowerCell Build started" $(date)
rm -rf bin/ obj/ UraninitePowerCell/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf UraninitePowerCell
    mkdir -p UraninitePowerCell
    cp bin/Release/net472/UraninitePowerCell.dll UraninitePowerCell
    cp -r Localization UraninitePowerCell
    cp -r Assets UraninitePowerCell
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/UraninitePowerCell
        cp -r ./UraninitePowerCell $SUBNAUTICA_HOME/BepInEx/plugins/UraninitePowerCell
    fi
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi
