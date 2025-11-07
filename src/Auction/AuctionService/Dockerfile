FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY CarsAuction.sln ./
COPY src/Auction/AuctionService/AuctionService.csproj ./src/Auction/AuctionService/
COPY src/Auction/Domain/Domain.csproj ./src/Auction/Domain/
COPY src/Auction/Application/Application.csproj ./src/Auction/Application/
COPY src/Auction/Infraestructure/Infraestructure.csproj ./src/Auction/Infraestructure/
COPY src/Auction/Utilities/Utilities.csproj ./src/Auction/Utilities/
COPY src/Contracts/Contracts.csproj ./src/Contracts/


RUN dotnet restore ./src/Auction/AuctionService/AuctionService.csproj

COPY . .
RUN dotnet publish ./src/Auction/AuctionService/AuctionService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "AuctionService.dll" ]