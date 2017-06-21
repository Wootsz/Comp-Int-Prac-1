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
        //Settings
        static int dims = 0, block_dims = 0;
        static List<int> values;
        static Dictionary<int, int[,]> local_optima;
        static List<Point> fixed_cells, full_blocks;
        static Random random = new Random();

        static int iterations = 1000;
        static int random_steps;
        static int random_steps_input;
        static int plateau_steps;
        static int max_hill_iterations;
        static int max_hill_iterations_o = max_hill_iterations;

        static int[,] reset_state;

        static int hill_steps = 0, random_hill_steps = 0;

        static Stopwatch timer;

        static bool solutionMode = true;

        //Output variables
        static bool output = false;
        static string outputPath = Directory.GetCurrentDirectory() + "\\output" + "\\" + DateTime.Now.ToString("h/mm/ss");

        //Reset all variables to original values
        //SET HERE ALL WANTED VALUES
        static void Reset()
        {
            dims = 0;
            block_dims = 0;
            random_steps = random_steps_input;
            plateau_steps = 3;
            max_hill_iterations = 100;
            hill_steps = 0;
            random_hill_steps = 0;
        }

        static void Main(string[] args)
        {
            
            //Choose puzzel
            string puzzel1 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            string puzzel2 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            string puzzel0 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";

            //a
            string s25251 = "0 0 12 6 0 0 7 0 18 0 5 24 0 10 1 0 0 4 0 0 0 0 0 0 0 2 0 19 0 13 0 0 0 10 0 0 0 0 0 0 0 0 18 5 0 0 0 0 0 1 0 0 0 0 0 0 0 22 0 0 0 0 3 0 2 0 0 14 12 0 16 8 25 0 0 0 16 0 0 0 2 23 0 0 13 12 22 0 0 0 21 15 19 3 0 0 0 0 14 0 23 0 24 0 0 0 0 0 25 8 4 0 16 19 21 0 0 7 0 0 0 3 12 0 9 0 4 0 2 0 0 0 0 0 0 0 10 0 24 12 17 16 0 0 0 5 0 0 0 0 0 0 9 0 0 6 25 0 0 0 8 0 5 3 0 0 0 0 0 0 20 0 0 18 19 15 0 10 11 0 0 0 18 12 19 0 0 0 0 0 0 0 23 0 0 7 0 0 4 0 0 0 0 0 0 0 0 14 0 22 0 0 18 16 20 0 6 11 13 0 0 0 0 0 0 0 22 0 25 0 0 1 17 5 4 7 0 0 14 0 8 3 21 0 0 11 0 0 0 6 0 20 13 15 0 0 0 0 0 0 9 0 0 2 0 25 0 1 8 0 0 5 0 21 0 0 1 0 0 0 0 16 10 0 7 0 0 4 20 0 0 9 0 0 14 0 24 0 17 0 25 2 5 0 0 0 0 0 13 0 0 0 0 0 22 0 0 0 0 0 19 1 8 0 0 0 0 7 21 0 0 12 0 2 17 0 0 0 18 6 16 0 0 15 0 0 13 0 10 0 8 10 18 12 16 9 0 0 0 5 0 0 0 0 19 0 0 17 0 21 0 15 0 0 22 0 8 0 0 15 0 3 0 6 0 21 0 0 7 0 18 14 5 0 1 0 0 0 0 0 0 0 0 19 0 1 0 16 11 0 0 0 10 22 25 15 0 0 0 0 0 0 21 0 0 0 3 1 0 21 0 0 4 0 0 0 0 2 0 13 0 24 25 0 0 14 0 0 6 0 0 0 0 0 0 0 0 15 0 12 14 0 6 17 24 0 0 0 0 0 0 0 13 0 0 0 5 23 16 4 0 13 24 7 2 0 9 0 0 15 3 0 22 0 0 0 0 0 0 8 0 0 25 20 2 0 19 0 0 0 0 1 0 0 0 0 21 3 0 0 12 0 0 0 0 16 12 0 5 0 11 21 0 23 0 0 15 0 0 0 0 19 9 0 0 0 0 0 25 10 0 0 0 0 9 20 22 7 4 0 3 0 14 25 18 0 11 0 0 0 0 0 1 0 15 24 0 6 0 22 8 0 25 14 0 10 11 0 9 0 20 1 16 0 7 0 23 0 0 13 14 13 21 1 0 0 5 0 0 0 6 0 22 0 23 10 0 0 0 2 0 0 18 7 11";
            //b
            string s25252 = "2 15 0 0 21 4 0 0 25 9 0 0 0 0 0 18 24 0 0 20 3 0 0 8 5 25 20 0 0 14 17 0 0 24 6 0 0 0 0 0 5 8 0 0 1 9 0 0 18 4 0 0 7 23 0 0 3 8 0 0 24 10 0 18 4 0 0 19 21 0 0 2 15 0 0 0 0 24 17 0 0 18 16 0 0 13 19 0 23 2 0 0 7 22 0 0 1 12 0 0 3 4 0 0 0 0 0 0 0 0 0 0 16 0 0 0 0 0 0 0 0 0 0 17 10 15 23 0 0 0 16 6 0 0 0 0 0 20 0 0 0 0 0 4 18 0 0 0 11 14 0 0 21 22 0 10 12 0 0 0 0 0 0 0 0 0 0 0 13 11 0 3 9 0 0 0 0 5 1 0 0 0 4 15 0 0 17 18 3 0 0 25 2 0 0 0 21 24 0 0 8 2 0 0 0 0 0 1 22 0 0 13 6 4 0 0 7 23 0 0 0 0 0 16 25 20 3 0 0 0 0 0 0 0 24 8 0 0 0 10 12 0 0 0 0 0 0 0 13 1 0 0 9 20 0 0 0 0 0 10 1 0 0 0 6 24 0 0 0 0 0 22 14 0 0 0 0 1 21 0 0 0 11 13 0 0 0 0 0 0 0 9 16 0 0 0 4 18 0 0 0 0 0 0 25 3 0 2 18 0 0 0 0 0 0 0 17 10 0 22 23 0 0 0 0 0 0 22 10 0 0 0 14 17 0 0 0 0 0 0 0 21 13 0 0 0 24 6 0 0 0 0 19 18 0 0 0 0 0 15 12 0 0 0 9 11 0 0 0 0 0 10 3 0 0 17 1 0 0 0 0 0 0 0 25 23 0 0 0 24 8 0 0 0 0 0 0 0 10 9 10 18 0 0 0 0 0 15 7 0 0 3 25 16 0 0 23 21 0 0 0 0 0 24 2 0 0 11 9 0 0 0 13 4 0 0 18 2 6 0 0 22 15 0 0 0 5 1 0 0 0 0 2 16 0 21 23 0 0 0 0 0 0 0 0 0 0 0 11 4 0 18 25 0 0 19 25 0 0 0 6 22 0 0 0 0 0 11 0 0 0 0 0 9 10 0 0 0 7 16 21 17 0 0 0 0 0 0 0 0 0 0 3 0 0 0 0 0 0 0 0 0 0 9 11 0 0 18 12 0 0 21 23 0 0 19 6 0 8 7 0 0 24 10 0 0 16 5 0 0 0 0 16 4 0 0 9 7 0 0 21 23 0 10 22 0 0 12 20 0 0 25 2 0 0 24 13 0 0 11 18 0 0 20 8 0 0 0 0 0 21 4 0 0 9 1 0 0 3 23 5 22 0 0 23 2 0 0 1 12 0 0 0 0 0 25 15 0 0 8 7 0 0 6 18";

            //16x16 tests

            //a
            string s16161 = "0 15 0 1 0 2 10 14 12 0 0 0 0 0 0 0 0 6 3 16 12 0 8 4 14 15 1 0 2 0 0 0 14 0 9 7 11 3 15 0 0 0 0 0 0 0 0 0 4 13 2 12 0 0 0 0 6 0 0 0 0 15 0 0 0 0 0 0 14 1 11 7 3 5 10 0 0 8 0 12 3 16 0 0 2 4 0 0 0 14 7 13 0 0 5 15 11 0 5 0 0 0 0 0 0 9 4 0 0 6 0 0 0 0 0 0 13 0 16 5 15 0 0 12 0 0 0 0 0 0 0 0 9 0 1 12 0 8 3 10 11 0 15 0 2 12 0 11 0 0 14 3 5 4 0 0 0 0 9 0 6 3 0 4 0 0 13 0 0 11 9 1 0 12 16 2 0 0 10 9 0 0 0 0 0 0 12 0 8 0 6 7 12 8 0 0 16 0 0 10 0 13 0 0 0 5 0 0 5 0 0 0 3 0 4 6 0 1 15 0 0 0 0 0 0 9 1 6 0 14 0 11 0 0 2 0 0 0 10 8 0 14 0 0 0 13 9 0 4 12 11 8 0 0 2 0";
            //b
            string s16162 = "0 9 0 0 0 0 0 14 0 6 0 16 0 0 11 0 0 0 0 0 0 0 0 0 3 0 8 0 0 0 1 16 0 0 0 0 12 10 0 11 0 9 0 0 3 0 0 0 0 0 0 0 0 5 6 1 0 10 15 11 0 0 13 0 7 0 9 0 0 3 0 0 0 0 6 1 0 0 15 11 0 12 0 15 7 4 0 13 14 0 2 0 0 0 0 1 0 0 0 0 0 0 10 0 0 0 0 13 14 0 0 0 0 14 3 0 0 0 0 0 0 0 0 0 0 4 0 0 0 0 4 0 8 0 0 0 0 0 5 6 0 0 0 15 0 11 0 0 0 0 0 0 0 0 0 0 1 0 5 0 6 1 0 0 0 0 12 0 0 0 4 0 8 14 0 0 0 0 0 0 6 0 0 0 15 0 0 10 13 7 0 0 9 0 7 0 0 0 0 3 0 0 0 0 0 0 0 0 10 0 0 0 0 13 7 0 0 0 0 0 0 1 0 5 0 0 0 0 10 0 0 12 0 0 0 4 0 0 14 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 4";
            //c
            string s16163 = "0 0 0 0 8 0 0 0 0 0 0 0 0 0 0 3 3 0 9 0 0 0 0 0 8 0 0 16 0 0 0 0 0 0 0 6 0 0 9 7 0 0 0 0 0 0 0 0 0 0 0 0 0 12 0 0 0 0 0 0 5 2 0 0 4 5 0 0 0 0 0 11 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 6 0 0 0 0 0 0 0 0 5 0 0 0 0 0 0 11 16 0 0 6 0 0 0 0 3 1 0 0 0 0 0 0 0 0 2 11 0 0 0 0 0 12 0 0 0 0 0 0 0 0 0 14 0 5 0 11 0 8 0 0 0 0 0 0 0 0 0 9 7 0 0 0 0 0 0 11 0 0 10 0 0 0 0 0 0 0 0 9 7 3 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 12 0 0 0 0 0 0 0 0 0 14 0 0 0 0 0 0 0 0 13 12 12 0 0 0 0 0 0 0 0 0 0 0 10 0 0 8 8 0 0 16 0 0 0 0 1 0 0 0 0 14 0 0";

            //9x9 tests

            //a
            string s11 = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 3 0 8 5 0 0 1 0 2 0 0 0 0 0 0 0 5 0 7 0 0 0 0 0 4 0 0 0 1 0 0 0 9 0 0 0 0 0 0 0 5 0 0 0 0 0 0 7 3 0 0 2 0 1 0 0 0 0 0 0 0 0 4 0 0 0 9";
            //b
            string s12 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            //c
            string s13 = "0 0 0 1 5 8 0 0 0 0 0 2 0 6 0 8 0 0 0 3 0 0 0 0 0 4 0 0 2 7 0 3 0 5 1 0 0 0 0 0 0 0 0 0 0 0 4 6 0 8 0 7 9 0 0 5 0 0 0 0 0 8 0 0 0 4 0 7 0 1 0 0 0 0 0 3 2 5 0 0 0";
            //d
            string s21 = "0 6 9 0 0 0 0 0 0 0 0 0 0 4 0 0 3 0 0 8 0 0 0 0 0 0 0 0 0 0 8 0 0 6 1 0 4 0 0 0 0 0 9 0 0 5 0 0 0 2 0 0 0 0 2 0 0 9 0 0 0 0 5 0 0 0 6 0 1 0 0 0 0 0 0 0 0 0 0 0 0";
            //e
            string s22 = "0 0 0 0 0 0 0 1 2 8 0 0 0 4 0 0 0 0 0 0 0 0 0 0 0 6 0 0 9 0 2 0 0 0 0 0 7 0 0 0 0 0 4 0 0 0 0 0 5 0 1 0 0 0 0 1 5 0 0 0 0 0 0 0 0 0 0 3 0 9 0 0 6 0 2 0 0 0 0 0 0";
            //f
            string s23 = "0 6 1 0 0 0 8 0 0 0 0 0 3 9 0 0 0 0 0 0 0 0 0 0 0 0 0 0 8 9 0 0 1 0 0 0 5 0 0 0 0 0 0 0 3 0 0 0 0 0 0 0 2 0 2 0 0 4 3 0 0 0 0 0 0 0 2 0 0 0 6 0 0 0 0 0 0 0 1 0 0";

            string[] puzzel_array = s25251.Split();


            //Get mode
            int invalid = 1;
            while (invalid == 1)
            {
                invalid++;
                Console.WriteLine("Do you want to use Solution Mode (1) or Iterations Mode (2)?\nEnter 1 or 2:");
                string[] fc = Console.ReadLine().Split();
                switch (fc[0])
                {
                    case "1": solutionMode = true; break;
                    case "2": solutionMode = false; break;
                    default: Console.WriteLine("Invalid input"); invalid--; break;
                }
            }

            invalid = 1;
            while (invalid == 1)
            {
                invalid++;
                Console.WriteLine("How many random steps: (1, 5, 10, 20, 50)");
                string[] fc = Console.ReadLine().Split();
                switch (fc[0])
                {
                    case "1": random_steps_input = 1; break;
                    case "5": random_steps_input = 5; break;
                    case "10": random_steps_input = 10; break;
                    case "20": random_steps_input = 20; break;
                    case "50": random_steps_input = 50; break;
                    default: Console.WriteLine("Invalid input"); invalid--; break;
                }
            }

            //Create timer
            timer = new Stopwatch();

            //Create outpot if wanted
            if(output)
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

            //Setup first state
            int[,] start_state = init(puzzel_array);

            // Fill the row and column arrays with scores of each row/column
            int[] row_scores = new int[dims], column_scores = new int[dims];
            for (int i = 0; i < dims; i++)
            {
                row_scores[i] = RowScore(start_state, i);
                column_scores[i] = ColumnScore(start_state, i);
            }

            //Data variables
            List<int> score = new List<int>();
            List<double> time = new List<double>();

            //Find solution or timeout
            if (solutionMode)
            {
                Console.WriteLine("Solution mode:\n");
                int solutionScore = -1;

                //Write example start state
                Console.WriteLine("Example start state:");
                WriteState(start_state);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Start search...\n");
                iterations = 0;
                while (solutionScore != 0)
                {
                    if(timer.Elapsed.TotalMinutes >= 5)
                    {
                        Console.WriteLine("Solution not found!");
                        break;
                    }

                    iterations++;

                    //Write progress
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.Write("#");
                    //Console.ForegroundColor = ConsoleColor.White;


                    timer.Start();
                    int[,] end = HillClimbing(start_state, row_scores, column_scores, -1, 0);

                    //Timer stop and record

                    //Write states
                    //WriteState(end);
                    //WriteStats(GetScore(row_scores, column_scores));

                    //Save scores
                    solutionScore = GetScore(row_scores, column_scores);
                    score.Add(solutionScore);

                    if (solutionScore == 0)
                    {
                        Console.WriteLine("Solution Found!\n\n");
                        WriteState(end);
                        WriteStats(GetScore(row_scores, column_scores));
                        break;
                    }

                    //Reset
                    start_state = init(puzzel_array);
                }
                Console.WriteLine("\nIterations: " + iterations + "\n");
                timer.Stop();
            }

            //Save time elapsed till first solution found
            double timeTillZero = -1;

            //Loop for a certain amount of iterations
            if (!solutionMode)
            {
                Console.WriteLine("Iteration Mode:\n");

                //Write example start state
                Console.WriteLine("Example start state:");
                WriteState(start_state);

                //Loop through X amount of iterations
                for (int i = 0; i < iterations; i++)
                {
                    //Write progress
                    //Console.Write("#");

                    timer.Start();
                    int[,] end = HillClimbing(start_state, row_scores, column_scores, -1, 0);

                    //Timer stop and record
                    timer.Stop();
                    time.Add(timer.Elapsed.TotalMilliseconds);
                    timer.Reset();

                    //Write states
                    //WriteState(end);
                    //WriteStats(GetScore(row_scores, column_scores));

                    //Save scores and reset
                    int solutionScore = GetScore(row_scores, column_scores);
                    score.Add(solutionScore);
                    start_state = init(puzzel_array);
                    if(solutionScore == 0)
                    {
                        if(timeTillZero == -1)
                            timeTillZero = time.Sum();
                        Console.WriteLine("Solution Found!\n\n");
                        WriteState(end);
                        WriteStats(GetScore(row_scores, column_scores));
                        solutionScore = -1;
                    }
                }
            }

            //Form output
            string scoreString = ("\nTotal score: " + score.Sum() + "\nAvg score: " + score.Average() + "\nMax: " + score.Max() + "\nMin :" + score.Min());
            string timeString = "";
            if (!solutionMode)
            {
                timeString = ("\nTotal time(Seconds): " + time.Sum() + "\nAvg time: " + time.Average() + "\nMax: " + time.Max() + "\nMin :" + time.Min());
                if (timeTillZero != -1)
                    timeString += ("Time till first solution: " + timeTillZero);
            }
            if (solutionMode)
                timeString = ("\nTotal time till solution(Seconds): " + timer.Elapsed.TotalSeconds);

            //Write output
            Console.WriteLine(scoreString);
            Console.WriteLine(timeString);
            if(output)
                File.WriteAllText(outputPath, scoreString + timeString);

            //Flee you fool
            Console.Read();
        }

        static int[,] init(string[] puzzel_array)
        {
            Reset();
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
            full_blocks = new List<Point>();
            local_optima = new Dictionary<int, int[,]>();
            FillArray(start_state, puzzel_array);
            FillBlocks(start_state);
            reset_state = start_state;
            return start_state;
        }

        static void WriteState(int[,] state)
        {
            for (int i = 0; i < dims; i++)
            {
                for (int j = 0; j < dims; j++)
                {
                    if (fixed_cells.Contains(new Point(i, j)))
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ResetColor();
                    Console.Write(state[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void WriteStats(int score)
        {
            Console.WriteLine("Score: " + score);
            Console.WriteLine();
            Console.WriteLine("Max hill iterations: " + max_hill_iterations_o);
            Console.WriteLine("Max random steps: " + random_steps);
            Console.WriteLine("Max plateau steps: " + plateau_steps);
            Console.WriteLine();
            Console.WriteLine("Local search recursion times: " + hill_steps);
            Console.WriteLine("Random search recursion times: " + random_hill_steps);
            Console.WriteLine("Total recursion times: " + (hill_steps + random_hill_steps));
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

                    // If there is only 1 free space (or 0) in the block, add it to the full_blocks list
                    if (block_values.Count <= 1)
                        full_blocks.Add(new Point(x, y));
                    
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
            Dictionary<int, List<Point>> switches;
            int loops = 0;

            if (max_hill_iterations == 0)
                return state;
            max_hill_iterations--;

            hill_steps++;
            
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
                //if (local_optima.Keys.Contains(new_score) && local_optima.Values.Contains(state))
                //  return state;
                if (local_optima.Keys.Contains(new_score))
                    local_optima[new_score] = state;
                else
                    local_optima.Add(new_score, state);
                int[,] best_optima = local_optima[local_optima.Keys.Min()];
                return RandomWalkHillClimbing(best_optima, row_scores, column_scores, random_steps);
            }

            // Generate a random block coordinate that is not "full" (i.e. has 0 or 1 non-fixed cells)
            int random_x, random_y;
            do
            {
                random_x = random.Next(0, block_dims);
                random_y = random.Next(0, block_dims);
            }
            while (full_blocks.Contains(new Point(random_x, random_y)));

            switches = new Dictionary<int, List<Point>>();

            for (int i = random_x * block_dims; i < random_x * block_dims + block_dims; i++)
            {
                for (int j = random_y * block_dims; j < random_y * block_dims + block_dims; j++)
                {
                    for (int k = random_x * block_dims; k < random_x * block_dims + block_dims; k++)
                    {
                        for (int l = random_y * block_dims; l < random_y * block_dims + block_dims; l++)
                        {
                            // Check if (i, j) != (k, l) and if both aren't in the fixed_cells list
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

                                //Add values to dict
                                addToDict(switches, change_in_score, new Point(i, j), new Point(k, l));

                            }
                        }
                    }
                }
            }

            // Get the best switch
            List<Point> best_switch = switches[switches.Keys.Max()];//switches.First().Value;

            // If the new score is worse than the previous one, we are at an optimum, so go into recursion with n = n
            if (new_score - switches.Keys.Max() > new_score)
                return HillClimbing(state, row_scores, column_scores, prev_score, plateau_steps);

            // Execute switch
            Point a = best_switch[0], b = best_switch[1];
            //Console.WriteLine("a: " + a + " b: " + b);
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
            // End condition
            if (steps <= 0)
                return HillClimbing(state, row_scores, column_scores, GetScore(row_scores, column_scores), 0);

            random_hill_steps++;

            // Generate a random block coordinate that is not "full" (i.e. has 0 or 1 non-fixed cells)
            int random_x, random_y;
            do
            {
                random_x = random.Next(0, block_dims);
                random_y = random.Next(0, block_dims);
            }
            while (full_blocks.Contains(new Point(random_x, random_y)));

            // Generate 4 random integers (the switch coordinates)
            List<int> r = GetRandomCo(random_x, random_y);
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

        // Generate 4 integers / 2 coordinates that are not fixed or the same, and within a single block
        static List<int> GetRandomCo(int random_x, int random_y)
        {
            // Generate 4 random coordinates in the given block (random_x, random_y)
            int random_i = random.Next(random_x * block_dims, random_x * block_dims + block_dims);
            int random_j = random.Next(random_y * block_dims, random_y * block_dims + block_dims);
            int random_k = random.Next(random_x * block_dims, random_x * block_dims + block_dims);
            int random_l = random.Next(random_y * block_dims, random_y * block_dims + block_dims);

            // Then check:
            // - That (i, j) != (k, l)
            // - That (i, j) or (k, l) is not a fixed coordinate
            if ((random_i == random_k && random_j == random_l) || fixed_cells.Contains(new Point(random_i, random_j)) || fixed_cells.Contains(new Point(random_k, random_l)))
                return GetRandomCo(random_x, random_y);

            // Return i, j, k and l if these conditions hold. Otherwise recurse and select cell-coordinates
            return new List<int> { random_i, random_j, random_k, random_l };
        }

        static void addToDict(Dictionary<int, List<Point>> switches, int change_in_score, Point point1, Point point2)
        {
            if (switches != null && switches.Keys.Contains(change_in_score))
            {
                switches[change_in_score].Add(point1);
                switches[change_in_score].Add(point2);
            }
            else
            {
                switches.Add(change_in_score, new List<Point> { point1, point2 });
            }
        }
    }
}