# Employee Authentication & Profile Service

A full-stack assessment project demonstrating:

-   JWT-secured ASP.NET Core Web API
-   MediatR-based CQRS architecture
-   Password generation & policy enforcement
-   Email notifications
-   Logging & configuration via Options pattern
-   Unit & integration testing
-   Three independent frontend clients:
    -   Vue 3 + Vuetify
    -   Next.js + Tailwind
    -   Blazor Server

------------------------------------------------------------------------

## Solution Architecture

EmployeeAuth.Api\
EmployeeAuth.Application\
EmployeeAuth.Domain\
EmployeeAuth.Infrastructure\
EmployeeAuth.Tests

employee-auth-vue (Vue 3 frontend)\
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

## Features

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

#### Vue 3 + Vuetify

-   Vue 3 (Composition API) + TypeScript
-   Vuetify 3 component library
-   JWT authentication
-   Login page
-   Profile page
-   Password update
-   Logout flow
-   Error & loading state handling

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

## Architectural Decisions

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

## Testing Strategy

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

## Running the System

### Backend API

cd EmployeeAuth.Api\
dotnet run

Swagger available at /swagger.

------------------------------------------------------------------------

### Vue 3 Client

cd employee-auth-vue\
npm install\
npm run dev

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

## Configuration

Backend configuration lives in:

-   appsettings.json
-   appsettings.Development.json

Includes:

-   JWT options
-   Password policy
-   Email provider
-   Connection strings

------------------------------------------------------------------------

## Future Improvements

-   Refresh tokens
-   Role-based authorization
-   Password reset via email
-   Email queueing
-   Audit logging
-   Rate limiting
-   Docker support
-   CI/CD pipeline

------------------------------------------------------------------------

## Summary

This project demonstrates:

-   enterprise-style backend layering
-   API-first design
-   test-driven development
-   extensible authentication strategies
-   multiple UI clients on a single backend
-   modern .NET practices

------------------------------------------------------------------------

## SQL

  ``` Database [EmployeeAuthDb]
USE [master]
GO

/****** Object:  Database [EmployeeAuthDb]    Script Date: 2026-02-11 6:03:46 PM ******/
CREATE DATABASE [EmployeeAuthDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'EmployeeAuthDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\EmployeeAuthDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'EmployeeAuthDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\EmployeeAuthDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EmployeeAuthDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [EmployeeAuthDb] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET ARITHABORT OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [EmployeeAuthDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [EmployeeAuthDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET  ENABLE_BROKER 
GO

ALTER DATABASE [EmployeeAuthDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [EmployeeAuthDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET RECOVERY FULL 
GO

ALTER DATABASE [EmployeeAuthDb] SET  MULTI_USER 
GO

ALTER DATABASE [EmployeeAuthDb] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [EmployeeAuthDb] SET DB_CHAINING OFF 
GO

ALTER DATABASE [EmployeeAuthDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [EmployeeAuthDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [EmployeeAuthDb] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [EmployeeAuthDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [EmployeeAuthDb] SET QUERY_STORE = ON
GO

ALTER DATABASE [EmployeeAuthDb] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [EmployeeAuthDb] SET  READ_WRITE 
GO
```

``` TABLE [dbo].[Users]
USE [EmployeeAuthDb]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 2026-02-11 6:04:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](500) NOT NULL,
	[CreatedAtUtc] [datetime2](7) NOT NULL,
	[UpdatedAtUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Users_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

```
