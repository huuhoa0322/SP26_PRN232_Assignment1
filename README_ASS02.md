# FU News Management System v2 — Distributed & Intelligent

## Project Information
- **Student**: Do Huu Hoa (HE186716)
- **Class Code**: SE1884-NET
- **Course**: PRN232 - Building Cross-Platform Back-End Application With .NET
- **Assignment**: 02

## Technologies Used
- **.NET 8.0**
- **ASP.NET Core Web API** (Core API, Analytics API, AI API)
- **ASP.NET Core Razor Pages** (Frontend)
- **Entity Framework Core** (ORM)
- **OData** (Advanced querying — `$filter`, `$orderby`, `$select`, `$top`, `$skip`, `$count`)
- **SQL Server** (Database)
- **JWT Authentication** with **Refresh Token** flow
- **SignalR** (Real-time notifications)
- **Chart.js** (Dashboard charts)
- **Bootstrap 5** (UI Framework)
- **EPPlus** (Excel report export)
- **Polly** (Retry policy for API calls)
- **Background Worker** (`IHostedService` — periodic data refresh every 6 hours)
- **Google Gemini API** (AI tag suggestion)

## Architecture Overview

The system follows a **distributed architecture** with 4 independent components communicating via **HttpClient**:

```
┌───────────────────────────────────────────────────────────────────┐
│                        Frontend (Razor Pages)                     │
│                   http://localhost:xxxx                            │
│  ┌──────────┐  ┌──────────────────┐  ┌────────────────────────┐  │
│  │  Pages/  │  │    Services/     │  │     Workers/           │  │
│  │  Admin/  │  │  CoreApiService  │  │  DataRefreshWorker     │  │
│  │  Staff/  │  │  AnalyticsApi..  │  │  (HostedService,       │  │
│  │  Auth/   │  │  LocalCacheSvc   │  │   refreshes every 6h) │  │
│  │  News/   │  │                  │  │                        │  │
│  └──────────┘  └──────────────────┘  └────────────────────────┘  │
│       │                │                        │                 │
└───────┼────────────────┼────────────────────────┼─────────────────┘
        │  HttpClient    │  HttpClient            │  HttpClient
        ▼                ▼                        ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────────────────┐
│  Core API    │  │ Analytics API│  │         AI API           │
│  :5042       │  │  :5002       │  │         :5003            │
│  (3-Layer)   │  │  (3-Layer)   │  │  Tag Suggestion Service  │
│  JWT + OData │  │  JWT + OData │  │  Learning Cache Service  │
│  SignalR Hub │  │  Excel Export│  │  Gemini API Integration  │
└──────────────┘  └──────────────┘  └──────────────────────────┘
        │                │
        ▼                ▼
   ┌──────────────────────────┐
   │    SQL Server Database   │
   │    FUNewsManagement      │
   └──────────────────────────┘
```

## Project Structure

