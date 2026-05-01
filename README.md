# Task Management Backend API

This project is a .NET 8 backend API built with a DDD-style layered structure:

- `TaskManagement` (Presentation/API)
- `Application` (DTOs + service contracts)
- `Domain` (entities + enums + constants)
- `MyProject.Infrastructure` (EF Core, repositories, services, Redis cache, background worker)

## Tech Stack

- ASP.NET Core Web API (.NET 8)
- SQL Server (LocalDB connection by default)
- JWT Authentication + Role-based Authorization
- Swagger/OpenAPI with Bearer support
- Redis (cache for task-by-id endpoint)
- BackgroundService + in-memory queue for task processing simulation

## Functional Coverage

- User registration and login
- Current user profile endpoint
- Seeded admin user on first app start
- Admin-only user management (create, list, delete)
- Task create/get/list/update-status endpoints
- Ownership rules (users can only access their own tasks)
- Task sorting by priority then created date
- Duplicate task prevention (same title, same day, same user)
- Redis cache invalidation on task update
- Background processing triggered on task creation

## Setup Instructions

1. Ensure SQL Server LocalDB is available (or change `DefaultConnection` in `TaskManagement/appsettings.json`).
2. Start Redis using Docker:
   - `docker compose up -d redis`
   - (from workspace root where `docker-compose.yml` exists)
3. Ensure Redis is reachable at `localhost:6379` (or change `Redis` connection string).
4. Restore/build:
   - `dotnet restore`
   - `dotnet build TaskManagement.sln`
5. Run API:
   - `dotnet run --project TaskManagement/TaskManagement.csproj`
6. Open Swagger:
   - `https://localhost:<port>/swagger`

## Seeded Admin Credentials

- Email: `admin@example.com`
- Password: `Admin@123`

## Assumptions

- A task belongs to exactly one user.
- Admin users cannot be deleted via admin API.
- Background processing simulation marks newly created tasks as `InProgress` after a short delay.
- SQL schema is created automatically on startup via `EnsureCreated`.

## API Notes

- Use `/api/auth/login` to get JWT.
- In Swagger, click **Authorize** and pass token as:
  - `Bearer <your-jwt-token>`

