# Klacks Marketplace — Comprehensive Documentation

## Overview

The Klacks Marketplace is a standalone **Blazor Server** application for distributing **Language Packages** and **Feature Plugins** for the Klacks ecosystem. It provides a public storefront, user authentication, admin review workflows, and REST APIs for automated integration.

- **URL (HTTPS)**: `https://157.180.42.127:7553`
- **URL (HTTP)**: `http://157.180.42.127:7003`
- **GitHub**: `https://github.com/HeribertG/Klacks.Marketplace`
- **Tech Stack**: ASP.NET Core 10.0, Blazor Server, SQLite, Entity Framework Core

## Architecture

```
Klacks.Marketplace/
├── Api/                          # REST API Controllers & DTOs
│   ├── DTOs/
│   │   ├── PackageListItemDto.cs
│   │   ├── PackageSearchResultDto.cs
│   │   ├── PackageUploadDto.cs
│   │   ├── PluginListItemDto.cs
│   │   ├── PluginSearchResultDto.cs
│   │   └── PluginUploadDto.cs
│   ├── PackagesApiController.cs  # /api/packages
│   └── PluginsApiController.cs   # /api/plugins
├── Constants/
│   ├── AppConstants.cs
│   ├── MarketplaceItemType.cs    # LanguagePackage | FeaturePlugin
│   ├── PackageStatus.cs          # Draft | PendingReview | Published | Rejected
│   └── PluginCategory.cs         # communication | erp | accounting | reporting | integration | other
├── Data/
│   ├── Migrations/
│   └── MarketplaceDbContext.cs
├── Models/
│   ├── LanguagePackage.cs
│   ├── PackageVersion.cs
│   ├── DownloadLog.cs
│   ├── FeaturePlugin.cs
│   ├── FeaturePluginVersion.cs
│   ├── PluginDownloadLog.cs
│   └── User.cs
├── Pages/                        # Blazor Pages
│   ├── Index.razor               # Homepage with tabs (Language Packs | Feature Plugins)
│   ├── PackageDetail.razor       # Language package detail view
│   ├── PluginDetail.razor        # Feature plugin detail view
│   ├── Upload.razor              # Language package upload form
│   ├── UploadPlugin.razor        # Feature plugin upload form
│   ├── MyPackages.razor          # User's packages & plugins (tabbed)
│   ├── AdminDashboard.razor      # Admin review panel (packages + plugins)
│   ├── Login.razor / Register.razor
│   └── _Layout.cshtml            # Master layout (Bootstrap + Font Awesome local)
├── Services/
│   ├── AuthService.cs / IAuthService.cs
│   ├── AdminService.cs / IAdminService.cs
│   ├── PackageService.cs / IPackageService.cs
│   ├── FeaturePluginMarketplaceService.cs / IFeaturePluginMarketplaceService.cs
│   ├── FileValidationService.cs / IFileValidationService.cs
│   └── PluginValidationService.cs / IPluginValidationService.cs
├── Shared/
│   ├── MainLayout.razor / NavMenu.razor
│   ├── PackageCard.razor / PluginCard.razor
│   ├── StatusBadge.razor / CategoryBadge.razor
│   ├── ConfirmDialog.razor / CoverageBar.razor
│   └── PluginCard.razor
├── wwwroot/
│   ├── css/ (site.css, marketplace.css)
│   └── lib/ (bootstrap, fontawesome — self-hosted)
├── Program.cs
└── Klacks.Marketplace.csproj
```

## Database Schema (SQLite)

### Users
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Auto-increment |
| Email | string (unique) | Login email |
| PasswordHash | string | PBKDF2 SHA256, 100k iterations |
| DisplayName | string | Display name |
| IsAdmin | bool | Admin flag |
| CreatedAt | DateTime | UTC |

### LanguagePackage
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Auto-increment |
| Code | string | Language code (e.g. "es", "fr-FR") |
| Name | string | English name |
| DisplayName | string | Native display name |
| SpeechLocale | string | Speech locale (e.g. "es-ES") |
| Version | string | Semver version |
| AuthorId | int (FK) | References User |
| Coverage | double | Translation coverage percentage |
| TranslationCount | int | Number of translation keys |
| Description | string | Package description |
| Status | PackageStatus | Draft/PendingReview/Published/Rejected |
| MinKlacksVersion | string | Minimum Klacks version |
| Downloads | int | Download counter |
| CreatedAt/UpdatedAt | DateTime | UTC timestamps |

