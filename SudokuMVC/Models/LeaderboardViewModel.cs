using System.Collections.Generic;

namespace YourProjectNamespace.Models
{
    public class LeaderboardViewModel
    {
        public IEnumerable<LeaderboardEntry> EasyEntries { get; set; } = new List<LeaderboardEntry>();
        public IEnumerable<LeaderboardEntry> MediumEntries { get; set; } = new List<LeaderboardEntry>();
        public IEnumerable<LeaderboardEntry> HardEntries { get; set; } = new List<LeaderboardEntry>();
    }
}
