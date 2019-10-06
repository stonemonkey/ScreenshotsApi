docker run -d ^
    -p 1401:1433 ^
	-e "ACCEPT_EULA=Y" -e "SA_PASSWORD=@MyPassw0rd" ^
   --name saas-sql mcr.microsoft.com/mssql/server:2017-latest

dotnet ef database update

docker run -d ^
    -p 4999:80 ^
	--name saas-api	saas