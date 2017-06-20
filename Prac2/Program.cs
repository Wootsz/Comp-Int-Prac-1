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
        static List<int> values;
        static int random_steps = 20;
        static int plateau_steps = 3;
        static List<Point> fixed_cells;
        static List<KeyValuePair<int, int[,]>> local_optima;
        static Random random = new Random();

        static void Main(string[] args)
        {
            //string puzzel1 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
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
            fixed_cells = new List<Point>();
            local_optima = new List<KeyValuePair<int, int[,]>>();
            FillArray(start_state, puzzel_array);
            FillBlocks(start_state);

            // Fill the row and column arrays with scores of each row/column
            int[] row_scores = new int[dims], column_scores = new int[dims];
            for (int i = 0; i < dims; i++)
            {
                row_scores[i] = RowScore(start_state, i);
                column_scores[i] = ColumnScore(start_state, i);
            }

            RandomWalkHillClimbing(start_state, row_scores, column_scores, 10);
            //HillClimbing(start_state, row_scores, column_scores, -1, 0);
            Console.Write("Finished");
            Console.Read();
        }

        static void WriteState(int[,] field)
        {
            for (int i = 0; i < dims; i++)
            {
                for (int j = 0; j < dims; j++)
                {
                    if (fixed_cells.Contains(new Point(i, j)))
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ResetColor();
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
                    if (puzzel_array[c] != "0")
                        fixed_cells.Add(new Point(i, j));
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
                    Random random = new Random();
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
                                int r = random.Next(block_values.Count);
                                state[i, j] = block_values[r];
                                block_values.RemoveAt(r);
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

        static int[,] HillClimbing(int[,] state, int[] row_scores, int[] column_scores, int prev_score, int n)
        {
            WriteState(state);
            int new_score = GetScore(row_scores, column_scores);
            // If new_score == 0, it means we have found the solution
            if (new_score == 0)
                return state;
            // If the new score is equal to the prev_score, we are at a platuea, so increase n by 1
            if (new_score == prev_score)
                n++;
            // If n >= plateasu steps, we are in a local optimum or on a plateau for n steps
            if (n >= plateau_steps)
            {
                // If we are at the same local optimum again, stop the algorithm and return the state
                if (new_score == local_optima.Last().Key && state == local_optima.Last().Value)
                    return state;
                local_optima.Add(new KeyValuePair<int, int[,]>(new_score, state));
                return RandomWalkHillClimbing(state, row_scores, column_scores, random_steps);
            }
            Random random = new Random();
            int random_x = random.Next(0, block_dims), random_y = random.Next(0, block_dims);

            SortedDictionary<int, List<Point>> switches = new SortedDictionary<int, List<Point>>();

            for (int i = random_x * block_dims; i < random_x * block_dims + block_dims; i++)
                for (int j = random_y; j < random_y * block_dims + block_dims; j++)
                    for (int k = random_x * block_dims; i < random_x * block_dims + block_dims; i++)
                        for (int l = random_y; j < random_y * block_dims + block_dims; j++)
                            if (!(i == k && j == l) && (!fixed_cells.Contains(new Point(i, j)) || !fixed_cells.Contains(new Point(k, l))))
                            {
                                // Make a new state
                                int[,] new_state = (int[,])state.Clone();

                                // Switch the values
                                int v = new_state[i, j];
                                new_state[i, j] = new_state[k, l];
                                new_state[k, l] = v;

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

                                // Calculate the change in score
                                int change_in_score = new_row_score1 + new_row_score2 + new_column_score1 + new_column_score2;
                                // Add the switched points to the switches list
                                if (switches.Keys.Contains(change_in_score))
                                {
                                    switches[change_in_score].Add(new Point(i, j));
                                    switches[change_in_score].Add(new Point(k, l));
                                }
                                else
                                    switches.Add(change_in_score, new List<Point> { new Point(i, j), new Point(k, l) });

                            }

            // Get the best switch
            List<Point> best_switch = switches.First().Value;

            // If the new score is worse than the previous one, we are at an optimum, so go into recursion with n = n
            if (switches.First().Key > prev_score)
                return HillClimbing(state, row_scores, column_scores, new_score, n);

            // Execute switch
            Point a = best_switch[0], b = best_switch[1];
            Console.WriteLine("a: " + a + " b: " + b);
            int v2 = state[a.X, a.Y];
            state[a.X, a.Y] = state[b.X, b.Y];
            state[b.X, b.Y] = v2;

            // Calculate new scores for the rows and columns that have changed
            row_scores[a.Y] = RowScore(state, a.Y);
            row_scores[b.Y] = RowScore(state, b.Y);
            column_scores[a.X] = ColumnScore(state, a.X);
            column_scores[b.X] = ColumnScore(state, b.X);

            return HillClimbing(state, row_scores, column_scores, new_score, n);
        }

        static int[,] RandomWalkHillClimbing(int[,] state, int[] row_scores, int[] column_scores, int steps)
        {
            Console.WriteLine("RANDOM");
            WriteState(state);

            // End condition
            if (steps <= 0)
                HillClimbing(state, row_scores, column_scores, GetScore(row_scores, column_scores), 0);

            // Generate a random block coordinate
            int random_x = random.Next(0, block_dims), random_y = random.Next(0, block_dims);
            // Generate 4 random integers (the switch coordinates)
            List<int> r = GetRandomCo(random_x, random_y, block_dims);
            int i = r[0], j = r[1], k = r[2], l = r[3];

            // Do the switch with the two random coordinates values
            int v2 = state[i, j];
            state[i, j] = state[k, l];
            state[k, l] = v2;

            // Calculate new scores for the rows and columns that have changed
            row_scores[j] = RowScore(state, j);
            row_scores[l] = RowScore(state, l);
            column_scores[i] = ColumnScore(state, i);
            column_scores[k] = ColumnScore(state, k);

            return RandomWalkHillClimbing(state, row_scores, column_scores, --steps);
        }

        static List<int> GetRandomCo(int random_x, int random_y, int block_dims)
        {
            int random_i = random.Next(random_x * block_dims, random_x * block_dims + block_dims);
            int random_j = random.Next(random_y * block_dims, random_y * block_dims + block_dims);
            int random_k = random.Next(random_x * block_dims, random_x * block_dims + block_dims);
            int random_l = random.Next(random_y * block_dims, random_y * block_dims + block_dims);
            if ((random_i == random_k && random_j == random_l) || fixed_cells.Contains(new Point(random_i, random_j)) || fixed_cells.Contains(new Point(random_k, random_l)))
                return GetRandomCo(random_x, random_y, block_dims);
            return new List<int> { random_i, random_j, random_k, random_l };
        }
    }
}