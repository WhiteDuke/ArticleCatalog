FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN groupadd -r appuser && \
    useradd -r -g appuser -u 1000 appuser
USER appuser

WORKDIR /app
EXPOSE 5041

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ArticleCatalog.Service/ArticleCatalog.Service.csproj", "ArticleCatalog.Service/"]
COPY ["ArticleCatalog.Entities/ArticleCatalog.Entities.csproj", "ArticleCatalog.Entities/"]
COPY ["ArticleCatalog.DataAccess/ArticleCatalog.DataAccess.csproj", "ArticleCatalog.DataAccess/"]
COPY ["ArticleCatalog.Dto/ArticleCatalog.Dto.csproj", "ArticleCatalog.Dto/"]
COPY ["ArticleCatalog.Domain/ArticleCatalog.Domain.csproj", "ArticleCatalog.Domain/"]

RUN dotnet restore "ArticleCatalog.Service/ArticleCatalog.Service.csproj"
RUN dotnet restore "ArticleCatalog.Entities/ArticleCatalog.Entities.csproj"
RUN dotnet restore "ArticleCatalog.DataAccess/ArticleCatalog.DataAccess.csproj"
RUN dotnet restore "ArticleCatalog.Dto/ArticleCatalog.Dto.csproj"
RUN dotnet restore "ArticleCatalog.Domain/ArticleCatalog.Domain.csproj"

COPY . .
WORKDIR "ArticleCatalog.Service"
RUN dotnet build "ArticleCatalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ArticleCatalog.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArticleCatalog.Service.dll"]
