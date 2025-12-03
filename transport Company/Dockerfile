FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copiar csproj (OJO con el espacio en el nombre de la carpeta)
COPY ["transport Company/transport Company.csproj", "transport Company/"]
COPY ["Frock.Contracts/Frock.Contracts.csproj", "Frock.Contracts/"]

# 2. Restaurar
RUN dotnet restore "transport Company/transport Company.csproj"

# 3. Copiar todo y compilar
COPY . .
WORKDIR "/src/transport Company"
RUN dotnet build "transport Company.csproj" -c Release -o /app/build
RUN dotnet publish "transport Company.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagen Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "transport Company.dll"]