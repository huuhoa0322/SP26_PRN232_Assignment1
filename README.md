# FU News Management System

## Project Information
- **Student**: Do Huu Hoa (HE186716)
- **Class Code**: SE1884-NET
- **Course**: PRN232 - Building Cross-Platform Back-End Application With .NET

> This repository contains **two assignments** that progressively build a News Management System — from a simple client-server application (ASS01) to a fully distributed, intelligent system (ASS02).

---

## Quick Navigation

| | Assignment 01 | Assignment 02 |
|---|---|---|
| **Scope** | Monolithic (2 projects) | Distributed (10 projects) |
| **Architecture** | BE ↔ FE | Core API + Analytics API + AI API + FE |
| **Auth** | Simple JWT | JWT + Refresh Token |
| **Data Querying** | REST API | REST + OData |
| **Detailed README** | [README_ASS01.md](README_ASS01.md) | [README_ASS02.md](README_ASS02.md) |

---

## Technologies Used

| Technology | ASS01 | ASS02 |
|------------|:-----:|:-----:|
| .NET 8.0 | ✅ | ✅ |
| ASP.NET Core Web API | ✅ | ✅ |
| ASP.NET Core Razor Pages | ✅ | ✅ |
| Entity Framework Core | ✅ | ✅ |
| SQL Server | ✅ | ✅ |
| Bootstrap 5 | ✅ | ✅ |
| JWT Authentication | ✅ | ✅ |
| OData | — | ✅ |
| Refresh Token Flow | — | ✅ |
| SignalR (Real-time) | — | ✅ |
| Chart.js (Dashboard) | — | ✅ |
| EPPlus (Excel Export) | — | ✅ |
| Polly (Retry Policy) | — | ✅ |
| Background Worker | — | ✅ |
| Google Gemini API (AI) | — | ✅ |
| Audit Logging | — | ✅ |
| Offline Mode | — | ✅ |

---

## Project Structure

```
HE186716_DoHuuHoa_A01/
│
├── ── Assignment 01 ──────────────────────────────────────────
│
├── HE186716_DoHuuHoa_SE1884-NET_A01_BE/   # ASS01 Backend API
│   ├── Controllers/                        # API Controllers
│   ├── DTOs/                               # Data Transfer Objects
│   ├── Models/                             # Entity Models
│   ├── Repositories/                       # Data Access Layer
│   └── Services/                           # Business Logic Layer
│
├── HE186716_DoHuuHoa_SE1884-NET_A01_FE/   # ASS01 Frontend
│   ├── Pages/                              # Razor Pages (Admin, Staff, Auth, News)
│   ├── Models/                             # Frontend DTOs
│   └── Services/                           # API Service calls
│
├── ── Assignment 02 (v2 — Distributed) ───────────────────────
│
├── FUNewsManagement_v2_CoreAPI/            # Core API (Web API)
│   └── Controllers/                        # Auth, Accounts, Categories, Tags,
│                                           # News, AuditLogs, Dashboard, Profile
│
├── FUNewsManagement_v2_CoreAPI.BusinessLogic/
│   ├── DTOs/                               # 18 DTO files
│   ├── Services/                           # Business logic (16 files)
│   ├── Hubs/                               # SignalR NotificationHub
│   ├── Helpers/                            # JWT helpers
│   ├── Mappings/                           # AutoMapper profiles
│   └── Validators/                         # FluentValidation
│
├── FUNewsManagement_v2_CoreAPI.DataAccess/
│   ├── Models/                             # 7 entity models
│   └── Repositories/                       # Repository pattern (14 files)
│
├── FUNewsManagement_v2_AnalyticsAPI/       # Analytics API (Web API)
│   └── Controllers/                        # Dashboard, Trending, Export
│
├── FUNewsManagement_v2_AnalyticsAPI.BusinessLogic/
├── FUNewsManagement_v2_AnalyticsAPI.DataAccess/
│
├── FUNewsManagement_v2_AIAPI/              # AI API (Web API)
│   ├── Controllers/                        # suggest-tags, learn-tags
│   ├── DTOs/                               # Request/Response DTOs
│   ├── Services/                           # TagSuggestion & LearningCache
│   └── learning_cache.json                 # Persisted learning cache
│
├── FUNewsManagement_v2_FE/                 # v2 Frontend (Razor Pages)
│   ├── Pages/                              # Admin, Staff, Auth, News, Offline
│   │   └── Api/                            # Internal API proxy controllers
│   ├── Services/                           # CoreApi, AnalyticsApi, LocalCache
│   └── Workers/                            # DataRefreshWorker (6h refresh)
│
├── ── Shared ─────────────────────────────────────────────────
│
├── FUNewsManagement.sql                    # Database script
├── HE186716_DoHuuHoa_A01.slnx             # Solution file (all projects)
├── README.md                               # This file
├── README_ASS01.md                         # ASS01 detailed docs
└── README_ASS02.md                         # ASS02 detailed docs
```

---

## Installation Guide

### 1. System Requirements
- Visual Studio 2026
- .NET 8.0 SDK
- SQL Server 2025

### 2. Database Setup
```sql
-- Run the SQL script to create the database
-- File: FUNewsManagement.sql
```

### 3. Configure Connection Strings
Update `appsettings.json` in the relevant API projects:
```json
{
  "ConnectionStrings": {
    "MyCnn": "Server=YOUR_SERVER;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 4. Run

**Assignment 01** (2 projects):
```bash
# Backend
cd HE186716_DoHuuHoa_SE1884-NET_A01_BE && dotnet run

