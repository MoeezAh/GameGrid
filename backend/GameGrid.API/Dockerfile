# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy projects and restore dependencies
COPY ["GameGrid.slnx", "./"]
COPY ["GameGrid.Domain/GameGrid.Domain.csproj", "GameGrid.Domain/"]
COPY ["GameGrid.Application/GameGrid.Application.csproj", "GameGrid.Application/"]
COPY ["GameGrid.Infrastructure/GameGrid.Infrastructure.csproj", "GameGrid.Infrastructure/"]
COPY ["GameGrid.API/GameGrid.API.csproj", "GameGrid.API/"]

RUN dotnet restore "GameGrid.API/GameGrid.API.csproj"

# Copy all source files and build
COPY . .
WORKDIR "/src/GameGrid.API"
RUN dotnet build "GameGrid.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "GameGrid.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameGrid.API.dll"]
