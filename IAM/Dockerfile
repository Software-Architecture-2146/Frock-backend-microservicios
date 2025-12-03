# 1. IMAGEN BASE (SDK para compilar)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# --- ZONA DE COPIA DE PROYECTOS ---
# Aquí copiamos los .csproj de IAM y de sus dependencias hermanas.
# Esto permite que Docker pueda restaurar los paquetes NuGet antes de copiar todo el código.

COPY ["IAM/IAM.csproj", "IAM/"]
COPY ["Frock.Contracts/Frock.Contracts.csproj", "Frock.Contracts/"]

# ----------------------------------

# 2. RESTAURAR
# Al restaurar IAM, también restaurará las dependencias porque ya copiamos sus csproj
RUN dotnet restore "IAM/IAM.csproj"

# 3. COPIAR EL CÓDIGO FUENTE
# Copiamos todo el contenido de la solución al contenedor
COPY . .

# 4. COMPILAR Y PUBLICAR
WORKDIR "/src/IAM"
RUN dotnet build "IAM.csproj" -c Release -o /app/build
RUN dotnet publish "IAM.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. IMAGEN FINAL (RUNTIME - Solo para ejecutar)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Exponemos el puerto interno 8080 (Estándar en contenedores .NET modernos)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copiamos lo compilado desde la etapa anterior
COPY --from=build /app/publish .

# 6. ARRANQUE
# ¡Aquí ponemos el nombre real de tu DLL!
ENTRYPOINT ["dotnet", "IAM.dll"]