FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copiar csproj del servicio y sus dependencias compartidas
COPY ["routes/routes.csproj", "routes/"]
COPY ["Frock.Contracts/Frock.Contracts.csproj", "Frock.Contracts/"]

# 2. Restaurar
RUN dotnet restore "routes/routes.csproj"

# 3. Copiar todo y compilar
COPY . .
WORKDIR "/src/routes"
RUN dotnet build "routes.csproj" -c Release -o /app/build
RUN dotnet publish "routes.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagen Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "routes.dll"]