# Estágio 1: Build da API .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia todo o diretório do backend para o container
COPY src/banckend/ ./banckend/

# Entra na pasta do projeto C#
WORKDIR /src/banckend/KylyApi

# Restaura as dependências e compila em modo Release
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY sample_db/ /app/sample_db/

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "KylyApi.dll"]