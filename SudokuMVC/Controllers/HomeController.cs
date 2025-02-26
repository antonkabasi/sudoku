using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YourProjectNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly LeaderboardDbContext _leaderboardContext;

        // Inject LeaderboardDbContext so we can check and save leaderboard entries.
        public HomeController(LeaderboardDbContext leaderboardContext)
        {
            _leaderboardContext = leaderboardContext;
        }

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
        public async Task<IActionResult> Check()
        {
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return RedirectToAction("Index");
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);

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

            string status = puzzle.CheckStatus();
            // If the game isn't complete or is incorrect, clear any StopTime and restart the timer.
            if (status == "GameNotOver")
            {
                ViewBag.Message = "The game isn't over yet.";
                HttpContext.Session.Remove("StopTime");
                HttpContext.Session.SetString("GameStartTime", DateTime.UtcNow.ToString("o"));
            }
            else if (status == "SomeIncorrect")
            {
                ViewBag.Message = "Some entries are incorrect.";
                HttpContext.Session.Remove("StopTime");
                HttpContext.Session.SetString("GameStartTime", DateTime.UtcNow.ToString("o"));
            }
            else // "Correct"
            {
                // Record stop time so that the timer stops.
                HttpContext.Session.SetString("StopTime", DateTime.UtcNow.ToString("o"));

                // Compute elapsed time.
                string startTimeStr = HttpContext.Session.GetString("GameStartTime");
                string stopTimeStr = HttpContext.Session.GetString("StopTime");
                DateTime startTime = DateTime.Parse(startTimeStr, null, DateTimeStyles.RoundtripKind);
                DateTime stopTime = DateTime.Parse(stopTimeStr, null, DateTimeStyles.RoundtripKind);
                int elapsed = (int)(stopTime - startTime).TotalSeconds;

                // Check if this elapsed time qualifies for top 10 for this difficulty.
                var entries = await _leaderboardContext.LeaderboardEntries
                                        .Where(e => e.Difficulty.ToLower() == puzzle.Difficulty.ToLower())
                                        .OrderBy(e => e.StopwatchValue)
                                        .ToListAsync();
                bool qualifies = false;
                if (entries.Count < 10)
                {
                    qualifies = true;
                }
                else if (elapsed < entries.Last().StopwatchValue)
                {
                    qualifies = true;
                }

                if (qualifies)
                {
                    // Pass elapsed time and difficulty via TempData.
                    TempData["Top10Qualified"] = "true";
                    TempData["ElapsedTime"] = elapsed.ToString();
                    TempData["Difficulty"] = puzzle.Difficulty;
                    return RedirectToAction("SubmitScore");
                }
                else
                {
                    ViewBag.Message = "Congratulations! You solved the puzzle.";
                }
            }

            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));
            return View("Index", puzzle);
        }

        // POST: /Home/Solve
        [HttpPost]
        public IActionResult Solve()
        {
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return RedirectToAction("Index");
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    puzzle.UserGrid[i][j] = puzzle.Solved[i][j];
                }
            }
            ViewBag.Message = "The puzzle has been solved.";
            HttpContext.Session.SetString("StopTime", DateTime.UtcNow.ToString("o"));
            HttpContext.Session.SetString("SudokuPuzzle", JsonSerializer.Serialize(puzzle));
            return View("Index", puzzle);
        }

        // GET: /Home/SubmitScore
        public IActionResult SubmitScore()
        {
            if (TempData["Top10Qualified"]?.ToString() == "true")
            {
                // Pre-fill a LeaderboardEntry model with the elapsed time and difficulty.
                var entry = new LeaderboardEntry
                {
                    Difficulty = TempData["Difficulty"]?.ToString(),
                    StopwatchValue = int.Parse(TempData["ElapsedTime"]?.ToString() ?? "0")
                };
                return View(entry);
            }
            return RedirectToAction("Index");
        }

        // POST: /Home/SubmitScore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitScore(LeaderboardEntry entry)
        {
            if (ModelState.IsValid)
            {
                entry.DateAchieved = DateTime.Now;
                _leaderboardContext.LeaderboardEntries.Add(entry);
                await _leaderboardContext.SaveChangesAsync();
                return RedirectToAction("Index", "Leaderboard");
            }
            return View(entry);
        }

        // POST: /Home/Validate
        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] Dictionary<string, string> cellValues)
        {
            string puzzleJson = HttpContext.Session.GetString("SudokuPuzzle");
            if (string.IsNullOrEmpty(puzzleJson))
            {
                return Json(new { status = "error", message = "No puzzle found" });
            }
            var puzzle = JsonSerializer.Deserialize<Sudoku>(puzzleJson);

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
                HttpContext.Session.Remove("StopTime");
                HttpContext.Session.SetString("GameStartTime", DateTime.UtcNow.ToString("o"));
            }
            else if (status == "SomeIncorrect")
            {
                message = "Some entries are incorrect.";
                HttpContext.Session.Remove("StopTime");
                HttpContext.Session.SetString("GameStartTime", DateTime.UtcNow.ToString("o"));
            }
            else // "Correct"
            {
                message = "Congratulations! You solved the puzzle.";
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