# Frontend
cd HE186716_DoHuuHoa_SE1884-NET_A01_FE && dotnet run
```

**Assignment 02** (4 projects — use Multiple Startup Projects):
```bash
cd FUNewsManagement_v2_CoreAPI && dotnet run       # :xxxx
cd FUNewsManagement_v2_AnalyticsAPI && dotnet run   # :xxxx
cd FUNewsManagement_v2_AIAPI && dotnet run          # :xxxx
cd FUNewsManagement_v2_FE && dotnet run             # Frontend
```

---

## API Endpoints Summary

### Assignment 01 — Backend API

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login |
| GET | `/api/account` | Get all accounts (Admin) |
| POST | `/api/account` | Create account |
| PUT | `/api/account/{id}` | Update account |
| DELETE | `/api/account/{id}` | Delete account |
| PUT | `/api/account/{id}/change-password` | Change password |
| GET | `/api/category` | Get all categories |
| POST | `/api/category` | Create category |
| PUT | `/api/category/{id}` | Update category |
| DELETE | `/api/category/{id}` | Delete category |
| GET | `/api/news` | Get active news (public) |
| GET | `/api/news/all` | Get all news |
| POST | `/api/news` | Create article |
| PUT | `/api/news/{id}` | Update article |
| DELETE | `/api/news/{id}` | Delete article |
| POST | `/api/news/{id}/duplicate` | Duplicate article |
| GET | `/api/tag` | Get all tags |
| POST | `/api/tag` | Create tag |
| PUT | `/api/tag/{id}` | Update tag |
| DELETE | `/api/tag/{id}` | Delete tag |
| GET | `/api/report/statistics` | Get statistics |
| GET | `/api/report/export` | Export CSV |

### Assignment 02 — Core API

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login (access_token + refresh_token) |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (revoke token) |
| GET | `/odata/Accounts` | List accounts (OData) |
| GET | `/odata/Accounts({id})` | Account detail |
| POST | `/api/accounts` | Create account |
| PUT | `/api/accounts/{id}` | Update account |
| DELETE | `/api/accounts/{id}` | Delete account |
| GET | `/odata/Categories` | List categories (OData) |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |
| PATCH | `/api/categories/{id}/status` | Toggle status |
| GET | `/odata/Tags` | List tags (OData) |
| POST | `/api/tags` | Create tag |
| PUT | `/api/tags/{id}` | Update tag |
| DELETE | `/api/tags/{id}` | Delete tag |
| GET | `/api/news/{id}` | Article detail |
| POST | `/api/news` | Create article (form-data) |
| PUT | `/api/news/{id}` | Update article (form-data) |
| DELETE | `/api/news/{id}` | Delete article |
| POST | `/api/news/{id}/duplicate` | Duplicate article |
| GET | `/api/recommend/{id}` | Related articles |
| GET | `/api/dashboard/stats` | Dashboard stats (Admin) |
| GET | `/odata/AuditLogs` | List audit logs (OData) |
| GET | `/api/auditlogs/filter` | Filter audit logs |
| GET | `/api/profile` | Get profile |
| PUT | `/api/profile` | Update profile |
| — | `/hubs/notifications` | SignalR Hub |

### Assignment 02 — Analytics API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/analytics/dashboard` | Article statistics (OData filtering) |
| GET | `/api/analytics/trending` | Trending articles |
| GET | `/api/analytics/export` | Export Excel (`?startDate=&endDate=`) |

### Assignment 02 — AI API

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ai/suggest-tags` | Suggest tags for content |
| POST | `/api/ai/learn-tags` | Learn user-selected tags |

---

## Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@FUNewsManagementSystem.org` | `@@abc123@@` |
| Staff | *(see database)* | *(see database)* |

## Authorization

| Role | Permissions |
|------|-------------|
| **Admin** | Manage Accounts, View Dashboard & Reports, View Audit Logs, Export Excel |
| **Staff** | Manage Categories, Articles, Tags, View Profile |
| **Lecturer** | View news articles (including inactive) |
| **Guest** | View active news articles only |

---

## Key Features by Assignment

### Assignment 01
- JWT Authentication (login only)
- CRUD: Account, Category, Tag, News Article
- Article duplication
- Report & CSV export
- Role-based authorization (Admin, Staff, Lecturer, Guest)
- 3-Layer Architecture + Repository Pattern

### Assignment 02 (Additions)
| Feature | Implementation |
|---------|----------------|
| **Distributed Architecture** | 4 independent projects via HttpClient |
| **JWT + Refresh Token** | Access token (60 min) + Refresh token (7 days) |
| **OData Integration** | `$filter`, `$orderby`, `$select`, `$top`, `$skip`, `$count` |
| **Audit Logging** | User, Action, Entity, Before/After JSON |
| **SignalR Notifications** | Real-time via `/hubs/notifications` |
| **AI Tag Suggestion** | Google Gemini API + keyword extraction + learning cache |
| **Dashboard & Charts** | Chart.js (Pie, Bar) with OData filtering |
| **Excel Export** | EPPlus-based analytics report (`.xlsx`) |
| **Background Worker** | `DataRefreshWorker` — cached data refresh every 6 hours |
| **Offline Mode** | Local JSON cache + "Offline Mode" banner + disabled CRUD |
| **Polly Retry Policy** | Automatic retry for failed API calls |
| **Image Upload** | File type & size validation for article images |

---

## Design Patterns
- **3-Layer Architecture** (Controller → Business Logic → Data Access)
- **Repository Pattern**
- **DTO Pattern**
- **Singleton Pattern** (DbContext)
- **Strategy Pattern** (Multiple HttpClient services — ASS02)
- **Observer Pattern** (SignalR real-time notifications — ASS02)
