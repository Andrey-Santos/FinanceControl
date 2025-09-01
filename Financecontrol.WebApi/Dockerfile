# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copia tudo (volta para raiz da solução)
COPY .. .

# restaura a solução completa
RUN dotnet restore FinanceControl.sln

# publica o WebApi (corrigido o caminho)
RUN dotnet publish FinanceControl.WebApi.csproj -c Release -o /app -r linux-x64 --no-self-contained

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FinanceControl.WebApi.dll"]
