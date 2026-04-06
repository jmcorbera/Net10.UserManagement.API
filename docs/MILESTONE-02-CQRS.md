# Milestone 2 — CQRS + Validation - `v2-cqrs`

**Version:** 2.0.0

## 📋 Overview

**Objective:** Implement separation of responsibilities using CQRS (Command Query Responsibility Segregation) pattern and robust validation with FluentValidation.

This milestone transforms the application from a simple CRUD service-based architecture to a more scalable CQRS pattern using MediatR, introduces automatic validation pipelines, and improves code organization and testability.

## 🎯 Goals

- ✅ Clear separation: Commands (write) vs Queries (read)
- ✅ Automatic validation in pipeline
- ✅ Thin controllers/endpoints
- ✅ Scalable and testable code
- ✅ Centralized cross-cutting concerns with behaviors

## 🏗️ Architecture Changes

### Before (Milestone 1)
```
Endpoint → Service → Repository
```

### After (Milestone 2)
```
Endpoint → MediatR → Handler → Repository
                ↓
         Pipeline Behaviors
         (Validation, Logging)
```

### CQRS Pattern

**Commands (Write Operations):**
- Modify state (Create, Update, Delete)
- Return void or simple confirmation
- Validated before execution
- Examples: `CreateUserCommand`, `UpdateUserCommand`

**Queries (Read Operations):**
- Read-only operations
- Return data
- No side effects
- Examples: `GetAllUsersQuery`, `GetUserByIdQuery`

### Benefits
- **Separation of Concerns:** Read and write logic are separated
- **Scalability:** Can optimize reads and writes independently
- **Testability:** Handlers are isolated and easy to test
- **Maintainability:** Single responsibility per handler
- **Flexibility:** Easy to add cross-cutting concerns via behaviors

## 📦 Features

### Feature 1: CQRS Implementation

**Branch:** `feature/cqrs-implementation`

#### Overview
Implement the CQRS pattern using MediatR library. This feature introduces Commands and Queries to replace direct service calls, refactors endpoints to be thin HTTP handlers, and adds AutoMapper for DTO mapping.

#### Tasks
- [ ] Add MediatR package and configure DI
- [ ] Create folder structure for Commands and Queries
- [ ] Implement Commands for write operations
- [ ] Implement Queries for read operations
- [ ] Add AutoMapper for DTO mapping
- [ ] Refactor endpoints to use MediatR
- [ ] Remove or deprecate old service layer

#### Implementation Details

**Dependencies Added:**
```xml
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="AutoMapper" Version="13.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="13.0.1" />
```

**MediatR Configuration:**
```csharp
// In Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    
    return services;
}
```

**Key Files:**
- `src/Net10.UserManagement.Application/Users/Commands/**/*.cs` - All command handlers
- `src/Net10.UserManagement.Application/Users/Queries/**/*.cs` - All query handlers
- `src/Net10.UserManagement.Application/Users/DTOs/*.cs` - Data transfer objects
- `src/Net10.UserManagement.Application/Common/Mappings/UserMappingProfile.cs` - AutoMapper configuration
- `src/Net10.UserManagement.Application/DependencyInjection.cs` - MediatR registration
- `src/Net10.UserManagement.Api/Endpoints/Users.cs` - Refactored endpoints

#### What Was Achieved
- ✅ CQRS pattern implemented with clear separation
- ✅ MediatR handles all request/response flow
- ✅ Endpoints are thin and focused on HTTP concerns only
- ✅ AutoMapper eliminates manual mapping code
- ✅ Handlers are isolated and easily testable
- ✅ Foundation ready for pipeline behaviors

---

### Feature 2: FluentValidation

**Branch:** `feature/fluent-validation`

#### Overview
Implement automatic validation using FluentValidation and MediatR pipeline behaviors. This ensures all commands are validated before execution, provides consistent error responses, and centralizes validation logic.

#### Tasks
- [ ] Add FluentValidation packages
- [ ] Create validators for all commands
- [ ] Implement ValidationBehavior pipeline
- [ ] Implement LoggingBehavior pipeline
- [ ] Configure behaviors in MediatR
- [ ] Handle validation errors in endpoints

#### Implementation Details

**Dependencies Added:**
```xml
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
```

**FluentValidation Configuration:**
```csharp
// In Application/DependencyInjection.cs
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    });
    
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    
    return services;
}
```