```
HE186716_DoHuuHoa_A01/
│
├── FUNewsManagement_v2_CoreAPI/               # Core API (Web API)
│   ├── Controllers/
│   │   ├── AuthController.cs                  # Login, Refresh, Logout
│   │   ├── AccountsController.cs              # Account CRUD (Admin, OData)
│   │   ├── CategoriesController.cs            # Category CRUD (Staff, OData)
│   │   ├── TagsController.cs                  # Tag CRUD (Staff, OData)
│   │   ├── NewsController.cs                  # News CRUD + Duplicate + Recommend
│   │   ├── AuditLogsController.cs             # Audit Log viewer (Admin, OData)
│   │   ├── DashboardController.cs             # Dashboard statistics (Admin)
│   │   └── ProfileController.cs               # User profile (self-service)
│   └── Program.cs                             # DI, JWT, OData, SignalR config
│
├── FUNewsManagement_v2_CoreAPI.BusinessLogic/ # Business Logic Layer
│   ├── DTOs/                                  # Data Transfer Objects (18 files)
│   ├── Services/                              # Service interfaces & implementations
│   ├── Hubs/                                  # SignalR NotificationHub
│   ├── Helpers/                               # JWT helpers
│   ├── Mappings/                              # AutoMapper profiles
│   └── Validators/                            # FluentValidation validators
│
├── FUNewsManagement_v2_CoreAPI.DataAccess/    # Data Access Layer
│   ├── Models/                                # Entity models (7 entities)
│   └── Repositories/                          # Repository pattern (14 files)
│
├── FUNewsManagement_v2_AnalyticsAPI/          # Analytics API (Web API)
│   └── Controllers/
│       └── AnalyticsController.cs             # Dashboard, Trending, Export
│
├── FUNewsManagement_v2_AnalyticsAPI.BusinessLogic/
│   ├── DTOs/                                  # Dashboard & Trending DTOs
│   └── Services/                              # Analytics & Excel export service
│
├── FUNewsManagement_v2_AnalyticsAPI.DataAccess/
│   ├── Models/                                # Shared entity models
│   └── Repositories/                          # Data access for analytics
│
├── FUNewsManagement_v2_AIAPI/                 # AI API (Web API)
│   ├── Controllers/
│   │   └── AIController.cs                    # suggest-tags, learn-tags
│   ├── DTOs/                                  # Request/Response DTOs
│   ├── Services/                              # TagSuggestion & LearningCache
│   └── learning_cache.json                    # Persisted learning cache
│
├── FUNewsManagement_v2_FE/                    # Frontend (Razor Pages)
│   ├── Pages/
│   │   ├── Admin/                             # Dashboard, Accounts, AuditLogs
│   │   ├── Staff/                             # News, Categories, Tags, Profile
│   │   ├── Auth/                              # Login/Logout
│   │   ├── News/                              # Public news views
│   │   ├── Api/                               # Internal API proxy controllers
│   │   ├── Offline.cshtml                     # Offline mode page
│   │   └── Shared/                            # Layout, partials
│   ├── Services/
│   │   ├── CoreApiService.cs                  # HttpClient → Core API
│   │   ├── AnalyticsApiService.cs             # HttpClient → Analytics API
│   │   └── LocalCacheService.cs               # Local JSON cache for offline
│   └── Workers/
│       └── DataRefreshWorker.cs               # Background data refresh (6h)
│
├── FUNewsManagement.sql                       # Database script
└── HE186716_DoHuuHoa_A01.slnx                # Solution file
```

## Installation Guide

### 1. System Requirements
- Visual Studio 2022+
- .NET 8.0 SDK
- SQL Server 2019+

### 2. Database Setup
```sql
-- Run the SQL script to create the database
-- File: FUNewsManagement.sql
```

### 3. Configure Connection Strings
**Core API & Analytics API** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "MyCnn": "Server=YOUR_SERVER;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 4. Configure API URLs
**Frontend** (`FUNewsManagement_v2_FE/appsettings.json`):
```json
{
  "CoreApiSettings": {
    "BaseUrl": "http://localhost:5042"
  },
  "AnalyticsApiSettings": {
    "BaseUrl": "http://localhost:5002"
  },
  "AiApiSettings": {
    "BaseUrl": "http://localhost:5003"
  }
}
```

### 5. Run the Application
Start **all 4 projects** (use Multiple Startup Projects in Visual Studio):

```bash
# Core API (port 5042)
cd FUNewsManagement_v2_CoreAPI
dotnet run

# Analytics API (port 5002)
cd FUNewsManagement_v2_AnalyticsAPI
dotnet run

# AI API (port 5003)
cd FUNewsManagement_v2_AIAPI
dotnet run

# Frontend
cd FUNewsManagement_v2_FE
dotnet run
```

## API Endpoints

### Core API (`http://localhost:5042`)

#### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login (returns access_token + refresh_token) |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (revoke refresh token) |

