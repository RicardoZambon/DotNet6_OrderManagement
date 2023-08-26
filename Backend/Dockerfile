FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
ENV TZ=America/Sao_Paulo

WORKDIR /app
COPY --from=build-env /app/out /app

ENTRYPOINT ["dotnet", "WebApi.dll", "--environment=Development"]