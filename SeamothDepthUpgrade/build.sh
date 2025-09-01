#!/bin/bash
echo "SeamothDepthUpgrade Build started:" $(date)
rm -rf bin/ obj/ SeamothDepthUpgrade/
dotnet build -c Release
if [ $? -eq 0 ]; then
    rm -rf SeamothDepthUpgrade
    mkdir -p SeamothDepthUpgrade
    cp bin/Release/net472/SeamothDepthUpgrade.dll SeamothDepthUpgrade
    cp -r Localization SeamothDepthUpgrade
    echo "Build succeeded."
else
    echo "Build failed."
    exit 1
fi