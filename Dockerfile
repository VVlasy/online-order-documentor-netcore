# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY online-order-documentor/*.csproj ./online-order-documentor/
COPY online-order-documentor.IntegrationTests/*.csproj ./online-order-documentor.IntegrationTests/
COPY online-order-documentor.UnitTests/*.csproj ./online-order-documentor.UnitTests/
RUN dotnet restore

# copy everything else and build app
COPY online-order-documentor/. ./online-order-documentor/
WORKDIR /source/online-order-documentor
RUN dotnet publish online-order-documentor.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "online-order-documentor.dll"]