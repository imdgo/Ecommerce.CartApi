# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia csproj e restaura
COPY src/CartApi/CartApi.csproj ./CartApi.csproj
RUN dotnet restore CartApi.csproj

# Copia todo o projeto e publica
COPY src/CartApi/. ./
RUN dotnet publish CartApi.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CartApi.dll"]
