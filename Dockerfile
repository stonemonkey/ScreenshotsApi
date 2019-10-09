FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./screenshotsapi/
RUN dotnet restore ./screenshotsapi/

# copy everything else and build app
COPY . ./screenshotsapi/
WORKDIR /app/screenshotsapi
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/screenshotsapi/out ./
ENTRYPOINT ["dotnet", "screenshots.dll"]