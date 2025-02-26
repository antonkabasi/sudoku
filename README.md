# Sudoku Web App

## Overview

This repository contains a web-based **Sudoku** application. Originally developed in C# using Windows Forms (in Croatian) for an Object‑Oriented Programming course at the Faculty of Science, University of Split in 2015, this project has been completely re‑implemented as an **ASP.NET Core MVC** application.

The original version used to work on my PC, but now it doesn't so with C# being open-source and cross-platform, I am rewriting it using ASP.NET Core, Entity Framework Core, and adding CRUD functionality.

## New Features

- **Stopwatch Integration:** Tracks the game time for each puzzle.
- **Top‑10 Leaderboard:** Automatically determines if your time qualifies as a new top‑10 score for the chosen difficulty and prompts you to submit your score.
- **CRUD Functionality:** Manage leaderboard entries (create, read, update, and delete).
- **Responsive UI:** Built using ASP.NET Core MVC, Razor Views, and Bootstrap 5.
- **Persistent Database:** Uses SQLite by default, with an option to switch to a hosted PostgreSQL (e.g., Supabase) if desired.

## Tech Stack

- **Backend:** ASP.NET Core MVC, Entity Framework Core
- **Frontend:** Razor Views, Bootstrap 5
- **Database:** SQLite
- **Authentication:** None

## Installation & Running

### Legacy Windows Forms Version (Legacy)

1. Open `SudokuGenerator.sln` in Visual Studio 2015 (or later).
2. Restore missing NuGet packages.
3. Build and run the project.

### New ASP.NET Core MVC Version

1. **Clone the repository:**

   git clone https://github.com/antonkabasi/sudoku.git
   cd sudoku-generator

2. **Restore dependencies:**

   dotnet restore