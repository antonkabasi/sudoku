namespace YourProjectNamespace.Models
{
    public class SudokuGame
    {
        public int[,] Board { get; set; }

        
        public required string Difficulty { get; set; }
        public DateTime StartTime { get; set; }

        public SudokuGame()
        {
            Board = new int[9, 9];
            ClearBoard();
            StartTime = DateTime.UtcNow; // Set the game start time
        }

        public void ClearBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Board[i, j] = 0;
                }
            }
        }

        // Generates a puzzle based on the difficulty level.
        public void GeneratePuzzle(string difficulty)
        {
            Difficulty = difficulty;
            ClearBoard();

            // Example stub: fill a certain number of cells based on difficulty
            int cellsToFill = difficulty.ToLower() switch
            {
                "easy" => 40,
                "medium" => 30,
                "hard" => 20,
                _ => 30,
            };

            Random rnd = new Random();
            for (int n = 0; n < cellsToFill; n++)
            {
                int i = rnd.Next(0, 9);
                int j = rnd.Next(0, 9);
                Board[i, j] = rnd.Next(1, 10);
            }
        }
    }
}
