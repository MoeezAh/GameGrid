# Overview

tags: #overview #introduction

## What is GameGrid (Antigravity)?

**Antigravity** (project codename: **GameGrid**) is a complete, production-ready, full-stack game collection manager. It is designed for gamers, collectors, curators, and gaming community administrators who want a self-hosted, privacy-first alternative to platforms like Backloggery, IGDB, or the library management features of Steam/GOG Galaxy.

The visual design draws inspiration from premium modern gaming launchers — Steam, GOG Galaxy, and the Xbox App — featuring a dark-themed, glassmorphic UI with vibrant HSL colour accents.

---

## Core Philosophy

```
+-----------------------------------------------------------------------------------+
|                               ANTIGRAVITY ECOSYSTEM                               |
+-----------------------------------------------------------------------------------+
|                                                                                   |
|  +---------------------------+                     +---------------------------+  |
|  |   Central Game Catalog    |<===================>|   Personal User Library   |  |
|  | (Authoritative Metadata)  |  "Add to Library"   |   (Ownership & Playtime)  |  |
|  +---------------------------+                     +---------------------------+  |
|               ^                                                  |                |
|               | (Approve & Ingest)                               |                |
|  +---------------------------+                                   v                |
|  |   Game Request Workflow   |<--------------------+   +-------------------+      |
|  |  (Community Contributions)|   "Submit Request"  |   | Analytics & KPIs  |      |
|  +---------------------------+                     |   | (Visual Charts)   |      |
|                                                    |   +-------------------+      |
|                                                    |                              |
|  +-------------------------------------------------+---------------------------+  |
|  |                   Generic Role-Based Access Control (RBAC)                  |  |
|  |              Users  --->  Assigned Roles  --->  Granular Permissions        |  |
|  +-----------------------------------------------------------------------------+  |
+-----------------------------------------------------------------------------------+
```

### The Two-Catalog Design

The most important architectural decision is the **strict separation** between:

| Concept | Description |
|:---|:---|
| **Central Game Catalog** | Authoritative, shared database of video games. Curated by trusted users (Admins, Curators). Think of this as the "master record." |
| **Personal User Library** | Each user's personal collection. Tracks ownership type, platforms owned, playtime, completion status, personal rating, and notes — without touching the central catalog. |

This means a user can own a game on PC only while the catalog shows it's also available on PS5 — both facts coexist independently.

---

## Key Features at a Glance

| Feature | Description |
|:---|:---|
| 🔐 Dynamic RBAC | Permissions are discrete identifiers — no hardcoded role names in business logic |
| 📚 Decoupled Library | Personal library is independent from the authoritative catalog |
| 📝 Game Request Workflow | Community-contributed game submissions with reviewer queue and duplicate detection |
| 📊 KPI Dashboard | Charts and metrics for personal library (playtime, genres, completion, platforms) |
| 🖼️ Multiple Views | Grid, List, Card, Gallery layouts for catalog and library browsing |
| 🔎 Advanced Filters | Multi-select filters across title, platform, service, developer, publisher, status |
| 🖼️ Media Management | Cover art, banners, screenshots, YouTube trailers |
| 🛠️ Admin Taxonomy | Full CRUD for 9 metadata entities (platforms, genres, developers, publishers, etc.) |
| 🌓 Theme Switcher | Persistent Dark/Light mode toggle |

---

## Navigation

← [[Home]] | [[Tech Stack]] →
