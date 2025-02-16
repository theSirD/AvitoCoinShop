FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 3. Копируем sln и все .csproj файлы
COPY ["src/AvitoCoinShop/AvitoCoinShop.csproj", "src/AvitoCoinShop/"]
COPY ["src/Application/AvitoCoinShop.Application/AvitoCoinShop.Application.csproj", "src/Application/AvitoCoinShop.Application/"]
COPY ["src/Application/AvitoCoinShop.Application.Abstractions/AvitoCoinShop.Application.Abstractions.csproj", "src/Application/AvitoCoinShop.Application.Abstractions/"]
COPY ["src/Application/AvitoCoinShop.Application.Contracts/AvitoCoinShop.Application.Contracts.csproj", "src/Application/AvitoCoinShop.Application.Contracts/"]
COPY ["src/Application/AvitoCoinShop.Application.Models/AvitoCoinShop.Application.Models.csproj", "src/Application/AvitoCoinShop.Application.Models/"]
COPY ["src/Infrastructure/AvitoCoinShop.Infrastructure.Persistence/AvitoCoinShop.Infrastructure.Persistence.csproj", "src/Infrastructure/AvitoCoinShop.Infrastructure.Persistence/"]
COPY ["src/Presentation/AvitoCoinShop.Presentation.Http/AvitoCoinShop.Presentation.Http.csproj", "src/Presentation/AvitoCoinShop.Presentation.Http/"]

# 4. Восстанавливаем зависимости
RUN dotnet restore "src/AvitoCoinShop/AvitoCoinShop.csproj"

COPY . .
WORKDIR "/src/src/AvitoCoinShop"
RUN dotnet build "AvitoCoinShop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AvitoCoinShop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AvitoCoinShop.dll"]

#
## 5. Копируем весь исходный код и компилируем приложение
#COPY . .
#WORKDIR /src/Presentation/AvitoCoinShop.Presentation.Http
#RUN dotnet publish -c Release -o /app
#
## 6. Этап для выполнения (более легковесный образ)
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app .
#
## 7. Указываем точку входа
#ENTRYPOINT ["dotnet", "AvitoCoinShop.dll"]
