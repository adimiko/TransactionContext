#!/bin/bash
echo Executing after success scripts on branch $GITHUB_REF_NAME
echo Triggering Nuget package build

cd source/$1
dotnet pack -c release /p:PackageVersion=0.0.$GITHUB_RUN_NUMBER --no-restore -o .

echo Uploading $1 package to Nuget using branch $GITHUB_REF_NAME

case "$GITHUB_REF_NAME" in
  "main")
    dotnet nuget push *.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json
    ;;
esac