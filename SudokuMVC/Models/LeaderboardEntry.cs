using System;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class LeaderboardEntry
    {
        public int Id { get; set; }
        
        [Required]
        public string PlayerName { get; set; } = string.Empty;
        
        [Required]
        public string Difficulty { get; set; } = string.Empty;
        
        // Use StopwatchValue to store the player's time (in seconds)
        [Required]
        public int StopwatchValue { get; set; }
        
        [Required]
        public DateTime DateAchieved { get; set; }
    }
}
