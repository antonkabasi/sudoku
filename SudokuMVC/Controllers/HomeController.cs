using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Globalization;

namespace YourProjectNamespace.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index?difficulty=easy (or medium/hard)
        public IActionResult Index(string difficulty)
        {
            string chosenDifficulty = "Medium"; // default
            if (!string.IsNullOrEmpty(difficulty))
            {
                switch (difficulty.ToLower())
                {
                    case "easy":
                        chosenDifficulty = "Easy";
                        break;
                    case "medium":
                        chosenDifficulty = "Medium";
                        break;
                    case "hard":
                        chosenDifficulty = "Hard";
                        break;
                }
            }

            // Create a new puzzle using our English classes.
            var puzzle = new Sudoku(chosenDifficulty);

            // Always reset the puzzle and game start time.
            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));
            HttpContext.Session.SetString("GameStartTime", DateTime.UtcNow.ToString("o"));
            // Remove any previous stop time.
            HttpContext.Session.Remove("StopTime");

            return View(puzzle);
        }

        // POST: /Home/Check
        [HttpPost]
        public IActionResult Check()
        {
            // Retrieve the puzzle from session.
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return RedirectToAction("Index");
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);

            // Update the puzzle's user grid from the posted form values.
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    string fieldName = $"cell_{i}_{j}";
                    string posted = Request.Form[fieldName];
                    if (!string.IsNullOrEmpty(posted) && byte.TryParse(posted, out byte value))
                    {
                        puzzle.UserGrid[i][j] = value;
                    }
                    else
                    {
                        puzzle.UserGrid[i][j] = 0;
                    }
                }
            }

            // Check the puzzle status.
            string status = puzzle.CheckStatus();
            if (status == "GameNotOver")
            {
                ViewBag.Message = "The game isn't over yet.";
                HttpContext.Session.Remove("StopTime");
            }
            else if (status == "SomeIncorrect")
            {
                ViewBag.Message = "Some entries are incorrect.";
                HttpContext.Session.Remove("StopTime");
            }
            else // "Correct"
            {
                ViewBag.Message = "Congratulations! You solved the puzzle.";
                // Record stop time so the timer stops.
                HttpContext.Session.SetString("StopTime", DateTime.UtcNow.ToString("o"));
            }

            // Update session with the latest puzzle state.
            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));

            return View("Index", puzzle);
        }

        // POST: /Home/Solve
        [HttpPost]
        public IActionResult Solve()
        {
            // Retrieve the puzzle from session.
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return RedirectToAction("Index");
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);

            // Replace the user grid with the solved puzzle.
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    puzzle.UserGrid[i][j] = puzzle.Solved[i][j];
                }
            }

            ViewBag.Message = "The puzzle has been solved.";
            // Record stop time.
            HttpContext.Session.SetString("StopTime", DateTime.UtcNow.ToString("o"));

            // Update session with the solved puzzle.
            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));

            return View("Index", puzzle);
        }

        // POST: /Home/Validate
        [HttpPost]
        public IActionResult Validate([FromBody] Dictionary<string, string> cellValues)
        {
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return Json(new { status = "error", message = "No puzzle found" });
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);

            // Update the puzzle's user grid from the provided values.
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    string key = $"cell_{i}_{j}";
                    if (cellValues.ContainsKey(key) && byte.TryParse(cellValues[key], out byte value))
                    {
                        puzzle.UserGrid[i][j] = value;
                    }
                    else
                    {
                        puzzle.UserGrid[i][j] = 0;
                    }
                }
            }

            string status = puzzle.CheckStatus();
            string message = "";
            if (status == "GameNotOver")
            {
                message = "The game isn't over yet.";
            }
            else if (status == "SomeIncorrect")
            {
                message = "Some entries are incorrect.";
            }
            else // "Correct"
            {
                message = "Congratulations! You solved the puzzle.";
                // Record stop time.
                HttpContext.Session.SetString("StopTime", DateTime.UtcNow.ToString("o"));
            }

            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));
            return Json(new { status = status, message = message });
        }

        // GET: /Home/StopwatchTime
        public IActionResult StopwatchTime()
        {
            string startTimeStr = HttpContext.Session.GetString("GameStartTime");
            if (!string.IsNullOrEmpty(startTimeStr) &&
                DateTime.TryParse(startTimeStr, null, DateTimeStyles.RoundtripKind, out DateTime startTime))
            {
                // If a stop time exists, use it instead of the current time.
                string stopTimeStr = HttpContext.Session.GetString("StopTime");
                DateTime effectiveTime;
                if (!string.IsNullOrEmpty(stopTimeStr) &&
                    DateTime.TryParse(stopTimeStr, null, DateTimeStyles.RoundtripKind, out DateTime stopTime))
                {
                    effectiveTime = stopTime;
                }
                else
                {
                    effectiveTime = DateTime.UtcNow;
                }
                int elapsed = (int)(effectiveTime - startTime).TotalSeconds;
                return Json(new { elapsed });
            }
            return Json(new { elapsed = 0 });
        }
    }
}
