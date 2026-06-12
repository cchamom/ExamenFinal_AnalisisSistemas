# 1. Usamos SDK 8.0 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copiamos el .csproj apuntando correctamente desde la raíz del repositorio
COPY ["ExamenFinal_Analisis/ExamenFinal_Analisis.csproj", "ExamenFinal_Analisis/"]
RUN dotnet restore "ExamenFinal_Analisis/ExamenFinal_Analisis.csproj"

# 3. Copiamos todo el código del repositorio al contenedor
COPY . .

# 4. Nos movemos a la carpeta del proyecto para compilar
WORKDIR "/src/ExamenFinal_Analisis"
RUN dotnet build "ExamenFinal_Analisis.csproj" -c Release -o /app/build

# 5. Publicamos la app
FROM build AS publish
RUN dotnet publish "ExamenFinal_Analisis.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Etapa final: Usamos ASP.NET 8.0 (Misma versión que el SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variables para Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ExamenFinal_Analisis.dll"]