### PackageVersion
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Auto-increment |
| PackageId | int (FK) | References LanguagePackage (Cascade) |
| Version | string | Version number |
| ManifestJson | string | Full manifest.json content |
| TranslationsJson | string | All translation key-value pairs |
| DocsJson | string | Optional documentation JSON |
| CountriesJson | string | Optional countries data |
| StatesJson | string | Optional states/cantons data |
| CalendarRulesJson | string | Optional calendar rules |
| ChangeLog | string | Version release notes |
| Status | PackageStatus | Version-level status |
| CreatedAt | DateTime | UTC |

### FeaturePlugin
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Auto-increment |
| Name | string (unique) | Plugin identifier (e.g. "messaging") |
| DisplayName | string | Human-readable name |
| Category | string | Plugin category |
| Version | string | Current version |
| AuthorId | int (FK) | References User |
| Description | string (max 4000) | Plugin description |
| MinKlacksVersion | string | Minimum Klacks version |
| RequiredPermissionsJson | string | JSON array of permissions |
| ProvidedSkillsJson | string | JSON array of skill names |
| Status | PackageStatus | Same enum as language packages |
| Downloads | int | Download counter |
| ReadmeMarkdown | string | Optional README content |
| CreatedAt/UpdatedAt | DateTime | UTC timestamps |

### FeaturePluginVersion
| Column | Type | Description |
|--------|------|-------------|
| Id | int (PK) | Auto-increment |
| PluginId | int (FK) | References FeaturePlugin (Cascade) |
| Version | string | Version number |
| ManifestJson | string | Full manifest.json content |
| I18nJson | string | Combined i18n: {"en": {...}, "de": {...}} |
| ChangeLog | string | Release notes |
| BundleData | byte[] | ZIP archive (manifest + i18n + README) |
| Status | PackageStatus | Version-level status |
| CreatedAt | DateTime | UTC |

### DownloadLog / PluginDownloadLog
Track downloads per package/plugin with IP address and timestamp.

## Authentication & Authorization

- **Cookie-based** authentication (not JWT)
- Cookie name: `MarketplaceAuth`, 7 days expiry, sliding expiration
- Password hashing: PBKDF2 SHA256, 16-byte salt, 32-byte hash, 100k iterations
- **Roles**: Admin (full access) and regular users (upload + view own)
- Admin can approve/reject packages and plugins, manage users

## REST API

### Language Packages (`/api/packages`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/packages?search=&page=&pageSize=` | Public | Search published packages |
| GET | `/api/packages/{code}` | Public | Get package by code |
| GET | `/api/packages/{code}/download` | Public | Download as JSON bundle |
| POST | `/api/packages` | API Key | Upload/update package |

### Feature Plugins (`/api/plugins`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/plugins?search=&category=&page=&pageSize=` | Public | Search published plugins |
| GET | `/api/plugins/{name}` | Public | Get plugin by name |
| GET | `/api/plugins/{name}/download` | Public | Download as ZIP bundle |
| POST | `/api/plugins` | API Key | Upload/update plugin |

### API Key Authentication
Upload endpoints require `X-Api-Key` header matching `ApiSettings:UploadApiKey` in configuration.

## Feature Plugin Manifest Schema

Feature plugins must include a `manifest.json` with this structure:

```json
{
  "name": "messaging",
  "displayName": "Messaging",
  "category": "communication",
  "version": "1.0.0",
  "description": "Send and receive messages via Telegram, WhatsApp, etc.",
  "minKlacksVersion": "1.0.0",
  "requiredPermissions": ["CanViewSettings", "CanEditSettings"],
  "providedSkills": ["send_message", "read_messages"],
  "defaultSettings": {
    "MESSAGE_RETENTION_COUNT": "1000"
  },
  "navigation": {
    "route": "/workplace/messaging",
    "labelKey": "messaging.nav.tooltip",
    "position": 7,
    "viewBox": "0 0 24 24",
    "svgPaths": [{"d": "M...", "opacity": "0.3"}]
  }
}
```

### Required Fields
- `name`, `displayName`, `category`, `version`, `description`, `minKlacksVersion`

### Allowed Categories
`communication`, `erp`, `accounting`, `reporting`, `integration`, `other`

### Plugin Bundle (ZIP)
- `manifest.json` (required)
- `i18n/en.json` (required)
- `i18n/de.json`, `i18n/fr.json`, etc. (optional)
- `README.md` (optional)

## Plugin Validation

