# Permission Reference

tags: #security #permissions #reference

## All System Permissions

| Permission Identifier | Description | Category |
|:---|:---|:---|
| `Games.View` | View games in the central catalog | Central Catalog |
| `Games.Create` | Create new authoritative game catalog records | Central Catalog |
| `Games.Edit` | Modify metadata, taxonomy, and media of catalog games | Central Catalog |
| `Games.Delete` | Soft-delete games from the central catalog | Central Catalog |
| `Libraries.View` | View personal game library and play statistics | Personal Library |
| `Libraries.Manage` | Add/edit/remove personal library entries and playtime | Personal Library |
| `GameRequests.Submit` | Submit new game requests for catalog ingestion | Game Requests |
| `GameRequests.ViewMine` | View own game request statuses and feedback | Game Requests |
| `GameRequests.Review` | Access reviewer dashboard with all pending requests | Game Requests |
| `GameRequests.Approve` | Approve requests and ingest them into the catalog | Game Requests |
| `GameRequests.Reject` | Reject requests with reviewer feedback notes | Game Requests |
| `GameRequests.Delete` | Delete game request records permanently | Game Requests |
| `Roles.View` | View defined roles and their assigned permissions | RBAC Admin |
| `Roles.Create` | Create new system roles *(Super Admin exclusive)* | RBAC Admin |
| `Roles.Edit` | Edit role names, descriptions, permission assignments *(Super Admin exclusive)* | RBAC Admin |
| `Roles.Delete` | Deactivate or delete system roles *(Super Admin exclusive)* | RBAC Admin |
| `Users.View` | View registered user profiles and their roles | User Management |
| `Users.ManageRoles` | Assign and modify roles for registered users | User Management |
| `Metadata.View` | View taxonomy master records (genres, platforms, etc.) | Master Taxonomy |
| `Metadata.Manage` | Create, edit, and delete taxonomy master records | Master Taxonomy |

---

## Default Role → Permission Matrix

| Permission | Super Admin | Admin | Game Curator | User |
|:---|:---:|:---:|:---:|:---:|
| `Games.View` | ✅ | ✅ | ✅ | ✅ |
| `Games.Create` | ✅ | ✅ | ✅ | ❌ |
| `Games.Edit` | ✅ | ✅ | ✅ | ❌ |
| `Games.Delete` | ✅ | ✅ | ❌ | ❌ |
| `Libraries.View` | ✅ | ✅ | ✅ | ✅ |
| `Libraries.Manage` | ✅ | ✅ | ✅ | ✅ |
| `GameRequests.Submit` | ✅ | ✅ | ✅ | ✅ |
| `GameRequests.ViewMine` | ✅ | ✅ | ✅ | ✅ |
| `GameRequests.Review` | ✅ | ✅ | ✅ | ❌ |
| `GameRequests.Approve` | ✅ | ✅ | ✅ | ❌ |
| `GameRequests.Reject` | ✅ | ✅ | ✅ | ❌ |
| `GameRequests.Delete` | ✅ | ✅ | ❌ | ❌ |
| `Roles.View` | ✅ | ✅ | ❌ | ❌ |
| `Roles.Create` | ✅ | ❌ | ❌ | ❌ |
| `Roles.Edit` | ✅ | ❌ | ❌ | ❌ |
| `Roles.Delete` | ✅ | ❌ | ❌ | ❌ |
| `Users.View` | ✅ | ✅ | ❌ | ❌ |
| `Users.ManageRoles` | ✅ | ✅ | ❌ | ❌ |
| `Metadata.View` | ✅ | ✅ | ✅ | ❌ |
| `Metadata.Manage` | ✅ | ✅ | ✅ | ❌ |

> [!NOTE]
> Super Admin has an **unconditional bypass** — it does not technically need individual permissions assigned. The matrix above is for illustration.

---

## Seeded Default Credentials

| Username | Password | Roles |
|:---|:---|:---|
| `admin` | `Admin123!` | Super Admin, Admin |
| `user` | `User123!` | User |

---

## Navigation

← [[Security/RBAC System]] | [[Backend/Domain Layer]] →
