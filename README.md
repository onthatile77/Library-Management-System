# Library Management System

A library management application for managing books, members and borrowing, built to practise object-oriented design and working with a database.

## About the project

Libraries need to know what books they hold, who the members are and which books are out on loan. This project models that workflow: staff can maintain the catalogue and members, and record when books are borrowed and returned.

## Features

- Add, edit, remove and search books
- Manage library members
- Borrow and return books, with loan tracking

## Tech stack

| Area | Tools |
| --- | --- |
| Language | [C#] |
| Framework | [.NET version, and the UI type: Web Forms] |
| Database | [SSQLite]|
| Tools | Git, GitHub, [Visual Studio / VS Code] |


## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) [version]
- [Database engine, and how to create the database or run the setup script]

### Run locally

```bash
git clone https://github.com/onthatile77/Library-Management-System.git
cd Library-Management-System

# Update the database connection string in [appsettings.json / App.config]
dotnet restore
dotnet run
```

## Database

[Describe your main tables, for example Books, Members and Loans, and how they relate. Add a diagram or the SQL setup script if you have one.]

## What I learned

- Modelling a real-world process (books, members, loans) as classes and database tables
- Separating data access, business logic and the user interface