**Key Files:**
- `src/Net10.UserManagement.Application/Users/Commands/*/Validators/*.cs` - Command validators
- `src/Net10.UserManagement.Application/Common/Behaviors/ValidationBehavior.cs` - Validation pipeline
- `src/Net10.UserManagement.Application/Common/Behaviors/LoggingBehavior.cs` - Logging pipeline
- `src/Net10.UserManagement.Application/Common/Exceptions/ValidationException.cs` - Custom exception
- `src/Net10.UserManagement.Api/Middleware/GlobalExceptionHandlerMiddleware.cs` - Exception handling

#### What Was Achieved
- ✅ Automatic validation before command execution
- ✅ Consistent validation error responses
- ✅ Centralized validation logic
- ✅ Async validation support (e.g., unique email check)
- ✅ Request/response logging in pipeline
- ✅ Clean separation of validation from business logic

---

### Feature 3: CQRS Tests

**Branch:** `feature/cqrs-tests`

#### Overview
Comprehensive test coverage for the CQRS implementation, including handlers, validators, and pipeline behaviors.

#### Tasks
- [ ] Create tests for all command handlers
- [ ] Create tests for all query handlers
- [ ] Create tests for all validators
- [ ] Create tests for pipeline behaviors
- [ ] Create integration tests for complete flows
- [ ] Verify test coverage > 80%

#### Implementation Details

**Test Structure:**
```
tests/Net10.UserManagement.Application.Tests/
├── Users/
│   ├── Commands/
│   │   ├── CreateUser/
│   │   │   ├── CreateUserCommandHandlerTests.cs
│   │   │   └── CreateUserCommandValidatorTests.cs
│   │   ├── UpdateUser/
│   │   │   ├── UpdateUserCommandHandlerTests.cs
│   │   │   └── UpdateUserCommandValidatorTests.cs
│   │   └── DeleteUser/
│   │       └── DeleteUserCommandHandlerTests.cs
│   └── Queries/
│       ├── GetAllUsersQueryHandlerTests.cs
│       ├── GetUserByIdQueryHandlerTests.cs
│       └── GetUserByEmailQueryHandlerTests.cs
└── Common/
    └── Behaviors/
        ├── ValidationBehaviorTests.cs
        └── LoggingBehaviorTests.cs
```

**Key Files:**
- `tests/Net10.UserManagement.Application.Tests/Users/Commands/**/*Tests.cs` - Command tests
- `tests/Net10.UserManagement.Application.Tests/Users/Queries/**/*Tests.cs` - Query tests
- `tests/Net10.UserManagement.Application.Tests/Common/Behaviors/*Tests.cs` - Behavior tests
- `tests/Net10.UserManagement.Api.Tests/Integration/*Tests.cs` - Integration tests

#### What Was Achieved
- ✅ Comprehensive unit test coverage for handlers
- ✅ Thorough validator testing with edge cases
- ✅ Pipeline behavior tests
- ✅ Integration tests for complete flows
- ✅ Test coverage > 80%
- ✅ Clear test naming and organization

---

## 🧪 Testing Strategy

### Unit Tests
- **Handlers:** Test business logic in isolation with mocked dependencies
- **Validators:** Test all validation rules and edge cases
- **Behaviors:** Test pipeline behavior logic
- **Target Coverage:** > 80%

### Integration Tests
- **API Endpoints:** Test complete request/response flows
- **Validation Flow:** Verify validation errors are properly returned
- **Happy Paths:** Ensure successful operations work end-to-end

### Test Execution
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Net10.UserManagement.Application.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~CreateUserCommandHandlerTests"
```

## 📊 Success Criteria

### Functional Requirements
- [ ] All CRUD operations work through CQRS pattern
- [ ] Commands and Queries are properly separated
- [ ] Validation occurs automatically before command execution
- [ ] Validation errors return proper 400 responses
- [ ] Logging occurs for all requests

### Quality Requirements
- [ ] All tests pass (unit + integration)
- [ ] Test coverage > 80%
- [ ] No compiler warnings
- [ ] Clean Architecture principles maintained
- [ ] SOLID principles followed

### Code Quality
- [ ] Handlers follow single responsibility principle
- [ ] Validators are comprehensive and clear
- [ ] No code duplication
- [ ] Proper error handling throughout
- [ ] Meaningful naming conventions

## 🚀 Next Steps

5. Proceed to **Milestone 3: Persistence (EF Core)**

## 📚 References

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [AutoMapper Documentation](https://docs.automapper.org/)
- [Pipeline Behaviors in MediatR](https://github.com/jbogard/MediatR/wiki/Behaviors)

