# Blog Management API

A RESTful API for managing a blog platform with post management, user authentication, role-based authorization, audit logging, and media uploads. Built with ASP.NET Core and clean layered architecture.

## Features

- **JWT Authentication** - Login, refresh tokens, and secure logout
- **Role-Based Authorization** - Admin and Editor roles with granular permissions
- **Blog Post Management** - Create, update, soft delete, restore, and search posts
- **Pagination & Filtering** - All list endpoints support paging, category filtering, and search
- **Related Posts** - Automatic related post suggestions by category
- **Audit Logging** - Full change trail with user, action, and timestamp
- **Media Uploads** - Image/media file upload support
- **Rate Limiting** - Per-endpoint rate limits to prevent abuse
- **Swagger UI** - Interactive API documentation in development

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB / SQLExpress) |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Mapping | AutoMapper |
| Documentation | Swashbuckle (Swagger/OpenAPI) |

## Project Structure

```
server/
└── BlogManagementApi/
    ├── BlogManagementApi/          # Web API - controllers, middleware, startup
    ├── BlogManagementApi.Domain/   # Entities, enums (Post, User, AuditLog, ...)
    ├── BlogManagementApi.DataAccess/  # EF Core DbContext, repositories, migrations
    ├── BlogManagementApi.Services/    # Business logic services
    ├── BlogManagementApi.DTOs/        # Request/response data transfer objects
    ├── BlogManagementApi.Mappers/     # AutoMapper profiles
    ├── BlogManagementApi.Helpers/     # DI extensions, settings helpers
    └── BlogManagementApi.Shared/      # Shared exceptions and settings
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express (or any SQL Server instance)
- `dotnet ef` CLI tool: `dotnet tool install --global dotnet-ef`

### Installation

1. **Clone the repository**

   ```bash
   git clone <repo-url>
   cd server/BlogManagementApi
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure the application**

   Edit `BlogManagementApi/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLExpress;Database=BlogManagementApp;Trusted_Connection=True;TrustServerCertificate=True"
     },
     "JwtSettings": {
       "SecretKey": "your-secret-key-at-least-32-characters-long",
       "ExpiresInMinutes": 60
     },
     "AdminSeed": {
       "Email": "admin@example.com",
       "Password": "YourPassword123"
     }
   }
   ```

4. **Apply database migrations**

   ```bash
   dotnet ef database update --project BlogManagementApi.DataAccess --startup-project BlogManagementApi
   ```

5. **Run the API**

   ```bash
   dotnet run --project BlogManagementApi
   ```

    The API will start on `https://localhost:7229` (HTTPS) or `http://localhost:5168` (HTTP). Swagger UI is available at
   `https://localhost:7229/swagger` in development.

   An admin account is automatically seeded on first run using the `AdminSeed` config values.

## API Reference

### Auth - `/api/auth`

> Rate limit: 5 requests/minute

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/auth/login` | Public | Login and receive access + refresh tokens |
| `POST` | `/api/auth/register-editor` | Admin | Create a new Editor account |
| `POST` | `/api/auth/refresh` | Public | Exchange a refresh token for a new access token |
| `POST` | `/api/auth/logout` | Bearer | Invalidate the current refresh token |

### Posts - `/api/posts`

> Rate limit: 60 requests/minute

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/posts` | Public | List published posts (paginated, filterable) |
| `GET` | `/api/posts/{slug}` | Public | Get a single post by slug |
| `GET` | `/api/posts/{slug}/related` | Public | Get related posts |
| `GET` | `/api/posts/deleted` | Admin | List soft-deleted posts |
| `POST` | `/api/posts` | Admin / Editor | Create a new post |
| `PUT` | `/api/posts/{id}` | Admin / Editor | Update an existing post |
| `DELETE` | `/api/posts/{id}` | Admin / Editor | Soft delete a post |
| `POST` | `/api/posts/{id}/restore` | Admin | Restore a soft-deleted post |

**Query parameters for `GET /api/posts`:** `page`, `pageSize`, `category`, `search`

### Users - `/api/users`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/users/me` | Bearer | Get the current user's profile |
| `PUT` | `/api/users/me` | Bearer | Update the current user's profile |

### Media - `/api/media`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/media/upload` | Admin / Editor | Upload a media file |

### Admin Users - `/api/admin/users`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/admin/users` | Admin | List all users (paginated) |
| `GET` | `/api/admin/users/{id}` | Admin | Get a user by ID |
| `PUT` | `/api/admin/users/{id}/roles` | Admin | Update a user's roles |
| `DELETE` | `/api/admin/users/{id}` | Admin | Delete a user |

## Authentication

The API uses JWT Bearer tokens with refresh token rotation.

1. **Login** - `POST /api/auth/login` returns an `accessToken` and a `refreshToken`.
2. **Authorize requests** - Include the access token in the `Authorization` header:
   ```
   Authorization: Bearer <accessToken>
   ```
3. **Refresh** - When the access token expires (60 min), call `POST /api/auth/refresh` with the refresh token to get a new pair. Refresh tokens expire after 7 days.
4. **Logout** - Call `POST /api/auth/logout` to invalidate the refresh token server-side.

## Roles

| Permission | Admin | Editor |
|-----------|-------|--------|
| Read published posts | Yes | Yes |
| Create / update posts | Yes | Yes |
| Delete posts (soft) | Yes | Yes |
| Restore deleted posts | Yes | No |
| View deleted posts | Yes | No |
| Register new editors | Yes | No |
| Manage users | Yes | No |
| Upload media | Yes | Yes |

## Configuration Reference

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `JwtSettings:SecretKey` | HMAC-SHA256 signing key (min 32 chars) |
| `JwtSettings:ExpiresInMinutes` | Access token lifetime (default: 60) |
| `AdminSeed:Email` | Email for the seeded admin account |
| `AdminSeed:Password` | Password for the seeded admin account |

> **Production note:** Never commit real secrets to source control. Use environment variables or a secrets manager to override `appsettings.json` values.

## Rate Limiting

| Endpoint Group | Limit | Window |
|----------------|-------|--------|
| `/api/auth/*` | 5 requests | 1 minute (fixed) |
| All other endpoints | 60 requests | 1 minute (sliding) |

## Post Categories

Posts are assigned one of the following categories:

`Blog` · `News` · `Events` · `Announcements` · `Guides` · `Reviews`
