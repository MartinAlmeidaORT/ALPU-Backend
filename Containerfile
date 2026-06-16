# syntax=docker/dockerfile:1

#############################
# Etapa 1: restore + build   #
#############################
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ALPU.slnx ./
COPY Domain/Domain.csproj Domain/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY Application/Application.csproj Application/
COPY GraphQL/GraphQL.csproj GraphQL/

COPY Domain/ Domain/
COPY DataAccess/ DataAccess/
COPY Application/ Application/
COPY GraphQL/ GraphQL/

# Restore y publish en un solo RUN para que los paquetes NuGet no se pierdan entre capas
RUN dotnet restore Domain/Domain.csproj && \
    dotnet restore DataAccess/DataAccess.csproj && \
    dotnet restore Application/Application.csproj && \
    dotnet restore GraphQL/GraphQL.csproj && \
    dotnet publish GraphQL/GraphQL.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

#############################
# Etapa 2: runtime           #
#############################
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

USER app

COPY --from=build --chown=app:app /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GraphQL.dll"]
