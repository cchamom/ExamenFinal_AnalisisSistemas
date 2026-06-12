# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
# (Asegúrate de que el nombre coincida exactamente con tu .csproj)
COPY ["ExamenFinal_Analisis/ExamenFinal_Analisis.csproj", "ExamenFinal_Analisis/"]
RUN dotnet restore "ExamenFinal_Analisis/ExamenFinal_Analisis.csproj"

# Copiar todo el código y compilar
COPY . .
WORKDIR "/src/ExamenFinal_Analisis"
RUN dotnet build "ExamenFinal_Analisis.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "ExamenFinal_Analisis.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final / producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Forzar a .NET a escuchar en el puerto que Render asigne dinámicamente
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "ExamenFinal_Analisis.dll"]