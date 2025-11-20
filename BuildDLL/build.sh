#!/bin/bash

# Build script for HuynnSDK DLLs
# This script compiles the Runtime and Editor DLLs for Unity

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Get the directory where the script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo -e "${YELLOW}Starting HuynnSDK DLL build...${NC}"
echo -e "${YELLOW}Working directory: ${SCRIPT_DIR}${NC}"

# Check if Unity is installed
UNITY_VERSION="2022.3.62f2"
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/Managed"

if [ ! -d "$UNITY_PATH" ]; then
    echo -e "${RED}Unity version ${UNITY_VERSION} not found!${NC}"
    echo -e "${YELLOW}Please update the Unity version in the .csproj files to match your installation.${NC}"
    echo -e "${YELLOW}You can find your Unity installation at: /Applications/Unity/Hub/Editor/YOUR_VERSION${NC}"
    exit 1
fi

# Create output directories
mkdir -p ../Assets/Plugins
mkdir -p ../Assets/Plugins/Editor

# Build Runtime DLL
echo -e "${YELLOW}Building HuynnSDK.Runtime.dll...${NC}"
dotnet build HuynnSDK.Runtime.csproj -c Release -o ./output

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Runtime DLL built successfully!${NC}"
    cp ./output/HuynnSDK.Runtime.dll ../Assets/Plugins/
else
    echo -e "${RED}✗ Failed to build Runtime DLL${NC}"
    exit 1
fi

# Build Editor DLL
echo -e "${YELLOW}Building HuynnSDK.Editor.dll...${NC}"
dotnet build HuynnSDK.Editor.csproj -c Release -o ./output

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Editor DLL built successfully!${NC}"
    cp ./output/HuynnSDK.Editor.dll ../Assets/Plugins/Editor/
else
    echo -e "${RED}✗ Failed to build Editor DLL${NC}"
    exit 1
fi

# Clean up
echo -e "${YELLOW}Cleaning up build artifacts...${NC}"
rm -rf ./output

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Build completed successfully!${NC}"
echo -e "${GREEN}========================================${NC}"
echo -e "Runtime DLL: ${GREEN}Assets/Plugins/HuynnSDK.Runtime.dll${NC}"
echo -e "Editor DLL:  ${GREEN}Assets/Plugins/Editor/HuynnSDK.Editor.dll${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Open Unity and let it recompile"
echo "2. You can now remove the original .cs files from Assets/HuynnSDK and Assets/Editor"
echo "3. Keep ThirdLibConfigExample.cs as it demonstrates how to use the SDK"
