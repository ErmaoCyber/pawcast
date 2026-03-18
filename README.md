# PawCast API

A backend service that provides a walkability index based on real-time weather and air quality data.

Built with .NET 8, this project demonstrates modern backend practices including authentication, background jobs, external API integration, and clean architecture.

---

## Features

- JWT Authentication
  - Login endpoint with token generation
  - Role-based authorization (user, ops)

- Walkability Index
  - Current walk score
  - Forecast (next 72 hours)
  - Historical data

- Background Jobs (Hangfire)
  - Scheduled data refresh every 30 minutes
  - Manual trigger via Ops API

- External API Integration
  - Open-Meteo Weather API
  - Open-Meteo Air Quality API

- Resilience
  - Retry policies using Polly

- Swagger / OpenAPI
  - Interactive API testing with JWT support

---

## Architecture

This project follows a layered architecture:

```
PawCast.Api            → Controllers, Middleware, Authentication
PawCast.Application    → Use cases, DTOs, services
PawCast.Domain         → Core business logic
PawCast.Infrastructure → Database, external APIs, repositories
```

---

## Tech Stack

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core with PostgreSQL
- Hangfire for background jobs
- Polly for resilience and retry policies
- JWT Bearer Authentication
- Swagger / Swashbuckle

---

## Authentication

### Login

POST /api/Auth/login

Request:

```json
{
  "username": "opsadmin",
  "password": "ops123"
}
```

Response:

```json
{
  "accessToken": "your-jwt-token"
}
```

---

### Using the Token

In Swagger, click the "Authorize" button and enter:

```
Bearer your-jwt-token
```

---

## API Endpoints

### Walk Index

- GET /api/WalkIndex/current
- GET /api/WalkIndex/forecast
- GET /api/WalkIndex/history

### Ops (requires "ops" role)

- POST /api/Ops/refresh  
  Triggers a background refresh job

- GET /api/Ops/fetch-runs  
  Returns recent background job execution results

### Health

- GET /api/Health

---

## Background Jobs

- Runs every 30 minutes
- Fetches weather and air quality data
- Calculates and stores walkability index

Implemented using Hangfire with PostgreSQL storage.

---

## Configuration

Update appsettings.json:

```json
"ConnectionStrings": {
  "PawCastDb": "Host=localhost;Port=5433;Database=pawcast;Username=...;Password=..."
}
```

---

## Running Locally

```bash
dotnet run
```

By default, the local development profiles use:

- http://localhost:5051
- https://localhost:7066

Swagger is available at:

- http://localhost:5051/swagger
- https://localhost:7066/swagger

---

## Key Design Decisions

- Used JWT for stateless authentication
- Applied layered architecture to separate concerns
- Offloaded data fetching and processing to background jobs
- Added retry policies for external API reliability
- Implemented role-based authorization for operational endpoints

---

## Future Improvements

- Add caching (e.g., Redis)
- Introduce rate limiting
- Improve test coverage
- Add CI/CD pipeline
- Deploy to a cloud platform (Azure or AWS)

---
