#STAGE 1 - BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS BUILD
WORKDIR /src

COPY APILibrary.Data/APILibrary.Data.csproj APILibrary.Data/
COPY APILibrary.Services/APILibrary.Services.csproj APILibrary.Services/
COPY SmartLedgerAPI/SmartLedgerAPI.csproj SmartLedgerAPI/

RUN dotnet restore "SmartLedgerAPI/SmartLedgerAPI.csproj" #READS csproj FILES AND DOWNLOAD ALL NUGET PACKAGES

COPY ..

RUN dotnet publish "SmartLedgerAPI/SmartLedgerAPI.csproj" -c Release -o /app/publish



#STAGE 2 - RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0  #runs app no sdk no compiler
WORKDIR /app

COPY --from=Build /app/publish . 

RUN adduser --disabled-pasword --gecos"" appuser
USER appuser

EXPOSE 80

ENTRYPOINT ["dotnet", "SmartLedgerAPI.dll"]



