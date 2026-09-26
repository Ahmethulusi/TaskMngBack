FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore ayrı katman: sadece csproj değişince paketler yeniden indirilir
COPY TaskMngBack.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish TaskMngBack.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Railway PORT değişkenini çalışma zamanında verir; shell form ile genişletilir
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet TaskMngBack.dll"]
