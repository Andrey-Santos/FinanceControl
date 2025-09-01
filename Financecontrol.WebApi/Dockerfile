# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia csproj e restaura dependências
COPY *.sln ./
COPY Financecontrol.WebApi/Financecontrol.WebApi.csproj Financecontrol