#### Account Management (Admin)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/odata/Accounts` | List accounts (OData: `$filter`, `$orderby`, `$top`, `$skip`, `$count`) |
| GET | `/odata/Accounts({id})` | Get account detail |
| POST | `/api/accounts` | Create new account |
| PUT | `/api/accounts/{id}` | Update account |
| DELETE | `/api/accounts/{id}` | Delete account |

#### Category Management (Staff)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/odata/Categories` | List categories (OData) |
| GET | `/api/categories/{id}` | Get category detail |
| POST | `/api/categories` | Create new category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |
| PATCH | `/api/categories/{id}/status` | Toggle active status |

#### Tag Management (Staff)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/odata/Tags` | List tags (OData) |
| GET | `/api/tags/{id}` | Get tag detail |
| POST | `/api/tags` | Create new tag |
| PUT | `/api/tags/{id}` | Update tag |
| DELETE | `/api/tags/{id}` | Delete tag |

#### News Article
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/news/{id}` | Get article detail (public) |
| POST | `/api/news` | Create new article (multipart/form-data) |
| PUT | `/api/news/{id}` | Update article (multipart/form-data) |
| DELETE | `/api/news/{id}` | Delete article |
| POST | `/api/news/{id}/duplicate` | Duplicate article |
| GET | `/api/recommend/{id}` | Get related articles (up to 3) |

#### Dashboard (Admin)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard/stats` | Get dashboard statistics |

#### Audit Logs (Admin)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/odata/AuditLogs` | List audit logs (OData) |
| GET | `/odata/AuditLogs({id})` | Get audit log detail |
| GET | `/api/auditlogs/filter` | Filter by userId, entity, fromDate, toDate |

#### Profile
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/profile` | Get current user profile |
| PUT | `/api/profile` | Update current user profile |

#### SignalR Hub
| Hub | Endpoint | Description |
|-----|----------|-------------|
| NotificationHub | `/hubs/notifications` | Real-time notifications for new articles |

---

### Analytics API (`http://localhost:5002`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/analytics/dashboard` | Article statistics with OData filtering |
| GET | `/api/analytics/trending` | Trending articles (most-tagged, newest) |
| GET | `/api/analytics/export` | Export Excel report (`?startDate=&endDate=`) |

---

### AI API (`http://localhost:5003`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ai/suggest-tags` | Suggest tags for content (`{ "content": "..." }`) |
| POST | `/api/ai/learn-tags` | Learn user-selected tags for future suggestions |

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

## Key Features (ASS02)

| Feature | Implementation |
|---------|----------------|
| **Distributed Architecture** | 4 independent projects communicating via HttpClient |
| **JWT + Refresh Token** | Access token (60 min) + Refresh token (7 days) |
| **OData Integration** | `$filter`, `$orderby`, `$select`, `$top`, `$skip`, `$count` |
| **Audit Logging** | Records User, Action, Entity, Before/After JSON |
| **SignalR Notifications** | Real-time notifications via `/hubs/notifications` |
| **AI Tag Suggestion** | Google Gemini API + keyword extraction + learning cache |
| **Dashboard & Charts** | Chart.js (Pie, Bar) with OData filtering |
| **Excel Export** | EPPlus-based analytics report (`xlsx`) |
| **Background Worker** | `DataRefreshWorker` refreshes cached data every 6 hours |
| **Offline Mode** | Local JSON cache + "Offline Mode" banner + disabled CRUD |
| **Polly Retry Policy** | Automatic retry for failed API calls |
| **Image Upload** | File type & size validation for article images |

## Design Patterns
- **3-Layer Architecture** (Controller → Business Logic → Data Access)
- **Repository Pattern**
- **DTO Pattern**
- **Singleton Pattern** (DbContext)
- **Strategy Pattern** (Multiple HttpClient services)
- **Observer Pattern** (SignalR real-time notifications)
