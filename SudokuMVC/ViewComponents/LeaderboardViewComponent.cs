using Microsoft.AspNetCore.Mvc;
using YourProjectNamespace.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace YourProjectNamespace.ViewComponents
{
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly LeaderboardDbContext _context;
        public LeaderboardViewComponent(LeaderboardDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var easyEntries = await _context.LeaderboardEntries
                                  .Where(e => e.Difficulty.ToLower() == "easy")
                                  .OrderBy(e => e.StopwatchValue)
                                  .ThenBy(e => e.DateAchieved)
                                  .ToListAsync();
            var mediumEntries = await _context.LeaderboardEntries
                                  .Where(e => e.Difficulty.ToLower() == "medium")
                                  .OrderBy(e => e.StopwatchValue)
                                  .ThenBy(e => e.DateAchieved)
                                  .ToListAsync();
            var hardEntries = await _context.LeaderboardEntries
                                  .Where(e => e.Difficulty.ToLower() == "hard")
                                  .OrderBy(e => e.StopwatchValue)
                                  .ThenBy(e => e.DateAchieved)
                                  .ToListAsync();

            var viewModel = new LeaderboardViewModel
            {
                EasyEntries = easyEntries,
                MediumEntries = mediumEntries,
                HardEntries = hardEntries
            };

            return View(viewModel);
        }
    }
}
