# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Fortuity.sln Directory.Build.props Directory.Packages.props ./
COPY src/Fortuity.Domain/Fortuity.Domain.csproj src/Fortuity.Domain/
COPY src/Fortuity.Application/Fortuity.Application.csproj src/Fortuity.Application/
COPY src/Fortuity.Infrastructure/Fortuity.Infrastructure.csproj src/Fortuity.Infrastructure/
COPY src/Fortuity.Api/Fortuity.Api.csproj src/Fortuity.Api/
COPY src/Fortuity.Web/Fortuity.Web.csproj src/Fortuity.Web/
RUN dotnet restore src/Fortuity.Api/Fortuity.Api.csproj \
 && dotnet restore src/Fortuity.Web/Fortuity.Web.csproj

COPY src/ src/
RUN dotnet publish src/Fortuity.Api/Fortuity.Api.csproj -c Release --no-restore -o /app/api \
 && dotnet publish src/Fortuity.Web/Fortuity.Web.csproj -c Release --no-restore -o /app/web

# fi-FI money formatting needs ICU, so the Debian-based runtime image is used
# instead of the Alpine one on purpose.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
RUN mkdir /data && chown app:app /data
COPY --from=build --chown=app:app /app/api ./
USER app
ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_HTTP_PORTS=8080 \
    ConnectionStrings__Fortuity="Data Source=/data/fortuity.db" \
    RepairNet__BaseUrl=http://localhost:8080
VOLUME /data
EXPOSE 8080
ENTRYPOINT ["dotnet", "Fortuity.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS web
WORKDIR /app
COPY --from=build --chown=app:app /app/web ./
USER app
ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_HTTP_PORTS=8080 \
    FortuityApi__BaseUrl=http://api:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Fortuity.Web.dll"]
