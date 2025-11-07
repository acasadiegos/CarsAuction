FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY CarsAuction.sln ./
COPY src/Search/SearchService/SearchService.csproj ./src/Search/SearchService/
COPY src/Search/Domain/Domain.csproj ./src/Search/Domain/
COPY src/Search/Application/Application.csproj ./src/Search/Application/
COPY src/Search/Infraestructure/Infraestructure.csproj ./src/Search/Infraestructure/
COPY src/Search/Utilities/Utilities.csproj ./src/Search/Utilities/
COPY src/Contracts/Contracts.csproj ./src/Contracts/

RUN dotnet restore ./src/Search/SearchService/SearchService.csproj

COPY . .
RUN dotnet publish ./src/Search/SearchService/SearchService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SearchService.dll"]