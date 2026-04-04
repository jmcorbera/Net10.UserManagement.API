# Milestone 1 — CRUD (Clean Architecture) - `v1-crud`

**Version:** 1.0.0  

## 📋 Overview

**Objective:** Complete a solid and simple structure with all necessary foundations for a Clean Architecture-based User Management API.

This milestone establishes the fundamental architecture, basic CRUD operations, health monitoring, API documentation, and testing infrastructure without real persistence (in-memory implementation).

## 🎯 Goals

- ✅ Functional API with complete CRUD operations
- ✅ Running tests (unit + integration)
- ✅ Documented Scalar/OpenAPI
- ✅ Operational HealthChecks
- ✅ Validated Clean Architecture

## 🏗️ Architecture

The project follows Clean Architecture principles with four distinct layers:

```
src/
├── Net10.UserManagement.Api/          # Presentation Layer
│   ├── Bootstrapper/                  # DI and middleware configuration
│   ├── Endpoints/                     # Minimal API endpoints
│   └── Program.cs                     # Application entry point
│
├── Net10.UserManagement.Application/  # Application Layer
│   ├── Abstracts/                     # Service interfaces
│   └── Users/                         # User services
│
├── Net10.UserManagement.Domain/       # Domain Layer
│   ├── Entities/                      # Domain entities
│   └── Repositories/                  # Repository interfaces
│
└── Net10.UserManagement.Infrastructure/ # Infrastructure Layer
    └── Repositories/                  # Repository implementations
```

### Layer Responsibilities

#### Domain Layer
- Contains core business entities (`User`)
- Defines repository contracts (`IUserRepository`)
- No dependencies on other layers
- Pure business logic and rules

#### Application Layer
- Implements business use cases
- Service interfaces and implementations (`IUserService`, `UserService`)
- Orchestrates domain objects
- Depends only on Domain layer

#### Infrastructure Layer
- Implements data access (`UserRepository`)
- In-memory implementation for Milestone 1
- External service integrations (future milestones)
- Depends on Domain layer

#### Presentation Layer (API)
- HTTP endpoints using Minimal APIs
- Request/Response handling
- API documentation (OpenAPI/Scalar)
- Middleware configuration
- Health checks
- Depends on Application and Infrastructure layers

## 📦 Features

### Feature 1: Base Configuration

**Branch:** `feature/m1-base`

### Base Configuration

#### Tasks
- [x] Verify and adjust project references
- [x] Configure `.env` and `.env.example` files (no `appsettings.json`)
- [x] Implement environment variable loading with `DotNetEnv`
- [x] Configure `Program.cs` with all base services

#### Implementation Details

**Environment Variables:**
```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
```

**Dependencies:**
- `DotNetEnv` (v3.1.1) - Environment variable management

**Key Files:**
- `src/Net10.UserManagement.Api/.env.example` - Template for environment variables
- `src/Net10.UserManagement.Api/Program.cs` - Application bootstrap
- `src/Net10.UserManagement.Api/Bootstrapper/DependencyInjection.cs` - Service registration

#### Validation Checklist
- [x] All project references are correct
- [x] `.env.example` file exists with all required variables
- [x] Environment variables load correctly on startup
- [x] No hardcoded configuration in code

---

### Add Health Checks

#### Tasks
- [x] Add `Microsoft.Extensions.Diagnostics.HealthChecks` package
- [x] Create `/health/live` endpoint to verify API status
- [x] Create `/health/ready` endpoint for readiness checks
- [x] Configure basic health checks (memory, disk)

#### Implementation Details

**Endpoints:**
- `GET /health/live` - Liveness probe (is the app running?)
- `GET /health/ready` - Readiness probe (is the app ready to serve traffic?)

**Health Check Configuration:**
```csharp
services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck("self-ready", () => HealthCheckResult.Healthy(), tags: ["ready"]);
```

**Key Files:**
- `src/Net10.UserManagement.Api/Bootstrapper/DependencyInjection.cs` - Health check registration
- `src/Net10.UserManagement.Api/Endpoints/Health.cs` - Health check endpoints

#### Validation Checklist
- [x] `/health/live` returns 200 OK
- [x] `/health/ready` returns 200 OK
- [x] Health checks respond within 1 second
- [x] Health check responses include status information

---

### Add Scalar/OpenAPI Documentation

#### Tasks
- [x] Configure complete Scalar/OpenAPI documentation
- [x] Add descriptions to endpoints
- [x] Configure API versioning (v1)
- [x] Integrate Scalar UI for modern documentation interface

#### Implementation Details

**Dependencies:**
- `Microsoft.AspNetCore.OpenApi` (v10.0.5)
- `Scalar.AspNetCore` (v2.13.19)

**OpenAPI Configuration:**
```csharp
services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "User Management API";
        document.Info.Version = "v1";
        document.Info.Description = "API for user management with Clean Architecture";
        return Task.CompletedTask;
    });
});
```

**Documentation URLs:**
- Scalar UI: `https://localhost:5001/scalar/v1`
- OpenAPI JSON: `https://localhost:5001/openapi/v1.json`

