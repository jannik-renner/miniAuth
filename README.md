# MiniAuth

A lightweight authentication and authorization service
built with ASP.NET Core and .NET 8.

I implemented a layered authentication service using dependency inversion, repository abstractions, EF Core, secure password hashing, role-based domain modelling and ASP.NET Core.

## Features

- JWT authentication
- Refresh token rotation
- Role-based authorization
- Secure password hashing
- Entity Framework Core
- SQL Server
- Docker
- Unit & integration tests
- OpenAPI
- GitHub Actions CI

## Architecture

[architecture diagram]

## Getting Started

docker compose up

## API

Swagger:
http://localhost:8080/swagger

## Authentication Flow

[authentication diagram]

## Testing

dotnet test