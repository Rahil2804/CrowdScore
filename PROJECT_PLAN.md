# MMA Live Scoring Platform — Project Plan

> Working title: **CrowdScore**  
> A real-time MMA companion app where fans can score fights round-by-round, compare their scorecards with the community, discuss fights live, review fight statistics, and receive AI-generated round analysis.

---

## 1. Project Goal

Build a production-style full-stack application that demonstrates:

- Modern frontend development with Next.js, React, and TypeScript
- Backend API development with C# and ASP.NET Core
- Real-time communication with SignalR
- Relational data modeling with PostgreSQL and Entity Framework Core
- Authentication and authorization
- Real-time community score aggregation
- Live fight comments
- Fight-stat ingestion through a provider abstraction
- AI analysis grounded in structured fight statistics
- Redis caching and real-time scale-out
- Docker, automated testing, CI/CD, and cloud deployment
- Social sharing of user scorecards to X/Twitter

The project should feel like a real product rather than a basic CRUD application.

---

# 2. Core Product Experience

A user should be able to:

1. View current and upcoming MMA events.
2. Open an individual fight.
3. Score each round using MMA-style scoring such as 10-9 or 10-8.
4. See how the community scored the same round.
5. Watch community percentages update in real time.
6. Participate in a live fight discussion.
7. View round-by-round fight statistics.
8. Read an AI-generated analysis based only on available fight statistics.
9. Save their complete scorecard to their account.
10. Compare their final score with the community and official judges when available.
11. Share their scorecard or individual round score directly to X/Twitter.

---

# 3. Recommended Technology Stack

## Frontend

- **Next.js**
- **React**
- **TypeScript**
- **Tailwind CSS**
- **Recharts**
- SignalR JavaScript client

## Backend

- **C#**
- **.NET 10**
- **ASP.NET Core Web API**
- **SignalR**
- **Entity Framework Core**

## Data

- **PostgreSQL**
- **Redis** later in development

## AI

- **OpenAI API**
- AI requests made only through the ASP.NET backend
- Structured responses for predictable output

## Authentication

Initial recommendation:

- ASP.NET Core Identity
- JWT-based authentication

## Testing

Backend:

- xUnit
- ASP.NET integration tests

Frontend:

- Vitest
- React Testing Library
- Playwright

## Infrastructure

- Docker
- Docker Compose
- GitHub Actions
- Azure for the backend
- Managed PostgreSQL
- Managed Redis
- Vercel or Azure for the Next.js frontend

---

# 4. Repository Structure

```text
fightpulse/

├── frontend/
│   ├── app/
│   ├── components/
│   ├── hooks/
│   ├── lib/
│   ├── services/
│   └── types/
│
├── backend/
│   └── FightPulse.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── DTOs/
│       ├── Hubs/
│       ├── Models/
│       ├── Providers/
│       ├── Services/
│       └── Program.cs
│
├── tests/
├── docker-compose.yml
├── .gitignore
├── LICENSE
├── README.md
└── PROJECT_PLAN.md
```

Do not over-engineer the backend into multiple projects immediately. Start with a well-organized ASP.NET Core API and split it later only if the application becomes large enough to justify it.

---

# 5. High-Level Architecture

```text
                          USERS
                            │
                            ▼
                    Next.js / React
                       TypeScript
                            │
                    REST + SignalR
                            │
                            ▼
                   ASP.NET Core API
                        .NET 10
                            │
            ┌───────────────┼────────────────┐
            │               │                │
            ▼               ▼                ▼
       PostgreSQL         Redis           OpenAI
         EF Core       Cache / PubSub       API
            │
            ▼
       Fight Data Layer
            │
      ┌─────┴───────────┐
      ▼                 ▼
Mock Provider      Live Data Provider
```

---

# 6. Initial Database Models

Start with:

```text
User
Fighter
Event
Fight
Scorecard
RoundScore
Comment
```

Add later:

```text
RoundStatistics
FightStatistics
AIAnalysis
OfficialScorecard
```

## Relationships

```text
User
 ├── Scorecards
 └── Comments

Event
 └── Fights

Fight
 ├── Fighter A
 ├── Fighter B
 ├── Scorecards
 ├── Comments
 ├── RoundStatistics
 └── AIAnalysis

Scorecard
 └── RoundScores
```

