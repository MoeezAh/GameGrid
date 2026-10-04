# Frontend Settings

tags: #configuration #frontend #environment

## Environment Variables

The frontend uses **Vite** environment variables, which must be prefixed with `VITE_` to be accessible in React code.

---

## `.env` File (Development)

Create this file in the `frontend/` directory:

```env
VITE_API_BASE_URL=http://localhost:5139/api
```

---

## Variable Reference

| Variable | Default | Description |
|:---|:---|:---|
| `VITE_API_BASE_URL` | `http://localhost:5139/api` | Base URL for all Axios API requests. Must match the running API server. |

---

## Usage in Code

```javascript
// services/api.js
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5139/api',
});
```

The fallback (`|| 'http://localhost:5139/api'`) means the app works even without a `.env` file in development.

---

## Per-Environment Configuration

| Environment | `.env` file | `VITE_API_BASE_URL` |
|:---|:---|:---|
| Local (VS Code debug) | `.env` or none | `http://localhost:5139/api` |
| Docker Compose | Build-time arg or nginx proxy | `http://localhost:5000/api` or proxied |
| Production | `.env.production` | `https://api.yourdomain.com/api` |

---

## Vite Config

**File:** `frontend/vite.config.js`

```javascript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
})
```

The dev server runs on `http://localhost:5173` by default.

---

## Docker Build (Frontend)

In Docker, the `VITE_API_BASE_URL` must be baked in at **build time** since Vite replaces `import.meta.env.*` at bundle creation. If the API URL changes after the image is built, a rebuild is required.

**Dockerfile (frontend):**
```dockerfile
ARG VITE_API_BASE_URL=http://localhost:5139/api
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build
```

---

## Theme Persistence

The Dark/Light theme toggle is stored in `localStorage`:

```javascript
// Set by Profile.jsx theme switcher
localStorage.setItem('theme', 'dark');   // or 'light'

// Applied in index.css / Layout.jsx via data attribute:
document.documentElement.setAttribute('data-theme', theme);
```

This is not an environment variable — it's a per-user browser preference.

---

## Navigation

← [[Configuration/Backend Settings]] | [[Deployment/Local Setup]] →
