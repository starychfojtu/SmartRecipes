#!/bin/bash
sudo dotnet publish -c Release -o /var/smart-recipes/publish /var/smart-recipes/src/SmartRecipes.Api/src/SmartRecipes.Api/SmartRecipes.Api.fsproj
sudo dotnet /var/smart-recipes/publish/SmartRecipes.Api.dll
sudo service httpd restart > /var/log/restartapache.out 2>&1