---

# 7. Development Roadmap

## Milestone 1 — Repository and Base Applications

### Tasks

- [ ] Create public GitHub repository
- [ ] Add MIT License
- [ ] Add `.gitignore`
- [ ] Create `frontend/`
- [ ] Create Next.js + TypeScript app
- [ ] Add Tailwind CSS
- [ ] Create `backend/`
- [ ] Create .NET 10 ASP.NET Core Web API
- [ ] Add `/api/health`
- [ ] Connect frontend to `/api/health`

### Definition of Done

Opening the frontend successfully displays data returned by the .NET backend.

---

## Milestone 2 — PostgreSQL and Entity Framework Core

### Tasks

- [ ] Set up PostgreSQL locally
- [ ] Install Entity Framework Core
- [ ] Configure database connection
- [ ] Create initial models:
  - [ ] Fighter
  - [ ] Event
  - [ ] Fight
- [ ] Create EF Core migration
- [ ] Seed fake UFC-style event data
- [ ] Create event API endpoints
- [ ] Create fight API endpoints

### Initial Endpoints

```http
GET /api/events
GET /api/events/{eventId}
GET /api/fights/{fightId}
```

### Definition of Done

The frontend loads an event and its fights from PostgreSQL through the .NET API.

---

## Milestone 3 — Event and Fight UI

### Pages

```text
/
 /events
 /events/[eventId]
 /fights/[fightId]
```

### Fight Page Should Initially Display

- Fighter A
- Fighter B
- Scheduled rounds
- Fight status
- Current round
- Placeholder stats
- Scoring interface
- Community score placeholder
- Chat placeholder
- AI analysis placeholder

### Definition of Done

A fake event looks and feels like a real live-fight page even though the data is still simulated.

---

## Milestone 4 — Round Scoring

Create:

```text
Scorecard
RoundScore
```

### Example Request

```http
POST /api/fights/{fightId}/rounds/1/scores
```

```json
{
  "fighterAScore": 10,
  "fighterBScore": 9
}
```

### Backend Validation

Validate:

- Fight exists
- Round exists
- Round number is valid
- Score combination is allowed
- User cannot accidentally create duplicate scores for the same round
- A score can be edited if we intentionally support edits

### Initial Score Options

Support:

```text
10-9
9-10
10-8
8-10
10-10
```

Other rare score combinations can be added later.

### Definition of Done

A user can submit a score and retrieve their scorecard.

---

# 8. Community Scoring

Create an endpoint such as:

```http
GET /api/fights/{fightId}/rounds/{round}/community-score
```

Example response:

```json
{
  "totalVotes": 1382,
  "fighterA": {
    "votes": 940,
    "percentage": 68.02
  },
  "fighterB": {
    "votes": 421,
    "percentage": 30.46
  },
  "draw": {
    "votes": 21,
    "percentage": 1.52
  }
}
```

Display something like:

```text
ROUND 1 COMMUNITY SCORE

Fighter A  █████████████████ 68%
Fighter B  ████████          30%
Draw       █                  2%

1,382 scorecards submitted
```

### Definition of Done

Community scoring works correctly using normal HTTP requests before real-time functionality is added.

---

# 9. SignalR — Real-Time Community Scores

Create:

```text
Hubs/FightHub.cs
```

Users viewing a fight join a SignalR group:

```text
fight-{fightId}
```

When someone submits a score:

```text
Score submitted
      ↓
ASP.NET validates it
      ↓
PostgreSQL updated
      ↓
Community score recalculated
      ↓
SignalR broadcasts update
      ↓
All viewers update immediately
```

### SignalR Events

Start with:

```text
CommunityScoreUpdated
RoundUpdated
FightStatusUpdated
```

### Definition of Done

Open the fight in two browser windows. Submit a score in one window and see the other update without refreshing.

---

# 10. Live Fight Comments

Add:

```text
Comment
```

### API

```http
GET  /api/fights/{fightId}/comments
POST /api/fights/{fightId}/comments
```

### SignalR Event

```text
CommentCreated
```

Flow:

```text
User posts comment
      ↓
Backend stores it
      ↓
SignalR broadcasts it
      ↓
Everyone on the fight page sees it
```

