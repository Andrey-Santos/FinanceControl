# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solution e restaura dependências
COPY ["FinanceControl.sln", "./"]
COPY ["FinanceControl.Core/FinanceControl.Core.csproj", "FinanceControl.Core/"]
COPY ["FinanceControl.Infrastructure/FinanceControl.Infrastructure.csproj", "FinanceControl.Infrastructure/"]
COPY ["FinanceControl.WebApi/FinanceControl.WebApi.csproj", "FinanceControl.WebApi/"]

RUN dotnet restore "FinanceControl.WebApi/FinanceControl.WebApi.csproj"

# Copia o restante do código
COPY . .

WORKDIR "/src/FinanceControl.WebApi"
RUN dotnet publish "FinanceControl.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render expõe a porta 8085
ENV ASPNETCORE_URLS=http://+:8085
EXPOSE 8085

ENTRYPOINT ["dotnet", "FinanceControl.WebApi.dll"]
