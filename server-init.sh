#!/bin/bash
sudo dotnet publish -c Release -o /var/smart-recipes/publish
sudo dotnet /var/smart-recipes/publish/SmartRecipes.Api.dll
sudo service httpd restart > /var/log/restartapache.out 2>&1