Later add:

- Rate limiting
- Deleted comments
- Reporting
- Basic moderation
- Slow mode if necessary

---

# 11. Authentication

Add:

- ASP.NET Core Identity
- JWT authentication

### Users Should Be Able To

- Register
- Log in
- Log out
- View their profile
- Save scorecards
- Submit scores
- Post comments

### Public Features

Users should not need an account just to:

- Browse events
- View fights
- View community scoring
- View fight statistics
- View AI summaries

---

# 12. Mock Fight Data Provider

Do **not** depend on a live sports-data provider during development.

Create:

```text
Providers/
├── IFightDataProvider.cs
└── MockFightDataProvider.cs
```

Example:

```csharp
public interface IFightDataProvider
{
    Task<FightStatsDto> GetFightStatsAsync(
        string externalFightId
    );
}
```

Example mock data:

```json
{
  "round": 2,
  "fighterA": {
    "significantStrikesLanded": 31,
    "significantStrikesAttempted": 52,
    "takedownsLanded": 2,
    "takedownsAttempted": 3,
    "controlSeconds": 92
  },
  "fighterB": {
    "significantStrikesLanded": 19,
    "significantStrikesAttempted": 44,
    "takedownsLanded": 0,
    "takedownsAttempted": 2,
    "controlSeconds": 28
  }
}
```

This abstraction allows us to swap providers later without rewriting the application.

---

# 13. Fight Simulator

Build a development-only fight simulator.

Example:

```text
Round 1 starts

00:30
A: 5 significant strikes
B: 3

01:00
A: 9
B: 7

02:00
A: 15
B: 11

05:00
A: 28
B: 19

Round 1 ends
```

Broadcast stats through SignalR:

```text
FightStatsUpdated
```

The simulator gives us:

- Reliable development data
- Automated testing data
- A live demo for recruiters
- No dependency on an actual Saturday UFC event

---

# 14. Fight Statistics UI

Show:

- Significant strikes landed / attempted
- Total strikes
- Takedowns landed / attempted
- Takedown defense
- Control time
- Knockdowns
- Submission attempts when available
- Round-by-round breakdown

Example:

| Metric | Fighter A | Fighter B |
| --- | ---: | ---: |
| Significant strikes | 31 / 52 | 19 / 44 |
| Takedowns | 2 / 3 | 0 / 2 |
| Control time | 1:32 | 0:28 |
| Knockdowns | 1 | 0 |

Use Recharts for useful visualizations.

---

# 15. AI Round Analysis

Only implement AI **after the statistics system is working**.

## Flow

```text
Round ends
    ↓
Round statistics finalized
    ↓
ASP.NET backend
    ↓
OpenAI API
    ↓
Structured analysis
    ↓
Save result
    ↓
SignalR broadcast
    ↓
Frontend displays analysis
```

## Important Rule

The AI should analyze only the supplied statistics.

It should not invent:

- Strikes
- Damage
- Takedowns
- Control time
- Events that are not present in the data

Example output:

```json
{
  "summary": "Fighter A produced the stronger statistical round...",
  "keyFactors": [
    "Higher significant-strike output",
    "Two successful takedowns",
    "Control-time advantage"
  ]
}
```

Keep the API key exclusively in the backend.

---

# 16. Share Scorecard to X / Twitter

This should be part of the MVP or first major release because MMA fans already discuss round scores heavily during live fights.

## User Experience

After submitting a round, show:

```text
[ Share on X ]
```

Also show a share button on the completed scorecard.

### Example Round Share

```text
Alex Pereira vs Magomed Ankalaev

Round 2: Pereira 10-9

My scorecard:
R1: Pereira 10-9
R2: Pereira 10-9

Score: Pereira 20-18

Score the fight on FightPulse:
[fight-link]

#UFC #FightPulse
```

The actual fighter names and score should be generated dynamically.

## Implementation

For the first version, **do not integrate the full X API**.

Instead:

1. Generate the share text on the client.
2. Generate a public URL to the FightPulse fight page.
3. Open X/Twitter's share/compose flow with the score text prefilled.
4. Let the user review the post and publish it from their own X account.

This avoids needing permission to post on a user's behalf.

Pseudo-flow:

