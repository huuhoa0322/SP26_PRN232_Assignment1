# FU News Management System

## Project Information
- **Student**: Do Huu Hoa (HE186716)
- **Class Code**: SE1884-NET
- **Course**: PRN232 - Building Cross-Platform Back-End Application With .NET
- **Assignment**: 01

## Technologies Used
- **.NET 8.0**
- **ASP.NET Core Web API** (Backend)
- **ASP.NET Core Razor Pages** (Frontend)
- **Entity Framework Core** (ORM)
- **SQL Server** (Database)
- **Bootstrap 5** (UI Framework)

## Project Structure

```
HE186716_DoHuuHoa_A01/
├── HE186716_DoHuuHoa_SE1884-NET_A01_BE/  # Backend API
│   ├── Controllers/                       # API Controllers
│   ├── DTOs/                              # Data Transfer Objects
│   ├── Models/                            # Entity Models
│   ├── Repositories/                      # Data Access Layer
│   └── Services/                          # Business Logic Layer
│
└── HE186716_DoHuuHoa_SE1884-NET_A01_FE/  # Frontend Web App
    ├── Pages/                             # Razor Pages
    │   ├── Admin/                         # Admin pages
    │   ├── Auth/                          # Login/Logout
    │   ├── News/                          # News views
    │   ├── Staff/                         # Staff pages
    │   └── Shared/                        # Layout, partials
    ├── Models/                            # Frontend DTOs
    └── Services/                          # API Service calls
```

## Installation Guide

### 1. System Requirements
- Visual Studio 2026
- .NET 8.0 SDK
- SQL Server 2025

### 2. Configure Connection String
**Backend** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "MyCnn": "Server=YOUR_SERVER;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3. Run the Application

**Backend:**
```bash
cd HE186716_DoHuuHoa_SE1884-NET_A01_BE
dotnet run
# API: http://localhost:xxxx (base on your port)
```

**Frontend:**
```bash
cd HE186716_DoHuuHoa_SE1884-NET_A01_FE
dotnet run
# Web: http://localhost:xxxx (base on your port)
```

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login |

### Account (Admin)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/account` | Get all accounts |
| POST | `/api/account` | Create new account |
| PUT | `/api/account/{id}` | Update account |
| DELETE | `/api/account/{id}` | Delete account |
| PUT | `/api/account/{id}/change-password` | Change password |

### Category (Staff)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/category` | Get all categories |
| POST | `/api/category` | Create new category |
| PUT | `/api/category/{id}` | Update category |
| DELETE | `/api/category/{id}` | Delete category |

### News Article
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/news` | Get active news (public) |
| GET | `/api/news/all` | Get all news |
| POST | `/api/news` | Create new article |
| PUT | `/api/news/{id}` | Update article |
| DELETE | `/api/news/{id}` | Delete article |
| POST | `/api/news/{id}/duplicate` | Duplicate article |

### Tag
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tag` | Get all tags |
| POST | `/api/tag` | Create new tag |
| PUT | `/api/tag/{id}` | Update tag |
| DELETE | `/api/tag/{id}` | Delete tag |

### Report (Admin)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/report/statistics` | Get statistics |
| GET | `/api/report/export` | Export to CSV |

## Test Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@FUNewsManagementSystem.org` | `@@abc123@@` |
| Staff | *(see database)* | *(see database)* |

## Authorization

| Role | Permissions |
|------|-------------|
| **Admin** | Manage Accounts, View Reports |
| **Staff** | Manage Categories, Articles, Tags |
| **Lecturer** | View news articles |
| **Guest** | View active news articles |

## Design Patterns
- 3-Layer Architecture
- Repository Pattern
- DTO Pattern
- Singleton Pattern (DbContext)
