#!/bin/bash
echo "SeamothDepthUpgrade Build started:" $(date)
rm -rf bin/ obj/ SeamothDepthUpgrade/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf SeamothDepthUpgrade
    mkdir -p SeamothDepthUpgrade
    cp bin/Release/net472/SeamothDepthUpgrade.dll SeamothDepthUpgrade
    cp -r Localization SeamothDepthUpgrade
    if [ -d $SUBNAUTICA_HOME/BepInEx/plugins ]; then
        rm -rf $SUBNAUTICA_HOME/BepInEx/plugins/SeamothDepthUpgrade
        cp -r ./SeamothDepthUpgrade $SUBNAUTICA_HOME/BepInEx/plugins/SeamothDepthUpgrade
    fi
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi