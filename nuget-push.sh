#!/usr/bin/env bash

NUGET_PKG=$1

echo "Publishing package, $NUGET_PKG"
dotnet nuget push -s https://api.nuget.org/v3/index.json -k `cat ~/nuget.org.key` $NUGET_PKG
