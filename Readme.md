# TaxPrep API

REST API for a tax-preparation management platform supporting client onboarding, service catalog management, and engagement tracking. Built with ASP.NET Core 9 Minimal APIs, Entity Framework Core, and SQL Server.

## Features

- Register and manage clients (name, email, phone)
- Browse a service catalog (Tax Return, Audit Support, Bookkeeping, etc.)
- Create engagements linking clients to services with status tracking
- Filter engagements by client, service, or year
- Structured logging with Serilog (console + rolling file)
- Swagger/OpenAPI documentation at `/swagger`
- Repository/service layer separation (Clean Architecture)
- xUnit test suite with Moq and FluentAssertions; SQLite in-memory for integration tests

## Tech Stack

- **ASP.NET Core 9** — Minimal API
- **Entity Framework Core 9** — SQL Server provider
- **Serilog** — structured logging
- **Swashbuckle** — Swagger UI
- **xUnit** + **Moq** + **FluentAssertions** — testing
- **coverlet** — code coverage

## Prerequisites

- .NET 9 SDK
- SQL Server (local or Docker)

### SQL Server via Docker (quickest)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123" \
  -p 1433:1433 --name taxprep-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

## Setup

```bash
git clone https://github.com/polyglotarist/taxprep_api.git
cd taxprep_api
```

### Connection string

The API resolves the connection string in this order:

1. `appsettings.json` → `ConnectionStrings:Default`
2. Environment variable `ConnectionStrings__Default`
3. Local file `src/TaxPrep.Api/ConnectionString.env` (not committed)
4. Dev fallback: `Server=localhost,1433;Database=TaxPrepDb;User Id=sa;Password=Password123;TrustServerCertificate=True`

For local dev, either set the env var or drop a `ConnectionString.env` file in `src/TaxPrep.Api/`.

### Apply migrations and run

```bash
dotnet ef database update --project src/TaxPrep.Api
dotnet run --project src/TaxPrep.Api
# Swagger UI at http://localhost:5000/swagger
```

## API Endpoints

### Clients

| Method | Path | Description |
|--------|------|-------------|
| GET | `/clients` | List all clients |
| GET | `/clients/{id}` | Get client by ID |
| POST | `/clients` | Register a new client |
| PATCH | `/clients/{id}` | Update client details |
| DELETE | `/clients/{id}` | Delete a client |

### Services

| Method | Path | Description |
|--------|------|-------------|
| GET | `/services` | List all services |
| GET | `/services/{id}` | Get service by ID |
| POST | `/services` | Add a service |

### Engagements

| Method | Path | Description |
|--------|------|-------------|
| GET | `/engagements` | List engagements (filter: `?clientId=`, `?serviceId=`, `?year=`) |
| GET | `/engagements/{id}` | Get engagement by ID |
| POST | `/engagements` | Create an engagement |
| PATCH | `/engagements/{id}` | Update status or notes |
| DELETE | `/engagements/{id}` | Cancel an engagement |

### Engagement Statuses

`Requested` → `InProgress` → `Completed` / `Canceled`

## Running Tests

```bash
dotnet test
```

### With coverage report

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" -reporttypes:Html
open coveragereport/index.html
```

## Data Model

```
Client ||--o{ Engagement : "requests"
Service ||--o{ Engagement : "is for"
```
