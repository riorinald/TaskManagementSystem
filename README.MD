# Task Management System

## Project Structure

The project follows a clean architecture approach with the following layers:

- **Presentation API Layer**: Contains API controllers and endpoints.
- **Application Layer**: Contains application services, DTOs, and business logic.
- **Domain Layer**: Contains the core business entities, interfaces, and logic.
- **Infrastructure Layer**: Contains implementations of repositories, database access currently with SQLite
- **Testing Layer**: Contains xUnit Test


## how to start
Running with .NET CLI
   - dotnet restore
   - dotnet build
   - dotnet run --project TaskManagementSystem.API

Running with Visual Studio
  - open project files
  - restore NuGet Package
  - Run the application TaskManagementSystem.API with IIS Express

## API Endpoints
GET /api/workitems - List all workitems
GET /api/workitems/{id} - Get task details
POST /api/workitems - Create a task
PUT /api/workitems/{id} - Update a task
DELETE /api/workitems/{id} - Delete a task
GET /api/users - List all users

## System Diagram
![Diagram](https://raw.githubusercontent.com/riorinald/TaskManagementSystem/master/diagram.jpg "diagram")

## GitHub Action
Test github action
