FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY CarsAuction.sln ./
COPY src/Gateway/GatewayService/GatewayService.csproj ./src/Gateway/GatewayService/

RUN dotnet restore ./src/Gateway/GatewayService/GatewayService.csproj

COPY . .
RUN dotnet publish ./src/Gateway/GatewayService/GatewayService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.dll"]