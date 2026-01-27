# Employee Authentication & Profile Service

A full-stack assessment project demonstrating:

-   JWT-secured ASP.NET Core Web API
-   MediatR-based CQRS architecture
-   Password generation & policy enforcement
-   Email notifications
-   Logging & configuration via Options pattern
-   Unit & integration testing
-   Two independent frontend clients:
    -   Next.js + Tailwind
    -   Blazor Server

------------------------------------------------------------------------

## 🏗️ Solution Architecture

EmployeeAuth.Api\
EmployeeAuth.Application\
EmployeeAuth.Domain\
EmployeeAuth.Infrastructure\
EmployeeAuth.Tests

employee-auth-ui (Next.js frontend)\
EmployeeAuth.Blazor (Blazor Server frontend)

### Backend Layers

  Project          Responsibility
  ---------------- -----------------------------
  Domain           Entities & options
  Application      Commands, queries, handlers
  Infrastructure   EF Core, email, security
  Api              Controllers & startup
  Tests            Unit + integration tests

------------------------------------------------------------------------

## 🚀 Features

### Backend

-   User registration with system-generated password
-   Email notification on account creation
-   Login by username or email
-   JWT authentication
-   Profile update
-   Password change
-   Password policy validation
-   Logging via ILogger`<T>`{=html}
-   Swagger UI
-   CORS support
-   SQL Server / SQLite test database
-   Strategy pattern for login lookup
-   Options pattern for configuration

### Frontends

#### Next.js

-   React + Tailwind
-   JWT auth
-   Modal registration form
-   Protected profile page
-   API service abstraction

#### Blazor Server

-   Razor components
-   TokenStore service
-   HttpClient JWT handler
-   Registration modal
-   Profile editing
-   Password visibility toggles
-   Redirect guards

------------------------------------------------------------------------

## 📐 Architectural Decisions

### MediatR / CQRS

All business logic is implemented through commands and queries.

Controllers remain thin and delegate to handlers.

Benefits:

-   Clear separation of concerns
-   Highly testable
-   No business logic in controllers
-   Easy extensibility

------------------------------------------------------------------------

### Strategy Pattern

Login supports username or email via strategies.

Avoids conditionals and supports extension.

------------------------------------------------------------------------

### API-First Design

Both frontends communicate only through HTTP.

Benefits:

-   Independent UI development
-   Swagger-driven testing
-   Easy addition of mobile apps
-   Clean separation of concerns

------------------------------------------------------------------------

### Dependency Injection

All infrastructure is abstracted via interfaces.

Enables mocking and test isolation.

------------------------------------------------------------------------

## 🧪 Testing Strategy

### Unit Tests

-   Handler tests
-   Password policy validation
-   Login lookup strategies
-   Hashing services

Tools:

-   xUnit
-   Moq
-   FluentAssertions
-   SQLite in-memory EF Core

Run:

dotnet test EmployeeAuth.Tests

------------------------------------------------------------------------

### Integration Tests

End-to-end API tests using WebApplicationFactory.

Scenarios:

-   Register → Login → Profile
-   Unauthorized access blocked
-   Password update flow

------------------------------------------------------------------------

## ▶️ Running the System

### Backend API

cd EmployeeAuth.Api\
dotnet run

Swagger available at /swagger.

------------------------------------------------------------------------

### Next.js Client

cd employee-auth-ui\
npm install\
npm run dev

------------------------------------------------------------------------

### Blazor Server Client

cd EmployeeAuth.Blazor\
dotnet run

------------------------------------------------------------------------

## ⚙️ Configuration

Backend configuration lives in:

-   appsettings.json
-   appsettings.Development.json

Includes:

-   JWT options
-   Password policy
-   Email provider
-   Connection strings

------------------------------------------------------------------------

## 📌 Future Improvements

-   Refresh tokens
-   Role-based authorization
-   Password reset via email
-   Email queueing
-   Audit logging
-   Rate limiting
-   Docker support
-   CI/CD pipeline

------------------------------------------------------------------------

## 🧠 Summary

This project demonstrates:

-   enterprise-style backend layering
-   API-first design
-   test-driven development
-   extensible authentication strategies
-   multiple UI clients on a single backend
-   modern .NET practices
