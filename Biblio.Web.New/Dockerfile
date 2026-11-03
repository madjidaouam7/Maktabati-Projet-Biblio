
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore Biblio.Web.New/Biblio.Web.New.csproj

RUN dotnet publish Biblio.Web.New/Biblio.Web.New.csproj \
    -c Release \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "exec dotnet Biblio.Web.New.dll --urls http://0.0.0.0:${PORT:-10000}"]
