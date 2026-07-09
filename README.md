# Antigravity Game Collection Management System

Antigravity is a complete, production-ready, full-stack game collection manager. The application features a premium dark-themed visual layout inspired by modern launchers like Steam, GOG Galaxy, and Xbox App, and provides users with powerful tools to catalog their library, track play hours, manage storefront connections, and view collection statistics charts.

---

## 🚀 Features

- **KPI Analytics Dashboard**: Track total games owned, completion percentage, backlog status, recently added, and most played items.
- **Data Charts**: Visually explore your catalog distribution using interactive Recharts components for platforms, genres, status, and release year trends.
- **Multiple Views**: Seamlessly switch between Grid view, list tabular layout, large Card info view, and Gallery cover wall.
- **Advanced Filters**: Collapsible multi-select filters including search by title, platform, service, developer, publisher, and specific completion states.
- **Image Uploads & Media**: Attach cover art, banner graphics, screenshots, and link YouTube trailer videos with automatic embed player rendering.
- **Administrative Master Tables**: Unified administration panel to add, edit, and delete developers, publishers, platforms, services, and taxonomies (genres, tags, themes, series, and franchises).

---

## 🛠️ Technology Stack

### Backend
- **ASP.NET Core Web API 8.0 (LTS)**: Clean Architecture with Domain-Driven Design (DDD) principles.
- **Entity Framework Core 8.0**: Database ORM mapping SQL Server databases.
- **ASP.NET Identity Core**: Built-in authorization, role assignment (`Administrator`, `User`), and account credential hashes.
- **JWT Bearer Token Authentication**: Secure token verification for API calls.
- **MediatR**: CQRS design pattern separating Queries from Command updates.
- **FluentValidation**: Request pipeline behaviors executing automatic input check validations.
- **AutoMapper**: DTO mapping profiles translating domain models.
- **RFC 7807 ProblemDetails**: Custom global exception handler middleware returning consistent error schemas.
- **Serilog**: Structured console request logging.

### Frontend
- **React (latest stable)** + **Vite**: Rapid Hot Module Replacement (HMR) development server.
- **Bootstrap 5 & Icons**: Modern dark theme CSS variables matching premium styling.
- **Recharts**: Responsive canvas SVGs illustrating library data charts.
- **React Router 6**: Client-side single page path routing, protected guard checks.
- **Axios**: Token interceptors, global response handlers, and request mappings.

---

## 📁 Architecture Overview

```
├── backend/                              # ASP.NET Core Solution
│   ├── GameCollection.slnx               # Modern XML Solution format
│   ├── GameCollection.Domain/            # Domain Entities, Repositories Interfaces
│   ├── GameCollection.Application/       # DTOs, Mappings, CQRS Commands/Queries, Validators
│   ├── GameCollection.Infrastructure/    # DBContext, Identity, Local Uploads, Migrations
│   └── GameCollection.API/               # Controllers, Middlewares, Program Bootstrapper
│
└── frontend/                             # React SPA Client
    ├── public/                           # Static assets
    └── src/
        ├── components/                   # Layout, RouteGuards, Common widgets
        ├── context/                      # AuthContext session provider
        ├── pages/                        # Dashboard, Library, GameForm, Profile, Admin CRUD
        ├── services/                     # Axios Client setup
        └── index.css                     # HSL Gaming Theme CSS styling
```

---

## 🏁 Getting Started

### 📋 Prerequisites
- [.NET SDK 8.0 / 10.0](https://dotnet.microsoft.com/download)
- [Node.js (v18+)](https://nodejs.org)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB/Express)
- [Docker & Docker Compose](https://www.docker.com) (Optional)

---

### 🖥️ Local Manual Run

#### 1. Setup Backend
1. Open a terminal in `backend/`.
2. Generate migrations and update database (automatically seeds default tables):
   ```bash
   dotnet ef database update --project GameCollection.Infrastructure --startup-project GameCollection.API
   ```
3. Run the API:
   ```bash
   dotnet run --project GameCollection.API
   ```
4. Confirm backend Swagger UI is visible at:
   - HTTP:  `http://localhost:5139/swagger`
   - HTTPS: `https://localhost:7214/swagger`

#### 2. Setup Frontend
1. Open a terminal in `frontend/`.
2. Install packages:
   ```bash
   npm install
   ```
3. Boot development dev server:
   ```bash
   npm run dev
   ```
4. Access the web app at `http://localhost:5173`.

### 🛠️ VS Code Debugging (Recommended)

You can launch and debug both the ASP.NET Core API and the React frontend simultaneously inside VS Code:

1. Open the repository folder in VS Code.
2. Press `F5` or open the **Run and Debug** view.
3. Select **`Both (API & Frontend)`** from the dropdown menu.
4. Click the green play icon (Start Debugging).
5. This automated workflow will:
   - Build and start the C# API project, serving over HTTPS and launching the **Swagger Documentation** page at `https://localhost:7214/swagger`.
   - Start the React/Vite development server, launching the **Frontend Web App** at `http://localhost:5173`.


---

### 🐳 Docker Compose Deployment (Single Command)

Run the entire stack (Database, API, Frontend) instantly using Docker:
```bash
docker-compose up --build -d
```
- **React Frontend**: access at `http://localhost`.
- **API / Swagger Documentation**: access at `http://localhost:5139/swagger` (container maps internal port 8080 → host 5139).
- **Database**: SQL Server container maps port `1433`.

---

## 🔐 Credentials (Default Seed Data)

After running the system, use the following logins to test roles:

| Username | Password | Role |
| :--- | :--- | :--- |
| **admin** | `Admin123!` | **Administrator, User** (CRUD Metadata enabled) |
| **user** | `User123!` | **User** (Only views catalog data) |

---

## ⚙️ Configuration Reference

All runtime settings are externalized — **no hardcoded values**. Modify the following files to adjust the system for your environment.

### Backend (`appsettings.json`)

| Key | Default | Purpose |
| :--- | :--- | :--- |
| `CorsSettings:AllowedOrigins` | `http://localhost:5173` | Comma-separated list of allowed frontend origins for CORS |
| `ConnectionStrings:DefaultConnection` | LocalDB connection string | SQL Server connection string |
| `JwtSettings:Secret` | (see appsettings) | HMAC-SHA256 signing key for JWT tokens |
| `JwtSettings:Issuer` | `GameCollectionAPI` | JWT token issuer claim |
| `JwtSettings:Audience` | `GameCollectionApp` | JWT token audience claim |
| `JwtSettings:DurationInMinutes` | `1440` | Token expiration time (24 hours) |

### Frontend (`.env` / Environment Variables)

| Variable | Default | Purpose |
| :--- | :--- | :--- |
| `VITE_API_BASE_URL` | `http://localhost:5139/api` | Base URL for all Axios API calls |

> **Tip**: For VS Code debugging, ports are forced via `ASPNETCORE_URLS` in `.vscode/launch.json` to match the documented ports above (`https://localhost:7214` and `http://localhost:5139`).

---

## 🔮 Future Extension Points

- **IGDB / RAWG Metadata Integrations**: Link game search queries to fetch metadata, tags, and cover images from public gaming databases automatically.
- **Steam / Epic / GOG Account Sync**: Integrate OAuth or developer tokens to import games owned on major storefronts automatically.
- **Social Library Sharing**: Implement profile visibility toggles to allow sharing libraries or backlog status lists with other gamers.
