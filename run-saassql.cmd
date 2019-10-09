docker run -d -p 1401:1433 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=@MyPassw0rd" --name saas-sql microsoft/mssql-server-windows-express

rem dotnet tool install dotnet-ef
dotnet ef database update
