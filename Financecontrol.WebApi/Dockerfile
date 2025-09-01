# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copia tudo (volta para raiz do projeto)
COPY .. .

# restaura a solução completa
RUN dotnet restore FinanceControl.sln

# publica o WebApi
RUN dotnet publish FinanceControl.WebApi/FinanceControl.WebApi.csproj -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "FinanceControl.WebApi.dll"]
