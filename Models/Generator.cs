using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YourProjectNamespace.Models
{
    public class Generator
    {
        // Expose these as public properties with setters.
        [JsonInclude]
        public byte[][] Solved { get; set; }
        [JsonInclude]
        public byte[][] Given { get; set; }

        private byte numberOfEmptyCells;

        // Parameterless constructor for deserialization.
        public Generator()
        {
            // Initialize the arrays.
            Solved = new byte[9][];
            Given = new byte[9][];
            for (byte i = 0; i < 9; i++)
            {
                Solved[i] = new byte[9];
                Given[i] = new byte[9];
            }
        }

        // Method that creates a new game based on difficulty.
        public void NewGame(string difficulty)
        {
            Random r = new Random();

            // Generate a solved sudoku matrix.
            Generate();

            // Generate random indices for empty cells.
            byte[] randomIndices = new byte[81];
            for (byte i = 0; i < 81; ++i)
            {
                randomIndices[i] = i;
            }
            for (byte i = 80; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                randomIndices[i] ^= randomIndices[p];
                randomIndices[p] ^= randomIndices[i];
                randomIndices[i] ^= randomIndices[p];
            }

            // Set difficulty by number of revealed digits.
            switch (difficulty)
            {
                case "Easy":
                    numberOfEmptyCells = (byte)(81 - 45);
                    break;
                case "Medium":
                    numberOfEmptyCells = (byte)(81 - 30);
                    break;
                case "Hard":
                    numberOfEmptyCells = (byte)(81 - 20);
                    break;
            }

            // Copy solved puzzle into given puzzle.
            for (byte i = 0; i < 9; i++)
                for (byte j = 0; j < 9; j++)
                {
                    Given[i][j] = Solved[i][j];
                }

            // Remove numbers from the given puzzle.
            for (byte i = 0; i < numberOfEmptyCells; ++i)
            {
                Given[randomIndices[i] / 9][randomIndices[i] % 9] = 0;
            }
        }

        // Generates a solved sudoku puzzle.
        private bool Generate()
        {
            Random r = new Random();
            var digits = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            // Reinitialize the solved array.
            for (byte i = 0; i < 9; ++i)
            {
                Solved[i] = new byte[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            // Initialize a random first row.
            for (byte i = 8; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                byte temp = digits[i];
                digits[i] = digits[p];
                digits[p] = temp;
            }
            for (byte i = 0; i < 9; ++i)
            {
                Solved[0][i] = digits[i];
            }

            // Initialize a random 3x3 block.
            digits.Remove(Solved[0][0]);
            digits.Remove(Solved[0][1]);
            digits.Remove(Solved[0][2]);
            for (byte i = 5; i > 1; --i)
            {
                byte p = (byte)r.Next(i);
                byte temp = digits[i];
                digits[i] = digits[p];
                digits[p] = temp;
            }
            for (byte i = 0; i < 6; ++i)
            {
                Solved[i / 3 + 1][i % 3] = digits[i];
            }

            // Fill the rest using recursive backtracking.
            return Solve(Solved, 0, 0);
        }

        // Recursive backtracking to fill the grid.
        private bool Solve(byte[][] solvedMatrix, byte row, byte col)
        {
            if (row == 9) return true;

            List<byte> possibleDigits = Possible(solvedMatrix, row, col);

            if (solvedMatrix[row][col] > 0)
            {
                return col == 8 ? Solve(solvedMatrix, (byte)(row + 1), 0)
                                : Solve(solvedMatrix, row, (byte)(col + 1));
            }

            for (byte i = 0; i < possibleDigits.Count; ++i)
            {
                solvedMatrix[row][col] = possibleDigits[i];
                if (col == 8 ? Solve(solvedMatrix, (byte)(row + 1), 0)
                             : Solve(solvedMatrix, row, (byte)(col + 1)))
                {
                    return true;
                }
            }
            solvedMatrix[row][col] = 0;
            return false;
        }

        // Returns a list of possible digits for the cell.
        private List<byte> Possible(byte[][] solvedMatrix, byte row, byte col)
        {
            List<byte> possible = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Remove digits from the same row and column.
            for (byte i = 0; i < 9; ++i)
            {
                if (i != col && solvedMatrix[row][i] > 0)
                    possible.Remove(solvedMatrix[row][i]);
                if (i != row && solvedMatrix[i][col] > 0)
                    possible.Remove(solvedMatrix[i][col]);
            }
            // Remove digits from the same 3x3 block.
            byte startRow = (byte)(row / 3 * 3);
            byte startCol = (byte)(col / 3 * 3);
            for (byte i = startRow; i < startRow + 3; ++i)
            {
                for (byte j = startCol; j < startCol + 3; ++j)
                {
                    if (i == row || j == col) continue;
                    if (solvedMatrix[i][j] > 0)
                        possible.Remove(solvedMatrix[i][j]);
                }
            }
            return possible;
        }
    }
}
