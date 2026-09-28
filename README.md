# CrowdScore

A real-time MMA scoring and analytics platform in development. Milestone 2 adds
PostgreSQL persistence, Entity Framework Core, seeded development data, and
read-only event and fight APIs. Scoring, accounts, real-time updates, and
analytics remain later milestones; see [PROJECT_PLAN.md](PROJECT_PLAN.md).

## Prerequisites

- Docker Desktop with Docker Compose.
- .NET 10 SDK.
- Node.js 24 or newer with npm for the existing frontend.
- A modern browser.

The commands below run from the repository root. PowerShell users can run the
same Docker and .NET commands; use `npm.cmd` instead of `npm` for frontend
commands when PowerShell blocks `npm.ps1`.

## Run locally

### 1. Start PostgreSQL

```sh
docker compose up -d postgres
docker compose ps
```

Wait until the `postgres` service reports `healthy`. PostgreSQL listens on
`localhost:5432` and stores data in the `crowdscore-postgres-data` Docker
volume.

### 2. Restore the EF Core tool and apply migrations

```sh
dotnet tool restore
dotnet ef database update --project backend/CrowdScore.Api/CrowdScore.Api.csproj --startup-project backend/CrowdScore.Api/CrowdScore.Api.csproj
```

The migration command creates the `Events`, `Fighters`, and `Fights` tables.
Migrations are explicit: starting the API does not create or update the schema.

To list the applied and available migrations:

```sh
dotnet ef migrations list --project backend/CrowdScore.Api/CrowdScore.Api.csproj --startup-project backend/CrowdScore.Api/CrowdScore.Api.csproj
```

### 3. Run the backend

```sh
dotnet run --project backend/CrowdScore.Api --launch-profile http
```

The API listens on `http://localhost:5000`. In Development, startup inserts one
fictional event, six fighters, and three fights if that event is not already
present. Restarting the API does not duplicate the seed data.

### 4. Verify the API

In another terminal:

```sh
curl -i http://localhost:5000/api/health
curl -i http://localhost:5000/api/events
curl -i http://localhost:5000/api/events/1
curl -i http://localhost:5000/api/fights/1
```

The health response remains:

```json
{"status":"healthy"}
```

On a fresh database, the seeded event and first fight use IDs `1`. To inspect
the seed counts directly in PostgreSQL:

```sh
docker compose exec postgres psql -U crowdscore -d crowdscore -c 'SELECT (SELECT COUNT(*) FROM "Events") AS events, (SELECT COUNT(*) FROM "Fighters") AS fighters, (SELECT COUNT(*) FROM "Fights") AS fights;'
```

### 5. Run the existing frontend

In another terminal:

```sh
cd frontend
cp .env.example .env.local
npm ci
npm run dev
```

Preserve an existing `.env.local`; copy the example only for initial setup.
Open `http://localhost:3000` to see the existing backend health check.

## PostgreSQL lifecycle

Stop and restart PostgreSQL while preserving its data:

```sh
docker compose stop postgres
docker compose start postgres
```

Stop and remove the container while preserving its named volume:

```sh
docker compose down
```

Deleting the named volume erases the local database and is intentionally not
part of the normal workflow.

## Configuration

| Setting | Development value | Purpose |
| --- | --- | --- |
| `ConnectionStrings:CrowdScore` | Local Compose connection | PostgreSQL connection used by EF Core |
| `ConnectionStrings__CrowdScore` | Not set by default | Environment-variable override for the connection |
| `NEXT_PUBLIC_API_BASE_URL` | `http://localhost:5000` | Public API origin used by the frontend |
| `Cors:AllowedOrigins` | `http://localhost:3000` | Browser origin allowed by the API in Development |
| Backend `http` launch profile | `http://localhost:5000` | Local API address and Development environment |

The committed database username and password are for the isolated local Docker
service only. Override the connection string for any other environment. For
example, on macOS/Linux:

```sh
export ConnectionStrings__CrowdScore='Host=localhost;Port=5432;Database=crowdscore;Username=crowdscore;Password=replace-me'
```

In PowerShell:

```powershell
$env:ConnectionStrings__CrowdScore = 'Host=localhost;Port=5432;Database=crowdscore;Username=crowdscore;Password=replace-me'
```

The backend uses standard ASP.NET Core configuration and does not load dotenv
files. Environment variables override JSON configuration. Frontend variables
prefixed with `NEXT_PUBLIC_` are public and embedded in the browser bundle.

## API endpoints

| Method | Route | Response |
| --- | --- | --- |
| `GET` | `/api/health` | Existing service health response |
| `GET` | `/api/events` | Event summaries ordered by date |
| `GET` | `/api/events/{eventId}` | One event with its ordered fight card |
| `GET` | `/api/fights/{fightId}` | One fight with both fighter DTOs |

Unknown integer IDs return `404 Not Found`. Database entities are projected to
API response DTOs and all database reads are asynchronous and no-tracking.

## Validation

```sh
dotnet restore backend/CrowdScore.Api/CrowdScore.Api.csproj
dotnet build backend/CrowdScore.Api/CrowdScore.Api.csproj
npm --prefix frontend run lint
npm --prefix frontend run build
```

No automated backend test project is currently configured.

## Troubleshooting

- **PostgreSQL is not healthy:** run `docker compose logs postgres` and check
  whether another process is using port `5432`.
- **API reports a missing table:** run the EF Core database update command
  before starting the backend.
- **Database authentication fails:** verify that the connection string matches
  the Compose credentials. Existing volumes retain the credentials used when
  they were first created.
- **CORS error:** use `http://localhost:3000`, not `http://127.0.0.1:3000`, and
  run the backend with the `http` launch profile.
- **Frontend API connection fails:** confirm `/api/health` works directly and
  verify `frontend/.env.local` contains
  `NEXT_PUBLIC_API_BASE_URL=http://localhost:5000`.

## Repository structure

```text
frontend/                       Next.js application
backend/CrowdScore.Api/
  Controllers/                 Health, event, and fight HTTP endpoints
  Data/                        EF Core context, migrations, and dev seeder
  DTOs/                        API response contracts
  Models/                      Fighter, event, fight, and fight status
docker-compose.yml             Local PostgreSQL service
.config/dotnet-tools.json      Repository-local EF Core CLI tool
PROJECT_PLAN.md                Architecture and roadmap
```

## License

[MIT](LICENSE)