```text
User clicks Share on X
        ↓
Frontend generates text
        ↓
Score + fighters + FightPulse URL
        ↓
X compose window opens
        ↓
User chooses whether to post
```

## Share Data

Create a helper such as:

```text
buildRoundShareText()
buildScorecardShareText()
```

Input:

```json
{
  "fighterA": "Fighter A",
  "fighterB": "Fighter B",
  "round": 2,
  "fighterAScore": 10,
  "fighterBScore": 9,
  "totalScoreA": 20,
  "totalScoreB": 18,
  "fightUrl": "https://..."
}
```

## Future Upgrade — Share Card Image

A particularly strong feature would be generating a social card:

```text
┌─────────────────────────────────────────┐
│               FIGHTPULSE                │
│                                         │
│      PEREIRA              ANKALAEV      │
│                                         │
│                ROUND 3                  │
│                                         │
│            MY SCORECARD                 │
│                                         │
│     R1  10-9                            │
│     R2  10-9                            │
│     R3   9-10                           │
│                                         │
│             29 - 28                     │
│                                         │
│        Community: 28 - 29               │
└─────────────────────────────────────────┘
```

Users could save/share the image alongside their post.

Potential implementation options later:

- Server-generated Open Graph image
- Next.js image generation
- HTML-to-image card
- Dynamic scorecard preview URL

This gives the project a viral/social component and makes demos much more visually interesting.

---

# 17. Real Fight Data Provider

Once everything works using the simulator:

```text
IFightDataProvider
├── MockFightDataProvider
└── LiveFightDataProvider
```

Possible providers can be evaluated based on:

- Round-by-round update speed
- MMA/UFC coverage
- Developer pricing
- API limits
- Historical statistics
- Licensing restrictions

The rest of the application should not know which provider is currently active.

Use configuration:

```env
FIGHT_DATA_PROVIDER=Mock
```

or:

```env
FIGHT_DATA_PROVIDER=Live
```

---

# 18. Redis

Do not add Redis immediately.

Add it after the application works with PostgreSQL.

Potential Redis uses:

- Community vote counts
- Current fight statistics
- Live viewer count
- Frequently requested fight state
- Rate limiting
- SignalR scale-out / pub-sub

Example keys:

```text
fight:{fightId}:round:{round}:votes
fight:{fightId}:stats
fight:{fightId}:viewers
```

---

# 19. User Profiles

Profile page:

```text
Rahil's FightPulse Profile

Fights scored: 47
Rounds scored: 126
Community agreement: 82%

Recent scorecards
────────────────────────
Fight A vs Fight B
29-28

Fight C vs Fight D
48-47
```

Possible future metrics:

- Community agreement rate
- Agreement with official judges
- Most-scored fighters
- Number of events participated in
- Scoring streak

Avoid presenting these as proof that a user is objectively a "better judge"; treat them as comparison statistics.

---

# 20. Official Scorecard Comparison

When official results become available:

```text
                YOU       COMMUNITY       OFFICIAL

Round 1         10-9         10-9           10-9
Round 2          9-10         9-10           9-10
Round 3         10-9          9-10          10-9
```

This can become one of the app's most interesting post-fight features.

---

# 21. Visualizations

Potential charts:

- Significant strikes by round
- Takedowns by round
- Community scoring percentages
- Score changes throughout the fight
- Community vote count over time

Do not add charts just for decoration. Each chart should answer a meaningful question.

---

# 22. Docker

Eventually run the local stack with:

```bash
docker compose up
```

Services:

```text
frontend
backend
postgres
redis
```

Use environment variables for secrets.

Never commit:

- Database passwords
- JWT secrets
- OpenAI API keys
- Sports-data API keys

Commit:

```text
.env.example
```

instead.

---

# 23. Testing

## Backend Unit Tests

Test:

- Valid round scoring
- Invalid score combinations
- Score editing
- Community vote calculations
- Fight-state validation
- AI response parsing
- Fight-provider failures

## Backend Integration Tests

Test:

```text
POST score
    ↓
database updated
    ↓
community score recalculated
```

## Frontend Tests

Test:

- Scoring UI
- Community-score rendering
- Connection-state UI
- Comment submission
- Share-text generation

## End-to-End Test

