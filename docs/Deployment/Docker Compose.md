# Docker Compose

tags: #deployment #docker #containers #production

## Overview

Docker Compose deploys the **entire stack** — SQL Server database, ASP.NET Core API, and React frontend — with a single command.

**File:** `docker-compose.yml` (repository root)

---

## Services

| Service | Container Name | Image | Port Mapping |
|:---|:---|:---|:---|
| `database` | `gamecollection-db` | `mcr.microsoft.com/mssql/server:2022-latest` | `1433:1433` |
| `backend-api` | `gamecollection-backend` | `gamecollection-api` (built locally) | `5000:80` |
| `frontend-app` | `gamecollection-frontend` | `gamecollection-frontend` (built locally) | `80:80` |

---

## Quick Start

```bash
# From the repository root (Antigravity/):
docker-compose up --build -d
```

This command:
1. Builds the API image from `backend/GameCollection.API/Dockerfile`
2. Builds the frontend image from `frontend/Dockerfile`
3. Pulls the SQL Server 2022 image
4. Creates and starts all containers in dependency order:
   - `database` starts first
   - `backend-api` waits for database
   - `frontend-app` waits for backend

---

## Access Points

| Service | URL |
|:---|:---|
| React Frontend | `http://localhost` |
| API / Swagger | `http://localhost:5000/swagger` |
| SQL Server | `localhost:1433` |

> [!NOTE]
> The API container maps internal port `80` to host port `5000`. The `VITE_API_BASE_URL` in the frontend Docker build should be set to `http://localhost:5000/api` or configured via build args.

---

## docker-compose.yml Reference

```yaml
version: '3.8'

services:
  database:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: gamecollection-db
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=SuperSecurePassword123!
    ports:
      - "1433:1433"
    volumes:
      - mssql-data:/var/opt/mssql      # Persistent DB storage

  backend-api:
    image: gamecollection-api
    build:
      context: ./backend
      dockerfile: GameCollection.API/Dockerfile
    container_name: gamecollection-backend
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=database;Database=GameCollectionDb;User Id=sa;Password=SuperSecurePassword123!;TrustServerCertificate=True;MultipleActiveResultSets=true
      - JwtSettings__Secret=SuperSecretKeyForGameCollectionManagementAppKeyHere_1234567890!
      - JwtSettings__Issuer=GameCollectionAPI
      - JwtSettings__Audience=GameCollectionApp
      - JwtSettings__DurationInMinutes=1440
    depends_on:
      - database
    volumes:
      - backend-uploads:/app/wwwroot/uploads  # Persistent file uploads

  frontend-app:
    image: gamecollection-frontend
    build:
      context: ./frontend
      dockerfile: Dockerfile
    container_name: gamecollection-frontend
    ports:
      - "80:80"
    depends_on:
      - backend-api

volumes:
  mssql-data:        # SQL Server data persistence
  backend-uploads:   # Uploaded media file persistence
```

---

## API Dockerfile

**File:** `backend/GameCollection.API/Dockerfile`

Multi-stage build:
1. **Build stage** — `dotnet restore`, `dotnet build`, `dotnet publish`
2. **Runtime stage** — Copies published output into ASP.NET Core runtime image

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# ...
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
RUN dotnet restore ...
RUN dotnet publish -c Release -o /app/publish ...
# ...
ENTRYPOINT ["dotnet", "GameCollection.API.dll"]
```

---

## Frontend Dockerfile + Nginx

**File:** `frontend/Dockerfile`

```dockerfile
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

**File:** `frontend/nginx.conf`

Serves the React SPA with HTML5 History API support (all paths fall back to `index.html`):

```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

---

## Useful Commands

```bash
# Start in background
docker-compose up --build -d

# View logs
docker-compose logs -f backend-api

# Stop all services
docker-compose down

# Stop and remove volumes (full reset)
docker-compose down -v

# Rebuild only the API
docker-compose up --build backend-api
```

---

## Navigation

← [[Deployment/VS Code Debugging]] | [[Roadmap]] →
