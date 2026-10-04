# Local Setup

tags: #deployment #setup #local #development

## Prerequisites

| Requirement | Version | Download |
|:---|:---|:---|
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| Node.js | 18.0+ | https://nodejs.org |
| SQL Server | LocalDB / Express / Full | https://www.microsoft.com/sql-server |
| Git | Any | https://git-scm.com |

---

## Step 1 — Clone the Repository

```bash
git clone <repository-url>
cd Antigravity
```

---

## Step 2 — Setup Backend

Open a terminal in the `backend/` directory.

### 2a. Apply Database Migrations

```bash
dotnet ef database update \
  --project GameCollection.Infrastructure \
  --startup-project GameCollection.API
```

This will:
- Create the `GameCollectionDb` SQL Server database (LocalDB by default)
- Run all EF Core migrations
- **Automatically seed** default permissions, roles, and users on first run

### 2b. Run the API

```bash
dotnet run --project GameCollection.API
```

### 2c. Verify

Open a browser and confirm the Swagger UI loads:
- HTTP: `http://localhost:5139/swagger`
- HTTPS: `https://localhost:7214/swagger`

---

## Step 3 — Setup Frontend

Open a **separate** terminal in the `frontend/` directory.

### 3a. Install Packages

```bash
npm install
```

### 3b. Start Dev Server

```bash
npm run dev
```

### 3c. Verify

The app opens automatically at: `http://localhost:5173`

---

## Default Logins

| Username | Password | Role |
|:---|:---|:---|
| `admin` | `Admin123!` | Super Admin, Admin |
| `user` | `User123!` | User |

---

## Troubleshooting

### "Cannot connect to database"

Check that SQL Server LocalDB is running:
```powershell
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

Or update the connection string in `appsettings.Development.json` to point to your SQL Server instance.

### "CORS error in browser"

Ensure `CorsSettings:AllowedOrigins` in `appsettings.json` includes `http://localhost:5173`.

### "401 Unauthorized on all API calls"

Check that the API is running and `VITE_API_BASE_URL` in `.env` matches the running API port.

### "dotnet ef not found"

Install the EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

---

## Navigation

← [[Configuration/Frontend Settings]] | [[Deployment/VS Code Debugging]] →
