using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using System.Diagnostics;
using System.IO;

namespace Prac2
{
    class Program
    {
        static int dims = 0, block_dims = 0;
        static int score = 0;
        static List<int> values;

        static void Main(string[] args)
        {
            string puzzel1 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            string puzzel2 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            string[] puzzel_array = puzzel2.Split();

            // Determine the puzzle dimensions based on the puzzel_array
            dims = (int)Math.Sqrt(puzzel_array.Length);
            block_dims = (int)Math.Sqrt(dims);

            // Fill the values list: a list that containt every value that needs to be in every row, column and block
            values = new List<int>();
            for (int v = 1; v <= dims; v++)
                values.Add(v);

            // Fil an array (start_array) with values from the puzzel_array and the other needed values in the blocks
            int[,] start_state = new int[dims, dims];
            FillArray(start_state, puzzel_array);
            FillBlocks(start_state);

            // Fill the row and column arrays with scores of each row/column
            int[] row_scores = new int[dims], column_scores = new int[dims];
            for (int i = 0; i < dims; i++)
            {
                row_scores[i] = RowScore(start_state, i);
                column_scores[i] = ColumnScore(start_state, i);
            }

            WriteState(start_state);
            Console.ReadLine();
            HillClimbing(start_state, row_scores, column_scores);
        }

        static void WriteState(int[,] field)
        {
            for (int i = 0; i < dims; i++)
            {
                for (int j = 0; j < dims; j++)
                {
                    Console.Write(field[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void FillArray(int[,] state, string[] puzzel_array)
        {
            // Fill a 2D array with the values from the string[] above
            int c = 0;
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                {
                    state[i, j] = int.Parse(puzzel_array[c]);
                    c++;
                }
        }

        static void FillBlocks(int[,] state)
        {
            // Foreach block:
            for (int x = 0; x < block_dims; x++)
                for (int y = 0; y < block_dims; y++)
                {
                    List<int> block_values = new List<int>(values);
                    // First, determine what values you still need to fill into the block
                    for (int i = x * block_dims; i < x * block_dims + block_dims; i++)
                        for (int j = y * block_dims; j < y * block_dims + block_dims; j++)
                            if (state[i, j] != 0)
                                block_values.Remove(state[i, j]);
                    // Then assign values to each cell in the block
                    for (int i = x * block_dims; i < x * block_dims + block_dims; i++)
                        for (int j = y * block_dims; j < y * block_dims + block_dims; j++)
                            if (state[i, j] == 0)
                            {
                                state[i, j] = block_values[0];
                                block_values.RemoveAt(0);
                            }
                }
        }

        // RowScore and ColumnScore determine for a row/column how many values are missing
        static int RowScore(int[,] state, int y)
        {
            List<int> new_values = new List<int>(values);
            int row_score = dims;
            for (int i = 0; i < dims; i++)
                if (new_values.Contains(state[i, y]))
                {
                    row_score--;
                    new_values.Remove(state[i, y]);
                }
            return row_score;
        }
        static int ColumnScore(int[,] state, int x)
        {
            List<int> new_values = new List<int>(values);
            int column_score = dims;
            for (int j = 0; j < dims; j++)
                if (new_values.Contains(state[x, j]))
                {
                    column_score--;
                    new_values.Remove(state[x, j]);
                }
            return column_score;
        }
        // Method that returns the total score
        static int GetScore(int[] row_scores, int[] column_scores)
        {
            int score = 0;
            for (int i = 0; i < dims; i++)
                score += column_scores[i] + row_scores[i];
            return score;
        }

        static int[,] HillClimbing(int[,] state, int[] row_scores, int[] column_scores)
        {
            if (GetScore(row_scores, column_scores) == 0)
                return state;

            Random random = new Random();
            int random_x = random.Next(0, block_dims), random_y = random.Next(0, block_dims);

            SortedDictionary<int, Point[]> switches = new SortedDictionary<int, Point[]>();

            for (int i = random_x * block_dims; i < random_x * block_dims + block_dims; i++)
                for (int j = random_y; j < random_y * block_dims + block_dims; j++)
                    for (int k = random_x * block_dims; i < random_x * block_dims + block_dims; i++)
                        for (int l = random_y; j < random_y * block_dims + block_dims; j++)
                            if (!(i == k && j == l)) // && i,j en k,l staan niet al samen in switches
                            {
                                // Make a new state
                                int[,] new_state = (int[,])state.Clone();
                                // Switch the values
                                int v = new_state[i, j];
                                new_state[i, j] = new_state[k, l];
                                new_state[k, l] = v;

                                // Calculate the change in score
                                //Row score
                                int new_row_score1 = 0, new_row_score2 = 0;
                                if (j != l)
                                {
                                    new_row_score1 = row_scores[j] - RowScore(new_state, j);
                                    new_row_score2 = row_scores[l] - RowScore(new_state, l);
                                }

                                //Column score
                                int new_column_score1 = 0, new_column_score2 = 0;
                                if (i != k)
                                {
                                    new_column_score1 = column_scores[i] - ColumnScore(new_state, i);
                                    new_column_score2 = column_scores[k] - ColumnScore(new_state, k);
                                }

                                int change_in_score = new_row_score1 + new_row_score2 + new_column_score1 + new_column_score2;
                                // Add the switch to the switches list
                                switches.Add(change_in_score, new Point[2] { new Point(i, j), new Point(k, l) });
                            }

            Point[] best_switch = switches.First().Value;
            return null;
        }
    }
}