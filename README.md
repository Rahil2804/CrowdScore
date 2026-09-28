# CrowdScore

A real-time MMA scoring and analytics platform in development. **Milestone 1**
provides a Next.js frontend connected to an ASP.NET Core health endpoint.
Scoring, accounts, persistence, real-time updates, and analytics arrive in later
milestones; see [PROJECT_PLAN.md](PROJECT_PLAN.md) for the roadmap.

## Prerequisites

- Node.js 24 or newer, with npm (validated with Node 24 and npm 11).
- .NET 10 SDK.
- Two terminals and a modern browser.

## Run locally

Run these commands from the repository root. The examples use PowerShell;
on macOS/Linux, use `npm` instead of `npm.cmd` and `cp` instead of `Copy-Item`.

**Terminal 1 — backend**

```powershell
dotnet run --project backend/CrowdScore.Api --launch-profile http
```

The API listens on `http://localhost:5000`. Open
`http://localhost:5000/api/health` to see:

```json
{"status":"healthy"}
```

**Terminal 2 — frontend**

```powershell
cd frontend
Copy-Item .env.example .env.local
npm.cmd ci
npm.cmd run dev
```

Copy the environment example only on initial setup; preserve an existing
`.env.local`. On subsequent runs, start each application using its run command.
Run `npm.cmd ci` again when the lockfile changes.

Open `http://localhost:3000`. The service card shows `healthy` only after the
browser receives and validates the API response. Requests time out after five
seconds. If the API is unavailable, the card shows an error and a Retry button.
Checks run on page load and retry, without background polling.

Stop each application with **Ctrl+C** in its terminal.

## Configuration

| Setting | Development value | Purpose |
| --- | --- | --- |
| `NEXT_PUBLIC_API_BASE_URL` | `http://localhost:5000` | API origin, set in `frontend/.env.local` |
| `Cors:AllowedOrigins` | `["http://localhost:3000"]` | Browser origins allowed by the API, set in development app settings |
| Backend `http` launch profile | `http://localhost:5000` | Local API address and Development environment |

`NEXT_PUBLIC_` variables are public and embedded in the browser bundle at build
time. Never put secrets in them. Restart the frontend after changing `.env.local`;
rebuild when running the production frontend.

The backend uses standard ASP.NET Core configuration, not dotenv files. An
environment variable such as `Cors__AllowedOrigins__0` overrides the first
configured origin. Base app settings allow no cross-origin browser access;
the localhost origin is enabled by the Development environment. CORS permits
GET requests without credentials and uses exact origins with no trailing slash.

HTTP is used for local development. Deployment and production TLS configuration
are deferred to a later milestone.

### Different ports and troubleshooting

- **API connection fails:** confirm the API is running and open `/api/health`
  directly. Verify `NEXT_PUBLIC_API_BASE_URL` and restart Next.js after changes.
- **CORS error:** use `http://localhost:3000`, not `http://127.0.0.1:3000`.
  Confirm the API uses the `http` launch profile and the Development environment.
- **Port already in use:** stop the conflicting process or deliberately change
  ports. For the API, update the launch profile and frontend environment value.
  For the frontend, update its `dev`/`start` script ports and the backend's allowed
  origin. Restart both applications after changing configuration.
- **Unexpected response:** confirm the configured address points to CrowdScore's
  API. The expected response is HTTP 200 with the JSON shown above.
- **PowerShell blocks npm.ps1:** use the documented `npm.cmd` commands.

## Validation

From the repository root:

```powershell
npm.cmd --prefix frontend run lint
npm.cmd --prefix frontend run build
dotnet build backend/CrowdScore.Api/CrowdScore.Api.csproj
```

The frontend build does not require a running API. To check the production
frontend locally, stop the frontend development server, then run:

```powershell
npm.cmd --prefix frontend run start
```

Keep the API running and open `http://localhost:3000` to verify the connection.
To check recovery, stop the API and reload the page; restart the API and click
Retry. To inspect CORS, request the endpoint with an `Origin` header:

```powershell
curl.exe -i -H "Origin: http://localhost:3000" http://localhost:5000/api/health
curl.exe -i -H "Origin: http://localhost:3001" http://localhost:5000/api/health
```

Only the first response should include `Access-Control-Allow-Origin`. CORS
controls browser access; it does not prevent requests from non-browser clients.
No automated test suites are configured in this milestone.

## Repository structure

```text
frontend/
  app/                  Page, layout, and Tailwind styles
  components/           Client health status UI
  services/             API request and response validation
  types/                Health response contract
backend/
  CrowdScore.Api/
    Controllers/        HTTP endpoints
    DTOs/               API response contracts
    Properties/         Local launch configuration
    Program.cs          Application startup and CORS
PROJECT_PLAN.md          Architecture and roadmap
```

The backend uses the CrowdScore name; the roadmap still contains earlier
FightPulse naming. Only directories needed for implemented code are created.

## License

[MIT](LICENSE)
