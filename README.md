# Sudoku Web App

## Overview

This repository contains a web-based **Sudoku** application. Originally developed in C# using Windows Forms 
(in Croatian) for an Object‑Oriented Programming course at the Faculty of Science, University of Split in 2015, 
this project has been completely re‑implemented as an **ASP.NET Core MVC** application.

The original version used to work on my PC, but now it doesn't so with C# being open-source and cross-platform, 
I am rewriting it using ASP.NET Core, Entity Framework Core, and adding CRUD functionality.

## New Features

- **Stopwatch Integration:** Tracks the game time for each puzzle.
- **Top‑10 Leaderboard:** Automatically determines if your time qualifies as a new top‑10 score for the chosen difficulty and prompts you to submit your score.
- **CRUD Functionality:** Manage leaderboard entries (create, read, update, and delete). 
                        
            But you have to unlock it by finishing a game first! :)
            (or remove the comment on the solve button in the Views/Home/index.cshtml line 130)

- **Responsive UI:** Built using ASP.NET Core MVC, Razor Views, and Bootstrap 5. 
- **Persistent Database:** SQlite, for testing purposes.

## Tech Stack

- **Backend:** ASP.NET Core MVC, Entity Framework Core
- **Frontend:** Razor Views, Bootstrap 5
- **Database:** SQLite
- **Containerization:** Docker

## TODO
-- **Minor frontend modifications**

## Might Do
- **Authentication**
- **Deploy**

## Installation & Running

   **Option 1: Local installation**

      1. **Clone the repository:**

         git clone https://github.com/antonkabasi/sudoku.git

         cd sudoku

      2. **Restore dependencies:**
      Assuming you have latest .NET Core version installed 

         dotnet restore

      3. **Apply Migrations**

      The project uses Entity Framework Core with a SQLite database for testing, 
      database isn't included in the github project.
      
      Run the following command to create the database:

         dotnet ef database update

      4. **Build and Run the Application:**

         dotnet run

      5. **Open in your Browser:**

      Navigate to the URL shown in the console (typically http://localhost:5000).

   **Option 2: Running in Docker - suggested option!**

   This project includes a Dockerfile for building and running the ASP.NET Core MVC application in a container. The Docker image is configured to listen on port 5000.

      1. **Build the Docker Image:**

      From the project root, run:

      docker build --no-cache -t sudoku-app .

      2. **Run the docker container:**

      docker run -d -p 5000:5000 --name sudoku-container sudoku-app

      3. Open the app in your web browser at: http://localhost:5000/


Copyright (c) 2025 Anton Kabaši

This project is licensed under the MIT License. See the LICENSE file for details.