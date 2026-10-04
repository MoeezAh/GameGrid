# Roadmap

tags: #roadmap #future #extensions #planning

## Planned Features & Extension Points

These are the officially documented future enhancements identified during initial development.

---

## 🎮 External Metadata Integrations

### IGDB / RAWG Metadata Auto-Import
- **What:** Link game search queries to public gaming databases (IGDB, RAWG) to automatically fetch metadata, cover art, tags, and descriptions
- **Why:** Reduces manual data entry when adding games to the central catalog
- **Approach:** Add a new `IGameMetadataService` interface in Application layer with an IGDB/RAWG implementation in Infrastructure
- **Impact:** `GameForm.jsx` would gain a "Search IGDB" button to pre-populate fields

---

## 🔗 Platform Account Sync

### Steam / Epic / GOG Library Import
- **What:** Integrate OAuth or developer API tokens to import games owned on major storefronts automatically
- **Providers:** Steam Web API, Epic Games API, GOG Galaxy API
- **Why:** Eliminates manual library entry for existing gaming accounts
- **Approach:** New `SyncController` + background job or on-demand sync command
- **Impact:** Library page gains a "Sync from Steam" button

---

## 👥 Social & Community Features

### Public Library Sharing
- **What:** Profile visibility toggles that allow users to share their library, backlog lists, or completion stats with other users
- **Why:** Transforms the tool from a personal tracker into a community platform
- **Approach:** `UserProfile.IsPublic` field + public profile endpoint + shareable URL
- **Impact:** New `/u/:username` public profile page

---

## 💡 Other Potential Additions

| Feature | Description | Complexity |
|:---|:---|:---|
| **Price Tracking** | Monitor game prices across storefronts | High |
| **Achievement Sync** | Import achievement data from Steam API | Medium |
| **Review System** | Write and share full game reviews (separate from personal notes) | Medium |
| **Collection Reports** | PDF/Excel export of library data | Low |
| **Multiple Lists** | Custom named lists beyond Wishlist/Backlog | Medium |
| **Game Recommendations** | AI-based "you might like" suggestions from library data | High |
| **Mobile App** | Native iOS/Android app consuming the same API | High |
| **2FA** | Two-factor authentication for accounts | Medium |
| **Email Notifications** | Notify users when their game request is reviewed | Low |
| **Backup & Restore** | Export/import user library data as JSON | Low |

---

## Architecture Extension Points

The Clean Architecture design makes these additions straightforward:

1. **New feature** = new folder under `Application/Features/` + controller action + frontend page
2. **New metadata entity** = new Domain entity + EF migration + MetadataController category + AdminMetadata type
3. **New external service** = new interface in Application + implementation in Infrastructure
4. **New permission** = add to seeder + `PermissionConstants` class + assign to appropriate roles

---

## Navigation

← [[Deployment/Docker Compose]] | [[Home]] →
