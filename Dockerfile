#STAGE 1 - BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS Build
WORKDIR /src

COPY APILibrary.Data/APILibrary.Data.csproj APILibrary.Data/
COPY APILibrary.Services/APILibrary.Services.csproj APILibrary.Services/
COPY SmartLedgerAPI/SmartLedgerAPI.csproj SmartLedgerAPI/

RUN dotnet restore "SmartLedgerAPI/SmartLedgerAPI.csproj" #READS csproj FILES AND DOWNLOAD ALL NUGET PACKAGES

#COPY APILibrary.Data/APILibrary.Data/
#COPY APILibrary.Services/APILibrary.Services/
#COPY SmartLedgerAPI/SmartLedgerAPI/
COPY . .

RUN dotnet publish "SmartLedgerAPI/SmartLedgerAPI.csproj" -c Release -o /app/publish



#STAGE 2 - RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0  #runs app no sdk no compiler
WORKDIR /app

#install curl
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

COPY --from=Build /app/publish . 

RUN adduser --disabled-password --gecos "" appuser

#log
RUN mkdir -p /app/Logs && chown -R appuser:appuser /app

USER appuser

EXPOSE 80

#health check 
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "SmartLedgerAPI.dll"]



