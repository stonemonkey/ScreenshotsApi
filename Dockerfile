FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
# libraries for wkhttptopdf and IronPDF
RUN ["apt-get", "update"]
RUN ["apt-get", "-y", "install", "libgdiplus"]
RUN ["apt-get", "-y", "install", "xvfb", "libfontconfig", "wkhtmltopdf"]
RUN ["apt-get", "-y", "install", "libc6-dev"]
RUN ["apt-get", "-y", "install", "openssl"]
RUN ["apt-get", "-y", "install", "libssl-dev"]
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
RUN chmod 755 /app/Wkhtmltopdf/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "screenshots.dll"]