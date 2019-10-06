docker stop saas-api
docker rm saas-api

docker stop saas-sql
docker rm saas-sql

dotnet tool uninstall dotnet-ef
docker rmi saas

