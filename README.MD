# NTC Interactive Tabletop Map

A team project built with ASP.NET Core MVC for managing tabletop game sessions, maps, pieces, and tokens.

## Project Overview

This application provides a tabletop session management interface where users can:
- sign in and manage session notes
- build and place map pieces and tokens on a tabletop map
- browse a store of pieces and sets with currency-aware pricing
- track owned pieces and session inventory
- seed sample map content for local development

The app uses SQLite for local data storage and supports user authentication with cookie-based login.

## Technology Stack

- ASP.NET Core 8.0
- MVC pattern with controllers, views, and view models
- Entity Framework Core with SQLite
- Serilog logging
- JavaScript-driven tabletop map interaction
- Currency conversion via external API support

## Getting Started

### Prerequisites

- .NET SDK 8.0 or later
- Visual Studio 2022/2023, Visual Studio Code, or another compatible IDE
- Git (optional, for cloning and version control)

### Run Locally

1. Open `SAGroupAlphaSpring26.sln` in Visual Studio or your editor.
2. Restore NuGet packages.
3. Build the solution.
4. Run the project.

The app uses the local SQLite database file at `TabletopDatabase.db` by default.

### Local Seed Data

When running in development mode, the application will seed sample data if needed, including:
- default piece types like Player, Map, Monster, and Object
- sample pieces such as Default Dungeon, Cleric, Goblin Chief, and Basic Chest
- a local admin user with credentials:
  - Email: `local@demo.com`
  - Password: `Password123`

## Key Features

- user authentication and session management
- interactive tabletop map and token placement
- store browsing with pagination, filtering, and sorting
- currency conversion for item pricing
- admin/user seeded demo data for local development

## Configuration

The main app settings are in `SAGroupAlphaSpring26/appsettings.json` and include:
- `AppName`: `NTC Interactive Tabletop Map`
- `DefaultConnection`: `Data Source=./TabletopDatabase.db`

## Project Structure

- `Controllers/` — request handlers for home, account, store, map, sessions, reports, and admin actions
- `Models/` — entity and domain models for users, pieces, sets, sessions, tokens, and sales
- `ViewModels/` — models used to pass data from controllers to views
- `Services/` — business logic and data access helpers
- `ApiServices/` — currency conversion and external API handling
- `Views/` — UI templates for pages and components
- `wwwroot/` — static assets, CSS, JavaScript, and images

## Team Members
- Evan Vang — evang8@students.ntc.edu
- Max Oesterreicher — moesterreicher@students.ntc.edu
- Tristan Jackson — tjackson@students.ntc.edu

## Useful Links
- Repository: `https://dev.azure.com/ntcsaa/_git/saa-spring26`
- Board: `https://dev.azure.com/ntcsaa/saa-spring26/_sprints/taskboard/saa-spring26%20Team/saa-spring26/Sprint%201`
- Slack channel: `#sa-team-a`
