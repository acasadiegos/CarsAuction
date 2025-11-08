FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

RUN apt-get update && apt-get install -y curl
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs

COPY CarsAuction.sln ./
COPY src/IdentityServer/IdentityServer.csproj ./src/IdentityServer/

RUN dotnet restore ./src/IdentityServer/IdentityServer.csproj

COPY . .

WORKDIR /app/src/IdentityServer
RUN npm ci
RUN npm run build

WORKDIR /app
RUN dotnet publish ./src/IdentityServer/IdentityServer.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
