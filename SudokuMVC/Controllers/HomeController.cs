using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;
using System.Globalization;

namespace YourProjectNamespace.Controllers
{
    public class HomeController : Controller
    {
        // Main page: generates a new game and starts the stopwatch automatically.
        public IActionResult Index(string difficulty)
        {
            var game = new SudokuGame();
            if (!string.IsNullOrEmpty(difficulty))
            {
                game.GeneratePuzzle(difficulty);
            }
            else
            {
                game.GeneratePuzzle("medium");
            }

            // Reset and start the stopwatch automatically.
            ResetStopwatchInternal();
            StartStopwatchInternal();

            return View(game);
        }

        // Endpoint to return the current stopwatch elapsed time (in seconds).
        public IActionResult StopwatchTime()
        {
            // Retrieve stored stopwatch data.
            string stopwatchRunning = HttpContext.Session.GetString("StopwatchRunning") ?? "false";
            string stopwatchStartString = HttpContext.Session.GetString("StopwatchStart");
            string stopwatchElapsedString = HttpContext.Session.GetString("StopwatchElapsed") ?? "0";
            int storedElapsed = int.Parse(stopwatchElapsedString);
            int totalElapsed = storedElapsed;

            // If running, add the elapsed time since the stopwatch started.
            if (stopwatchRunning == "true" && !string.IsNullOrEmpty(stopwatchStartString))
            {
                DateTime stopwatchStart = DateTime.Parse(stopwatchStartString, null, DateTimeStyles.RoundtripKind);
                totalElapsed += (int)(DateTime.UtcNow - stopwatchStart).TotalSeconds;
            }
            return Json(new { elapsed = totalElapsed });
        }

        #region Stopwatch Helper Methods

        // Resets the stopwatch session data.
        private void ResetStopwatchInternal()
        {
            HttpContext.Session.SetString("StopwatchElapsed", "0");
            HttpContext.Session.Remove("StopwatchStart");
            HttpContext.Session.SetString("StopwatchRunning", "false");
        }

        // Starts the stopwatch by recording the current time.
        private void StartStopwatchInternal()
        {
            HttpContext.Session.SetString("StopwatchStart", DateTime.UtcNow.ToString("o"));
            HttpContext.Session.SetString("StopwatchRunning", "true");
        }

        #endregion
    }
}
