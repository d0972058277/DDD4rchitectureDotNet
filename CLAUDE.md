# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build and Run
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the WebAPI project
dotnet run --project src/Project.WebApi/Project.WebApi.csproj

# Run with docker-compose (MySQL and RabbitMQ)
docker-compose up -d
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test test/Project.Unit.Tests/Project.Unit.Tests.csproj
dotnet test test/Project.Integration.Tests/Project.Integration.Tests.csproj
dotnet test test/Architecture.Tests/Architecture.Tests.csproj

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName -s src/Project.WebApi -p src/Project.Infrastructure

# Update database
dotnet ef database update -s src/Project.WebApi -p src/Project.Infrastructure
```

## Architecture Overview

This codebase implements Domain-Driven Design (DDD) with Clean Architecture principles. The solution is organized into distinct layers with clear boundaries:

### Layer Structure
- **Architecture**: Core framework providing base classes for DDD patterns (AggregateRoot, ValueObject, DomainEvent) and infrastructure patterns (CQRS, EventBus, Unit of Work)
- **Project.Domain**: Contains domain entities, value objects, domain events, and repository interfaces. This layer has no dependencies on other layers
- **Project.Application**: Implements use cases through CQRS pattern using MediatR. Contains command/query handlers, DTOs, and application services
- **Project.Infrastructure**: Implements repository interfaces, database context, external service integrations, and infrastructure concerns
- **Project.WebApi**: REST API layer with controllers, request/response models, validation, and Swagger documentation

### Key Architectural Patterns

**CQRS Implementation**: Commands and queries are separated using MediatR. Commands modify state while queries read state. Look for ICommand and IQuery interfaces in the Architecture project.

**Event-Driven Architecture**: The system uses both domain events (for in-process notifications) and integration events (for cross-service communication). Events are processed reliably using the Outbox/Inbox pattern to ensure eventual consistency.

**Repository Pattern with Unit of Work**: Repositories abstract data access logic. The Unit of Work pattern ensures transactional consistency across aggregate boundaries.

**Aggregate Design**: Aggregates are the transaction boundaries. Each aggregate root inherits from AggregateRoot base class and enforces business invariants.

### Database and Messaging
- **Database**: MySQL with Entity Framework Core. Connection string is configured in appsettings.json
- **Message Bus**: RabbitMQ via MassTransit for integration events
- **Background Jobs**: Hangfire for processing outbox messages and other background tasks

### Testing Strategy
- **Unit Tests**: Test domain logic and application handlers in isolation using xUnit, FluentAssertions, and Moq
- **Integration Tests**: Test database operations and external service integrations
- **Architecture Tests**: Verify architectural constraints and dependencies between layers