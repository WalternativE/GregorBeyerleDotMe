#!/bin/bash

dotnet fornax clean
dotnet fornax build

rm -rf ./docs
mv ./_public ./docs
