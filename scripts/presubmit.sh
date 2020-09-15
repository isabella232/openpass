#!/bin/sh

if [ "$1" = "end" ]; then
    # Run Javascript UTests
    pushd ./Criteo.IdController/Javascript
    npm install
    npm test
    popd
fi
