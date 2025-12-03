FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copiar csproj
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
# Si tu Gateway NO usa shared/contracts, no hace falta copiarlos aqui.

# 2. Restaurar
RUN dotnet restore "ApiGateway/ApiGateway.csproj"

# 3. Copiar todo y compilar
COPY . .
WORKDIR "/src/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagen Final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]