**Key Files:**
- `src/Net10.UserManagement.Api/Bootstrapper/DependencyInjection.cs` - OpenAPI configuration
- `src/Net10.UserManagement.Api/Bootstrapper/MiddlewareExtensions.cs` - Scalar UI setup

#### Validation Checklist
- [x] Scalar UI loads correctly in development mode
- [x] All endpoints are documented
- [x] Endpoint descriptions are clear and accurate
- [x] Request/Response schemas are correct
- [x] API version is clearly indicated

---

### Feature 2: CRUD implementation

**Branch:** `feature/m1-crud`

### User Entity Enhancement

##### Tasks
- [x] Add `Status` enum (Pending/Active/Inactive)
- [x] Add basic validations to the model
- [x] Update tests for new properties

**Key Files:**
- `src/Net10.UserManagement.Domain/Entities/User.cs` - User entity
- `src/Net10.UserManagement.Domain/Enums/UserStatus.cs` - Status enum

#### Validation Checklist
- [x] User entity has all required properties
- [x] UserStatus enum is properly defined
- [x] Default status is Pending
- [x] All tests pass with new properties

---

### User CRUD Operations

#### Tasks
- [x] Implement `GET /api/v1/users` - List all users
- [x] Implement `GET /api/v1/users/{id}` - Get user by ID
- [x] Implement `POST /api/v1/users` - Create user
- [x] Implement `PUT /api/v1/users/{id}` - Update user
- [x] Implement `DELETE /api/v1/users/{id}` - Delete user (hard delete)

#### Implementation Details

**Endpoints:**

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/v1/users` | Get all users | - | `200 OK` with user array |
| GET | `/api/v1/users/{id}` | Get user by ID | - | `200 OK` with user or `404 Not Found` |
| POST | `/api/v1/users` | Create new user | User DTO | `201 Created` with user |
| PUT | `/api/v1/users/{id}` | Update user | User DTO | `200 OK` with updated user or `404 Not Found` |
| DELETE | `/api/v1/users/{id}` | Delete user | - | `204 No Content` or `404 Not Found` |

**Key Files:**
- `src/Net10.UserManagement.Api/Endpoints/Users.cs` - User endpoints
- `src/Net10.UserManagement.Application/Users/Services/UserService.cs` - User service
- `src/Net10.UserManagement.Application/Abstracts/IUserService.cs` - Service interface

#### Validation Checklist
- [x] All CRUD endpoints are implemented
- [x] Endpoints follow REST conventions
- [x] Proper HTTP status codes are returned
- [x] Request validation is in place
- [x] Responses include all necessary data
- [x] Error handling is implemented

---

### Repository Pattern

#### Tasks
- [x] Create `IUserRepository` interface with CRUD operations
- [x] Implement in-memory repository for testing
- [x] Configure repository in DI container
- [x] Update service to use repository

**Key Files:**
- `src/Net10.UserManagement.Domain/Repositories/IUserRepository.cs` - Repository interface
- `src/Net10.UserManagement.Infrastructure/Repositories/UserRepository.cs` - In-memory implementation
- `src/Net10.UserManagement.Infrastructure/DependencyInjection.cs` - Repository registration

#### Validation Checklist
- [x] Repository interface is defined in Domain layer
- [x] In-memory implementation works correctly
- [x] Repository is registered in DI container
- [x] Service uses repository instead of direct data access
- [x] All CRUD operations work through repository

---
### Feature 3: Unit Testing Setup

**Branch:** `feature/m1-unitTest`

### Unit Testing Setup

#### Tasks
- [x] Configure xUnit testing framework
- [x] Create unit tests for User entity
- [x] Configure basic test coverage reporting

#### Implementation Details

**Test Projects:**
```
tests/
├── Net10.UserManagement.Domain.Tests/      # Domain unit tests
├── Net10.UserManagement.Application.Tests/ # Application unit tests
└── Net10.UserManagement.Api.Tests/         # API integration tests
```

**Testing Framework:**
- xUnit - Test framework
- Moq - Mocking library (if needed)
- FluentAssertions - Assertion library (optional)

**Key Files:**
- `tests/Net10.UserManagement.Domain.Tests/UserTests.cs`
- `tests/Net10.UserManagement.Application.Tests/UserServiceTests.cs`
- `tests/Net10.UserManagement.Api.Tests/UsersEndpointsTests.cs`

#### Validation Checklist
- [x] All test projects build successfully
- [x] Domain tests cover entity behavior
- [x] Application tests cover service logic
- [x] API tests cover endpoint behavior
- [x] All tests pass
- [x] Test coverage is > 70%

---

## 🧪 Testing Strategy

### Unit Tests
- **Domain Layer:** Entity behavior, business rules
- **Application Layer:** Service logic, use cases
- **Target Coverage:** > 70%

### Test Execution
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Net10.UserManagement.Domain.Tests
dotnet test tests/Net10.UserManagement.Application.Tests
dotnet test tests/Net10.UserManagement.Api.Tests

```

## 🔄 Next Steps

Proceed to **Milestone 2: CQRS + Validation**

## 📚 References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/)
- [Minimal APIs in .NET](https://docs.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [xUnit Documentation](https://xunit.net/)
