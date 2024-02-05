#!/bin/bash

chmod +x ./scripts/publish-package.sh

for dir in source/*/
do
    dir=${dir%*/}
    echo Publishing NuGet package:  ${dir##*/}
    
    exec ./scripts/publish-package.sh ${dir##*/} &
    wait
done

echo Finished publishing NuGet packages.