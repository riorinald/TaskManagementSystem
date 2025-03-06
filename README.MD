# Task Management System

A Task Management System that allows users to create, assign, and track tasks following SOLID principles and clean architecture using NET FRAMEWORK 8.0
makeit the database configureable in web.config to switch between SQLite database and json db.
simplify varibale names, also avoid using Task as varialbe that may conflict

## Project Structure

The project follows a clean architecture approach with the following layers:

- **Domain Layer**: Contains the core business entities, interfaces, and logic.
- **Application Layer**: Contains application services, DTOs, and business logic.
- **Infrastructure Layer**: Contains implementations of repositories, database access with SQLite
- **Presentation Layer**: Contains API controllers and endpoints.

## Core Features

- Create a new task (Title, Description, Due Date, Priority, Status)
- Assign a task to a user
- Update task details (e.g., status, priority)
- Delete a task
- Retrieve a list of all tasks
- Retrieve tasks assigned to a specific user
- Add validation for task data (e.g., due date cannot be in the past)
- Implement logging for critical operations
- Add unit tests for core functionality testing with xUnit

## SOLID Principles Implementation

- **Single Responsibility Principle (SRP)**: Each class/method has a single responsibility
- **Open/Closed Principle (OCP)**: The system is open for extension but closed for modification
- **Liskov Substitution Principle (LSP)**: Subtypes are substitutable for their base types
- **Interface Segregation Principle (ISP)**: Interfaces are specific to the needs of the client
- **Dependency Inversion Principle (DIP)**: High-level modules don't depend on low-level modules; both depend on abstractions

## Clean Infrastructure

- Uses dependency injection to manage dependencies
- Separates concerns into different layers
- Uses repository pattern to abstract data access logic
- Uses a mock database for persistence