# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy projects and restore dependencies
COPY ["GameCollection.slnx", "./"]
COPY ["GameCollection.Domain/GameCollection.Domain.csproj", "GameCollection.Domain/"]
COPY ["GameCollection.Application/GameCollection.Application.csproj", "GameCollection.Application/"]
COPY ["GameCollection.Infrastructure/GameCollection.Infrastructure.csproj", "GameCollection.Infrastructure/"]
COPY ["GameCollection.API/GameCollection.API.csproj", "GameCollection.API/"]

RUN dotnet restore "GameCollection.API/GameCollection.API.csproj"

# Copy all source files and build
COPY . .
WORKDIR "/src/GameCollection.API"
RUN dotnet build "GameCollection.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "GameCollection.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameCollection.API.dll"]
