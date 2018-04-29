#!/usr/bin/env bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
NUGET_DIR=$SCRIPT_DIR/scripts/nuget
SIMPLE_TARGETS_DIR=$NUGET_DIR/simple-targets-csx

if [ ! -d "$SIMPLE_TARGETS_DIR" ]; then
  dotnet restore scripts/build/build.csproj --packages "$SIMPLE_TARGETS_DIR"
fi

dotnet script $SCRIPT_DIR/build.csx -- $*