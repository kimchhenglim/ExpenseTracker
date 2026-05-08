# Expense Tracker

Project URL: https://roadmap.sh/projects/expense-tracker-api

This project focuses on the backend, including the API and application logic, and does not include a web user interface.

The Expense Tracker is an application that allows users to track their daily expenses. It includes authentication and security features, requiring users to log in before they can use the application.

## Features
- Create new expense records
- Retrieve all expenses
- Retrieve a single expense by ID
- Update existing expenses
- Filter expenses by range such as last week, last 3 weeks

## Prerequisites
- .NET 8 SDK
- SQL Server / SQLite (depending on your configuration)
- Entity Framework Core tools (optional for migrations)

## Configuration
1. Configure the database connection string in `appsettings.json`.
2. Apply database migrations if required:

```bash
dotnet ef database update
```

## Getting Started
1. Clone the repository or download the source code.
2. Open a terminal and navigate to the project directory.
3. Restore dependencies:

```bash
dotnet restore
```

4. Run the application:

```bash
dotnet run
```

5. The API will start locally and can be accessed using Swagger or API tools such as Postman.