### Manifest Validation (`PluginValidationService`)
- Must be valid JSON object
- All required fields must be non-empty strings
- Category must be from allowed list
- Navigation (if present) must have `route` and `labelKey`

### i18n Validation
- Must be JSON object with string values
- Must contain at least one entry

### Bundle Validation
- Must be valid ZIP archive
- Must contain `manifest.json`
- Must contain `i18n/en.json`
- Total size under 10 MB

## Package Status Workflow

```
User uploads → PendingReview → Admin approves → Published
                             → Admin rejects → Rejected

API upload (with key) → Published (auto-approved)
```

## UI Pages

### Index (Homepage)
- Tabs: "Language Packs" | "Feature Plugins"
- Search bar with real-time filtering
- Category dropdown filter (plugins only)
- Paginated grid with PackageCard/PluginCard components

### Plugin Detail (`/plugin/{name}`)
- Display name, category badge, version, author, downloads
- Description, provided skills (badges), required permissions
- README content (if available)
- Download button, version history

### Upload Plugin (`/upload-plugin`)
- Form: name, display name, category (dropdown), version, description, min version, changelog
- File uploads: manifest.json (required), en.json (required), de.json/fr.json (optional), README.md (optional)
- Auto-builds ZIP bundle from uploaded files
- Submits for admin review

### Admin Dashboard (`/admin`)
- Stats cards: pending packages, pending plugins, users
- Pending packages table with approve/reject actions
- Pending plugins table with approve/reject actions
- User management with admin toggle

### My Packages (`/my-packages`)
- Tabs: "Language Packs" | "Feature Plugins"
- Tables showing user's uploads with status

## Klacks.Api Integration

The main Klacks application can install plugins directly from the marketplace:

### MarketplaceClient (`Infrastructure/Services/Plugins/MarketplaceClient.cs`)
- HTTP client calling Marketplace REST API
- `SearchPluginsAsync(search, category)` → browse available plugins
- `DownloadPluginAsync(name)` → download ZIP bundle

### FeaturePluginController Endpoints
- `GET /api/plugins/features/marketplace?search=&category=` — browse marketplace
- `POST /api/plugins/features/{name}/install-from-marketplace` — download, extract, install

### Installation Flow
1. Admin triggers install from marketplace
2. MarketplaceClient downloads ZIP bundle
3. ZIP extracted to `Plugins/Features/{name}/`
4. FeaturePluginService discovers new manifest
5. Plugin installed and available for activation

### Configuration
```json
{
  "LanguagePlugins": {
    "MarketplaceUrl": "http://157.180.42.127:7003"
  }
}
```

## Deployment

### Docker
- Container: `klacks-marketplace`
- Image: `klacksapi-klacks-marketplace`
- Port mapping: `7003:80`
- Volume: `marketplace_data:/app/data` (SQLite DB)
- Network: Connected to both `klacksapi_klacks_network` and `apps_klacks_network`

### GitHub Actions (`deploy.yml`)
On push to `main`:
1. SSH to Hetzner server
2. Pull latest code
3. Build Docker image (`--no-cache`)
4. Force-recreate container
5. Connect to nginx-proxy network (`apps_klacks_network`)
6. Reload nginx config
7. Health check on port 7003

### Nginx Proxy
- HTTPS on port 7553 via `klacks-proxy` container
- Self-hosted Bootstrap + Font Awesome (no CDN dependency)
- CSP-compatible: all assets served from same origin

### Configuration
```json
{
  "ApiSettings": {
    "UploadApiKey": "klacks-marketplace-upload-key"
  },
  "AdminSettings": {
    "SeedAdminEmail": "admin@klacks.app",
    "SeedAdminPassword": "...",
    "SeedAdminDisplayName": "Admin"
  }
}
```

## Key Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `MaxUploadSizeBytes` | 5 MB | Language package upload limit |
| `PluginMaxUploadSizeBytes` | 10 MB | Plugin bundle upload limit |
| `PageSize` | 12 | Items per page |
| `MinPasswordLength` | 8 | Minimum password length |
| `DatabaseFileName` | marketplace.db | SQLite database file |
| `AuthCookieName` | MarketplaceAuth | Authentication cookie |

## Security

- PBKDF2 password hashing with constant-time comparison
- API key for upload endpoints
- Role-based authorization (Admin for review operations)
- Self-hosted assets (no external CDN calls)
- Nginx CSP headers for XSS protection
- Docker network isolation (expose, not ports for internal services)
