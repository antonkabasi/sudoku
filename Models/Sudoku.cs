using System;
using System.Text.Json.Serialization;

namespace YourProjectNamespace.Models
{
    // This class stores the puzzle state.
    public class Sudoku : Generator
    {
        [JsonInclude]
        
        public byte[][] UserGrid { get; set; }
        
        [JsonInclude]
        public string Difficulty { get; set; }

        // Parameterized constructor for creating a new puzzle.
        public Sudoku(string difficulty)
        {
            Difficulty = difficulty;
            Initialize();
            NewGame(difficulty);
        }

        // Parameterless constructor for JSON deserialization.
        public Sudoku()
        {
            Initialize();
        }

        // Initializes the user grid.
        private void Initialize()
        {
            UserGrid = new byte[9][];
            for (byte i = 0; i < 9; i++)
            {
                UserGrid[i] = new byte[9];
            }
        }

        // Updates the user grid.
        public void UpdateUserGrid(byte[][] grid)
        {
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    UserGrid[i][j] = grid[i][j];
                }
            }
        }

        // Checks if the user grid matches the solved puzzle.
        public bool Check()
        {
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    // If a cell is empty or doesn't match the solved value, return false.
                    if (UserGrid[i][j] == 0 || UserGrid[i][j] != Solved[i][j])
                        return false;
                }
            }
            return true;
        }

        public string CheckStatus()
        {
            // First, check if any cell is empty.
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    if (UserGrid[i][j] == 0)
                    {
                        return "GameNotOver";
                    }
                }
            }
            // If board is complete, check for incorrect entries.
            for (byte i = 0; i < 9; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    if (UserGrid[i][j] != Solved[i][j])
                    {
                        return "SomeIncorrect";
                    }
                }
            }
            return "Correct";
        }
    }
}
