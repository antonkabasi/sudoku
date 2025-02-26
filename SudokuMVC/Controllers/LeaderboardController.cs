using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProjectNamespace.Models;
using System.Linq;
using System.Threading.Tasks;

namespace YourProjectNamespace.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardDbContext _context;

        public LeaderboardController(LeaderboardDbContext context)
        {
            _context = context;
        }

        // GET: Leaderboard?difficulty=Easy (or Medium, Hard)
        public async Task<IActionResult> Index(string difficulty)
        {
            var entries = _context.LeaderboardEntries.AsQueryable();
            if (!string.IsNullOrEmpty(difficulty))
            {
                // Use case-insensitive filtering.
                entries = entries.Where(e => e.Difficulty.ToLower() == difficulty.ToLower());
            }
            // Order by StopwatchValue (lower is better) and then DateAchieved.
            return View(await entries.OrderBy(e => e.StopwatchValue)
                                      .ThenBy(e => e.DateAchieved)
                                      .ToListAsync());
        }

        // GET: Leaderboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaderboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaderboardEntry entry)
        {
            if (ModelState.IsValid)
            {
                entry.DateAchieved = System.DateTime.Now;
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }

        // GET: Leaderboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entry = await _context.LeaderboardEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        // POST: Leaderboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaderboardEntry entry)
        {
            if (id != entry.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LeaderboardEntries.Any(e => e.Id == entry.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }

        // GET: Leaderboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var entry = await _context.LeaderboardEntries.FirstOrDefaultAsync(m => m.Id == id);
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        // POST: Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.LeaderboardEntries.FindAsync(id);
            _context.LeaderboardEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
