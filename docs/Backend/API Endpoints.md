# API Endpoints

tags: #backend #api #endpoints #rest #reference

> **Base URL (Development):** `http://localhost:5139/api`
> **Swagger UI:** `http://localhost:5139/swagger` | `https://localhost:7214/swagger`

All authenticated endpoints require: `Authorization: Bearer <JWT>`

---

## Authentication — `/api/auth`

### `POST /api/auth/register`
Register a new user account. Auto-assigns the default **User** role.

**Request:**
```json
{
  "username": "gamer123",
  "email": "gamer@example.com",
  "password": "MyPassword123!"
}
```

**Response `200`:**
```json
{
  "token": "<JWT>",
  "username": "gamer123",
  "email": "gamer@example.com",
  "roles": ["User"],
  "permissions": ["Games.View", "Libraries.View", "Libraries.Manage", ...],
  "isSuperAdmin": false
}
```

---

### `POST /api/auth/login`
Authenticate and receive JWT token.

**Request:**
```json
{
  "usernameOrEmail": "admin",
  "password": "Admin123!"
}
```

**Response `200`:** Same shape as register response.

---

### `GET /api/auth/me`
Get current authenticated user's profile and permission list.

**Response `200`:**
```json
{
  "username": "admin",
  "email": "admin@example.com",
  "roles": ["Super Admin", "Admin"],
  "permissions": ["Games.View", "Games.Create", ...],
  "isSuperAdmin": true
}
```

---

### `POST /api/auth/change-password`
```json
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}
```

---

## Central Catalog — `/api/games`

### `GET /api/games`
Paginated, filtered catalog search.

**Query params:**
| Param | Type | Description |
|:---|:---|:---|
| `search` | string | Full-text search across title, alternate titles |
| `platformIds` | int[] | Filter by platform IDs |
| `genreIds` | int[] | Filter by genre IDs |
| `developerIds` | int[] | Filter by developer IDs |
| `publisherIds` | int[] | Filter by publisher IDs |
| `serviceIds` | int[] | Filter by digital service IDs |
| `sortBy` | string | `title`, `releaseDate`, `criticRating`, `communityRating` |
| `sortDesc` | bool | Descending sort order |
| `page` | int | Page number (default: 1) |
| `pageSize` | int | Items per page (default: 20) |

**Response `200`:**
```json
{
  "items": [
    {
      "id": 1,
      "title": "The Witcher 3: Wild Hunt",
      "coverImage": "/uploads/witcher3-cover.jpg",
      "releaseDate": "2015-05-19",
      "criticRating": 9.3,
      "platforms": ["PC", "PS5", "Xbox Series X"],
      "genres": ["RPG", "Open World"]
    }
  ],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20
}
```

---

### `GET /api/games/{id}`
Full game detail.

**Response `200`:**
```json
{
  "id": 1,
  "title": "The Witcher 3: Wild Hunt",
  "description": "...",
  "coverImage": "/uploads/...",
  "banner": "/uploads/...",
  "trailerUrl": "https://youtube.com/...",
  "releaseDate": "2015-05-19",
  "criticRating": 9.3,
  "communityRating": 9.1,
  "esrbRating": "M",
  "multiplayerSupport": false,
  "steamDeckCompatibility": "Verified",
  "platforms": [...],
  "genres": [...],
  "developers": [...],
  "publishers": [...]
}
```

---

### `POST /api/games` — `[Games.Create]`
Create a new catalog game.

### `PUT /api/games/{id}` — `[Games.Edit]`
Update catalog game metadata.

### `DELETE /api/games/{id}` — `[Games.Delete]`
Soft-delete a game (`IsDeleted = true`).

### `POST /api/games/upload` — Bearer
Upload media file (cover, banner, screenshot).

**Request:** `multipart/form-data` with file field
**Response:** `{ "url": "/uploads/<guid>.<ext>" }`

---

## Personal Library — `/api/libraries`

### `GET /api/libraries` — `[Libraries.View]`
Returns **only the authenticated user's** library entries.

### `POST /api/libraries` — `[Libraries.Manage]`
```json
{
  "gameId": 42,
  "ownGame": true,
  "wishlist": false,
  "backlog": false,
  "platformIds": [1, 3],
  "serviceIds": [2],
  "completionStatus": 1,
  "hoursPlayed": 0
}
```

### `PUT /api/libraries/{id}` — `[Libraries.Manage]`
Update playtime, completion status, rating, or notes.

### `DELETE /api/libraries/{id}` — `[Libraries.Manage]`
Remove game from personal library (does **not** delete the catalog entry).

---

## Game Requests — `/api/gamerequests`

### `GET /api/gamerequests/my` — `[GameRequests.ViewMine]`
### `POST /api/gamerequests` — `[GameRequests.Submit]`
```json
{
  "gameTitle": "Elden Ring 2",
  "approximateReleaseYear": "2027",
  "platforms": "PC, PS5",
  "links": "https://store.steampowered.com/...",
  "additionalInformation": "Sequel to Elden Ring"
}
```

### `GET /api/gamerequests` — `[GameRequests.Review]`
All requests with filter by status.

### `GET /api/gamerequests/check-duplicate?title=Elden Ring`
Returns catalog similarity matches to prevent duplicates.

### `POST /api/gamerequests/{id}/approve` — `[GameRequests.Approve]`
### `POST /api/gamerequests/{id}/reject` — `[GameRequests.Reject]`
```json
{ "reviewNotes": "This game is already in the catalog as 'Elden Ring'." }
```

---

## RBAC & Users

### `GET /api/roles` — `[Roles.View]`
### `GET /api/roles/permissions` — `[Roles.View]`
### `POST /api/roles` — Super Admin
### `PUT /api/roles/{id}` — Super Admin
### `DELETE /api/roles/{id}` — Super Admin

### `GET /api/users` — `[Users.View]`
### `PUT /api/users/{id}/roles` — `[Users.ManageRoles]`
```json
{ "roleIds": [1, 3] }
```

---

## Master Metadata — `/api/metadata/{category}`

`category` values: `platforms` | `services` | `developers` | `publishers` | `genres` | `tags` | `themes` | `franchises` | `series`

### `GET /api/metadata/{category}/list` — Bearer
Lightweight `[{id, name}]` array for form dropdowns.

### `GET /api/metadata/{category}` — `[Metadata.View]`
Paginated management table.

### `POST /api/metadata/{category}` — `[Metadata.Manage]`
### `PUT /api/metadata/{category}/{id}` — `[Metadata.Manage]`
### `DELETE /api/metadata/{category}/{id}` — `[Metadata.Manage]`

---

## Dashboard — `/api/dashboard`

### `GET /api/dashboard/stats` — Bearer
```json
{
  "totalGames": 47,
  "completedGames": 12,
  "completionPercentage": 25.5,
  "backlogCount": 20,
  "totalHoursPlayed": 340.5,
  "platformDistribution": [
    { "name": "PC", "value": 30 },
    { "name": "PS5", "value": 10 }
  ],
  "completionStatusDistribution": [
    { "name": "Not Started", "value": 15 },
    { "name": "Completed", "value": 12 }
  ],
  "genreDistribution": [...],
  "releaseYearTimeline": [...],
  "currentlyPlaying": [...],
  "recentBacklog": [...]
}
```

---

## Navigation

← [[Backend/API Layer]] | [[Frontend/React App Structure]] →
