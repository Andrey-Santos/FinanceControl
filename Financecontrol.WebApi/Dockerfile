# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia tudo para dentro do container
COPY . .

# Restaura dependências
RUN dotnet restore FinanceControl.sln

# Publica apenas o WebApi
RUN dotnet publish Financecontrol.WebApi/Financecontrol.WebApi.csproj -c Release -o /app -r linux-x64 --no-self-contained

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FinanceControl.WebApi.dll"]
