#!/usr/bin/env bash

# Usage:
# 0. Create an API key at https://www.nuget.org/account/apikeys and store it in ~/nuget.org.key
# 1. Increment the package version number in source/LiteMigrator/Xeno.LiteMigrator.csproj
# 2. Run `dotnet build source` or `dotnet pack source`  (where 'source' is the folder)
# 3. Run this script to publish


NUGET_PKG=$1

echo "Publishing package, $NUGET_PKG"
dotnet nuget push -s https://api.nuget.org/v3/index.json -k `cat ~/nuget.org.key` $NUGET_PKG
