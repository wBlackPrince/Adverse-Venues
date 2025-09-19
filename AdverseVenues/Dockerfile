FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AdverseVenues.csproj .
RUN dotnet restore AdverseVenues.csproj

COPY . .
RUN dotnet publish AdverseVenues.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AdverseVenues.dll"]
