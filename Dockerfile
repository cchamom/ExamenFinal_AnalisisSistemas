FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiamos el .csproj indicando la ruta de la subcarpeta
COPY ["ExamenFinal_Analisis/ExamenFinal_Analisis.csproj", "ExamenFinal_Analisis/"]
RUN dotnet restore "ExamenFinal_Analisis/ExamenFinal_Analisis.csproj"

# 2. Copiamos todo el contenido del repositorio
COPY . .

# 3. Nos movemos dentro de la carpeta para compilar el proyecto
WORKDIR "/src/ExamenFinal_Analisis"
RUN dotnet build "ExamenFinal_Analisis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExamenFinal_Analisis.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ExamenFinal_Analisis.dll"]