```text
Register / Log in
        ↓
Open fight
        ↓
Submit score
        ↓
Community updates
        ↓
Post comment
        ↓
Complete scorecard
        ↓
Open Share on X flow
```

---

# 24. GitHub Actions / CI

For every pull request:

```text
Frontend lint
      ↓
Frontend tests
      ↓
.NET restore
      ↓
.NET build
      ↓
.NET tests
      ↓
Docker build
```

Do not deploy if tests fail.

---

# 25. Deployment

Suggested final architecture:

```text
                   Vercel
                     │
                  Next.js
                     │
                     ▼
              Azure App Service
                     │
               ASP.NET Core
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
     PostgreSQL    Redis      OpenAI
```

A full-Azure deployment is also an option.

---

# 26. Development Order

Follow this order rather than jumping between features:

- [ ] 1. Create GitHub repository
- [ ] 2. Create Next.js frontend
- [ ] 3. Create .NET 10 backend
- [ ] 4. Connect frontend to backend
- [ ] 5. Add PostgreSQL + EF Core
- [ ] 6. Create Event / Fighter / Fight models
- [ ] 7. Seed fake event data
- [ ] 8. Build event and fight pages
- [ ] 9. Create Scorecard / RoundScore models
- [ ] 10. Build round scoring API
- [ ] 11. Build scoring UI
- [ ] 12. Calculate community scores
- [ ] 13. Add SignalR
- [ ] 14. Make community scoring real-time
- [ ] 15. Add live comments
- [ ] 16. Add authentication
- [ ] 17. Add Share on X functionality
- [ ] 18. Build mock fight-data provider
- [ ] 19. Build fight simulator
- [ ] 20. Display live fight statistics
- [ ] 21. Add AI round analysis
- [ ] 22. Connect a real fight-data provider
- [ ] 23. Add Redis
- [ ] 24. Add charts and analytics
- [ ] 25. Add official-scorecard comparisons
- [ ] 26. Add user-profile analytics
- [ ] 27. Add automated tests
- [ ] 28. Dockerize the stack
- [ ] 29. Configure GitHub Actions
- [ ] 30. Deploy
- [ ] 31. Create polished README
- [ ] 32. Add architecture diagram
- [ ] 33. Record demo video

---

# 27. MVP Definition

The first publicly demoable release should include:

- [ ] Event page
- [ ] Fight page
- [ ] User accounts
- [ ] Round scoring
- [ ] Community score
- [ ] Real-time SignalR updates
- [ ] Live comments
- [ ] Mock/live statistics
- [ ] AI round summary
- [ ] Saved scorecards
- [ ] Share scorecard to X
- [ ] Responsive UI
- [ ] Public deployment

Redis, advanced analytics, official-judge comparisons, and social share images can come immediately after the MVP if necessary.

---

# 28. Resume-Level Goals

The final project should give us legitimate experience discussing:

- API design
- Relational database modeling
- Entity Framework Core
- Authentication
- Real-time WebSocket communication
- SignalR groups
- Event-driven UI updates
- Caching
- Redis
- Concurrency
- Rate limiting
- External API integration
- Provider abstractions
- AI grounding
- Structured AI outputs
- Testing
- Docker
- CI/CD
- Azure
- System scaling
- Failure handling
- Product design

The goal is not simply to say:

> "Built a UFC scoring website."

The stronger engineering story is:

> Built a real-time MMA scoring and analytics platform using Next.js and ASP.NET Core, with SignalR-powered community scorecards and live discussion, PostgreSQL persistence, provider-agnostic fight-stat ingestion, AI-generated round analysis grounded in structured fight data, social scorecard sharing, and cloud deployment.

---

# 29. Features to Avoid Until the Core Product Works

Do not initially spend time on:

- Betting odds
- Fantasy MMA
- Fight predictions
- Native mobile apps
- Computer vision
- Machine-learning winner prediction
- Complex microservices
- Kubernetes
- Recommendation systems
- Fighter social feeds

These can distract from finishing the stronger core system.

---

# 30. Final Product Vision

FightPulse should eventually answer four questions during a fight:

1. **How did I score the round?**
2. **How did everyone else score it?**
3. **What actually happened statistically?**
4. **Why might the round have been scored that way?**

Then make the result easy to share with the wider MMA community through X/Twitter.
