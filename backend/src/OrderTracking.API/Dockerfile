FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копирование файлов проектов
COPY ["src/OrderTracking.API/OrderTracking.API.csproj", "src/OrderTracking.API/"]
COPY ["src/OrderTracking.Domain/OrderTracking.Domain.csproj", "src/OrderTracking.Domain/"]
COPY ["src/OrderTracking.Data/OrderTracking.Data.csproj", "src/OrderTracking.Data/"]
COPY ["src/OrderTracking.Service/OrderTracking.Service.csproj", "src/OrderTracking.Service/"]

# Восстановление зависимостей
RUN dotnet restore "src/OrderTracking.API/OrderTracking.API.csproj"

# Копирование всех файлов
COPY . .

# Сборка проекта
WORKDIR "/src/src/OrderTracking.API"
RUN dotnet build "OrderTracking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderTracking.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderTracking.API.dll"]
