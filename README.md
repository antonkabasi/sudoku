# Sudoku Generator & Solver

## Overview

This repository contains a **Sudoku Generator & Solver**, originally written in **C# using Windows Forms** as a part of the **Object-Oriented Programming course** at the **Faculty of Science, University of Split** in 2015.

The original version *used to* work on my PC (a meme), but now, with C# being **open-source and cross-platform**, I am **rewriting it using ASP.NET Core, Entity Framework Core**, and adding **CRUD functionality** to build a fully functional web application.

## New Features in the ASP.NET Core Version

- **Stopwatch:** Track game time for each puzzle.
- **High Score Table:** Best times per difficulty level.
- **Improved UI:** Web-based interfaces.

## Tech Stack

- **Backend:** ASP.NET Core, Entity Framework Core
- **Frontend:** Razor Pages&#x20;
- **Database:** SQLite&#x20;
- **Authentication:** Identity Framework

## Installation & Running

### Original Windows Forms Version (Legacy)

1. Open `SudokuGenerator.sln` in Visual Studio 2015 (or later).
2. Restore missing NuGet packages.
3. Build and run the project.

### New ASP.NET Core Version

1. Clone the repository:
   ```sh
   git clone https://github.com/your-username/sudoku-generator.git
   cd sudoku-generator
   ```
2. Install dependencies:
   ```sh
   dotnet restore
   ```
3. Run the application:
   ```sh
   dotnet run
   ```
4. Open `http://localhost:5105` in your browser.

## License

This project is licensed under the MIT License.

