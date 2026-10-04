# VS Code Debugging

tags: #deployment #vscode #debugging #development

## Overview

The repository includes a pre-configured VS Code launch configuration that starts both the ASP.NET Core API and the React frontend simultaneously with full debugging support.

---

## Setup

1. Open the repository root (`Antigravity/`) in VS Code
2. Ensure you have these VS Code extensions installed:
   - **C# Dev Kit** (or C# extension by Microsoft)
   - **JavaScript Debugger** (built-in)

---

## Running

1. Press `F5` **or** open the **Run and Debug** view (`Ctrl+Shift+D`)
2. Select **`Both (API & Frontend)`** from the configuration dropdown
3. Click the green ▶ **Start Debugging** button

This will:

| Service | URL | Action |
|:---|:---|:---|
| ASP.NET Core API | `https://localhost:7214` | Builds and launches, opens Swagger UI |
| React Dev Server | `http://localhost:5173` | Starts Vite, opens browser |

---

## Configuration Files

### `.vscode/launch.json`

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "API (ASP.NET Core)",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/backend/GameCollection.API/bin/Debug/net10.0/GameCollection.API.dll",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "https://localhost:7214;http://localhost:5139"
      },
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    },
    {
      "name": "Frontend (React/Vite)",
      "type": "node",
      "request": "launch",
      "runtimeExecutable": "npm",
      "runtimeArgs": ["run", "dev"],
      "cwd": "${workspaceFolder}/frontend"
    }
  ],
  "compounds": [
    {
      "name": "Both (API & Frontend)",
      "configurations": ["API (ASP.NET Core)", "Frontend (React/Vite)"]
    }
  ]
}
```

---

## Debugging Features

### Backend (C#)
- Set breakpoints in any `.cs` file
- Full variable inspection in the Debug panel
- Hot Reload for minor code changes

### Frontend (React/JavaScript)
- Vite's Hot Module Replacement (HMR) — most changes update without full reload
- Browser DevTools remain the primary debugging tool for React state

---

## Port Reference

| Service | Port | Protocol |
|:---|:---|:---|
| React Dev Server | 5173 | HTTP |
| API (HTTP) | 5139 | HTTP |
| API (HTTPS) | 7214 | HTTPS |
| Swagger UI | 5139 / 7214 | `/swagger` path |

---

## Navigation

← [[Deployment/Local Setup]] | [[Deployment/Docker Compose]] →
