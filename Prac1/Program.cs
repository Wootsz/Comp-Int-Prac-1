using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using System.Diagnostics;
using System.IO;

namespace Prac1
{
    public class Program
    {
        static bool forward_checking = true;
        static int dims;                        // Dimensions of the sudoku
        static int expansion_type;              // Type of expansion for the state (1-3)
        static Stack<Point> sorted_domain_sizes;

        //Paths
        static string inputPath = Directory.GetCurrentDirectory() + "\\input";
        static string outputPath = Directory.GetCurrentDirectory() + "\\output" + "\\" + DateTime.Now.ToString("h/mm/ss");

        static bool file;

        static Stopwatch timer;
        //Streamreaders
        static StreamReader reader;

        //Stats
        static int calls = 0;
        static string solution;

        static void Main(string[] args)
        {
            //File reading
            if (!Directory.Exists(inputPath))
                Directory.CreateDirectory(inputPath);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            string domain_output = "";
            try
            {
                reader = new StreamReader(inputPath + "\\testFile.txt");
            }
            catch
            {
                file = false;
                Console.WriteLine("File not found\nUsing defalt input");

                //Environment.Exit(0);
            }

            string input = "";

            //Read line in file
            if (file)
                while (!reader.EndOfStream)
                    input += reader.ReadLine();

            // The sudoku to solve, in a string format (0 = empty cell)

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

            //Currenty used input
            string[] start = s11.Split();

            //Dimensions
            dims = (int)Math.Sqrt(start.Length);

            // Select the algorithm you want to use:
            int invalid = 1;
            while (invalid == 1)
            {
                invalid++;
                Console.WriteLine("Do you want to use Chronological Backtracking (1) or Forward Checking (2)?\nEnter 1 or 2:");
                string[] fc = Console.ReadLine().Split();
                switch (fc[0])
                {
                    case "1": forward_checking = false; break;
                    case "2": forward_checking = true; break;
                    default: Console.WriteLine("Invalid input"); invalid--; break;
                }
            }

            if (!forward_checking)
            {
                // Fill a 2D array with the values from the string[] above
                int[,] start_state = new int[dims, dims];
                int c = 0;
                for (int i = 0; i < dims; i++)
                    for (int j = 0; j < dims; j++)
                    {
                        start_state[i, j] = int.Parse(start[c]);
                        c++;
                    }

                // Get the expansion type
                Console.WriteLine("Select order of expansion:\n" +
                    "1 = From left to right and top to bottom\n" +
                    "2 = The other way around: From right to left and bottom to top\n" +
                    "3 = From lowest domain size to biggest\n" +
                    "---------------------------------------------------------------");
                Console.Write("Number: ");
                expansion_type = int.Parse(Console.ReadLine());

                // Get a list of every point paired with its domain size
                if (expansion_type == 3)
                {
                    List<KeyValuePair<int, Point>> domain_sizes = DomainSort(start_state);

                    // Sort the list
                    domain_sizes = domain_sizes.OrderBy(x => x.Key).ToList();
                    domain_sizes.Reverse();

                    // Transform it into a stack
                    sorted_domain_sizes = new Stack<Point>((from pair in domain_sizes select pair.Value).ToList());

                }

                // Make a stack and put the start_state on top
                Stack<int[,]> stack = new Stack<int[,]>();
                stack.Push(start_state);

                // Make a timer to measure the execution time
                timer = new Stopwatch();
                timer.Start();

                // Do the backtracking search
                int[,] end = BackTracking(stack);

                // If there is a solution...
                if (end != null)
                {
                    // Write the solution to the console
                    Console.WriteLine("Solution:");
                    for (int i = 0; i < dims; i++)
                    {
                        for (int j = 0; j < dims; j++)
                        {
                            Console.Write(end[i, j] + " ");
                            solution += end[i, j] + " ";
                        }

                        Console.Write("\n");
                        solution += "\n";
                    }
                }
                // Otherwise, write an appropriate message
                else Console.WriteLine("Could not find a solution");
            }
            else
            {
                // The standard domain for each variable
                List<int> domain = new List<int>();
                for (int i = 1; i <= dims; i++)
                    domain.Add(i);

                // Fill the start_state with keyvaluepairs
                KeyValuePair<int, List<int>>[,] start_state = new KeyValuePair<int, List<int>>[dims, dims];
                int c = 0;
                for (int i = 0; i < dims; i++)
                    for (int j = 0; j < dims; j++)
                    {
                        if (start[c] == "0")
                            start_state[i, j] = new KeyValuePair<int, List<int>>(0, domain);
                        else
                            start_state[i, j] = new KeyValuePair<int, List<int>>(int.Parse(start[c]), null);
                        c++;
                    }

                // Adjust all domains
                for (int j = 0; j < dims; j++)
                    for (int i = 0; i < dims; i++)
                        if (start_state[i, j].Key != 0)
                            start_state = AdjustDomains(start_state, i, j);

                // DEBUG
                domain_output = "Domain:\n";
                for (int i = 0; i < dims; i++)
                {
                    for (int j = 0; j < dims; j++)
                        if (start_state[i, j].Value == null)
                            domain_output += "- ";
                        else
                            domain_output += start_state[i, j].Value.Count + " ";
                    domain_output += "\n";
                }
                domain_output += "\n";

                // Make the stack
                Stack<KeyValuePair<int, List<int>>[,]> stack = new Stack<KeyValuePair<int, List<int>>[,]>();
                stack.Push(start_state);

                // Make a timer to measure the execution time
                timer = new Stopwatch();
                timer.Start();

                // Do the algorithm
                KeyValuePair<int, List<int>>[,] end = ForwardChecking(stack);

                // If there is a solution...
                if (end != null)
                {
                    // Write the solution to the console
                    Console.WriteLine("Solution:");
                    for (int i = 0; i < dims; i++)
                    {
                        for (int j = 0; j < dims; j++)
                        {
                            Console.Write(end[i, j].Key + " ");
                            solution += end[i, j].Key + " ";
                        }

                        Console.Write("\n");
                        solution += "\n";
                    }
                }
                // Otherwise, write an appropriate message
                else Console.WriteLine("Could not find a solution");
            }

            //Stats
            Console.WriteLine("\n\nStatistics:");
            Console.WriteLine("Amount of recursion calls: " + calls);
            timer.Stop();
            Console.WriteLine("Time elapsed: " + timer.Elapsed);
            Console.WriteLine("Method used: " + expansion_type);

            //Print results to file
            string outputString = domain_output + "Solution:\n\n" + solution + "\nStatistics:\nAmount of recursion calls: " + calls + "\nTime elapsed: " + timer.Elapsed + "\nMethod used: " + expansion_type;
            File.WriteAllText(outputPath + "\\testOutputResults.txt", outputString);

            Console.ReadLine();
        }

        // We sort the cells in the field by storing each cel in a list as value, paired with its domain size (possible_numbers)
        static List<KeyValuePair<int, Point>> DomainSort(int[,] field)
        {
            List<KeyValuePair<int, Point>> list = new List<KeyValuePair<int, Point>>();

            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                {
                    if (field[i, j] == 0)
                    {
                        int possible_numbers = 0;
                        int[,] prev_state = (int[,])field.Clone();
                        int[,] next_state;
                        for (int n = 1; n <= dims; n++)
                        {
                            next_state = prev_state;
                            next_state[i, j] = n;
                            if (ValidState(next_state, i, j))
                                possible_numbers++;
                        }
                        list.Add(new KeyValuePair<int, Point>(possible_numbers, new Point(i, j)));
                    }
                }

            return list;
        }

        static int[,] BackTracking(Stack<int[,]> stack)
        {
            //Keep yo stats
            calls++;

            if (stack.Count == 0)
                return null;
            else
            {
                int[,] state = stack.Pop();

                // Check if the current state is a goal state
                if (Goal(state))
                    return state;
                // If not, do backtracking with sucessor states
                else
                {
                    // Find the next empty cell
                    Point cell = FindEmptyCell(state);
                    // If there are no empty cells, return null
                    if (cell.X == -1 && cell.Y == -1)
                        return null;

                    int[,] next_state, prev_state = (int[,])state.Clone();

                    for (int n = 1; n <= dims; n++)
                    {
                        next_state = prev_state;
                        next_state[cell.X, cell.Y] = n;
                        if (ValidState(next_state, cell.X, cell.Y))
                        {
                            stack.Push(next_state);
                            int[,] end = BackTracking(stack);
                            if (end != null)
                                return end;
                            //else if (expansion_type == 3 && ValidState(next_state, cell.X, cell.Y))

                        }
                    }
                    if (expansion_type == 3)
                        sorted_domain_sizes.Push(cell);
                }
            }
            return null;
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

        // Loops through the cells and returns the first/last cell it finds with value 0
        static Point FindEmptyCell(int[,] field)
        {
            if (expansion_type == 1)
            {
                for (int i = 0; i < dims; i++)
                    for (int j = 0; j < dims; j++)
                        if (field[i, j] == 0)
                            return new Point(i, j);
            }
            else if (expansion_type == 2)
            {
                for (int i = dims - 1; i >= 0; i--)
                    for (int j = dims - 1; j >= 0; j--)
                        if (field[i, j] == 0)
                            return new Point(i, j);
            }
            else if (expansion_type == 3)
            {
                // Take the top value of the list and remove it from the list
                if (sorted_domain_sizes.Count > 0)
                {
                    Point cell = sorted_domain_sizes.Pop();
                    return cell;
                }
            }
            return new Point(-1, -1);
        }

        // Check if a state is the goal state
        static bool Goal(int[,] field)
        {
            for (int i = dims - 1; i >= 0; i--)
                for (int j = dims - 1; j >= 0; j--)
                    if (field[i, j] == 0 || !ValidState(field, i, j))
                        return false;
            return true;
        }

        // Check if a state is valid
        static bool ValidState(int[,] field, int x, int y)
        {
            int number = field[x, y];

            // Check the rows and colums
            for (int i = 0; i < dims; i++)
                if (i != x && field[i, y] == number)
                    return false;
            for (int j = 0; j < dims; j++)
                if (j != y && field[x, j] == number)
                    return false;

            // Get the block location and dimensions
            int blockdims = (int)Math.Sqrt(dims);
            int block_x = x / blockdims, block_y = y / blockdims;

            // Check the blocks
            for (int i = block_x * blockdims; i < block_x * blockdims + blockdims; i++)
                for (int j = block_y * blockdims; j < block_y * blockdims + blockdims; j++)
                    if ((i != x && j != y) && field[i, j] == number)
                        return false;

            return true;
        }

        static KeyValuePair<int, List<int>>[,] ForwardChecking(Stack<KeyValuePair<int, List<int>>[,]> stack)
        {
            //Keep yo stats 2
            calls++;

            if (stack.Count == 0)
                return null;
            else
            {
                KeyValuePair<int, List<int>>[,] state = stack.Pop();

                // Check if the current state is a goal state
                if (Goal(ToIntArray(state)))
                    return state;
                // If not, do backtracking with sucessor states
                else
                {
                    // Find the next empty cell with the smallest domain size using the MostConstrainedVariable heuristic
                    Point cell = MostConstrainedVariable(state);
                    // If there are no empty cells, return null
                    if (cell.X == -1 && cell.Y == -1)
                        return null;

                    KeyValuePair<int, List<int>>[,] next_state, prev_state = (KeyValuePair<int, List<int>>[,])state.Clone();
                    List<int> domain = state[cell.X, cell.Y].Value;

                    for (int i = 0; i < domain.Count; i++)
                    {
                        int n = domain[i];
                        next_state = prev_state;
                        next_state[cell.X, cell.Y] = new KeyValuePair<int, List<int>>(n, next_state[cell.X, cell.Y].Value);
                        // Adjust the domains of all cells and check if none of them are empty
                        KeyValuePair<int, List<int>>[,] next_state_clone = (KeyValuePair<int, List<int>>[,])next_state.Clone();
                        KeyValuePair<int, List<int>>[,] adjusted_next_state = AdjustDomains(next_state_clone, cell.X, cell.Y);
                        if (!ExistsEmptyDomain(adjusted_next_state))
                        {
                            stack.Push(adjusted_next_state);
                            KeyValuePair<int, List<int>>[,] end = ForwardChecking(stack);
                            if (end != null)
                                return end;
                        }
                    }
                }
            }
            return null;
        }

        // Transform our array of KeyValuePairs to an int array, so we can use old functions
        static int[,] ToIntArray(KeyValuePair<int, List<int>>[,] state)
        {
            int[,] state2 = new int[dims, dims];
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                    state2[i, j] = state[i, j].Key;
            return state2;
        }

        // Checks if there is at least one cell with an empty domain
        static bool ExistsEmptyDomain(KeyValuePair<int, List<int>>[,] state)
        {
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                    if (state[i, j].Key == 0 && (state[i, j].Value.Count == 0 || state[i, j].Value == null))
                        return true;
            return false;
        }

        // Returns the location of the cell with the lowest domain size 
        static Point MostConstrainedVariable(KeyValuePair<int, List<int>>[,] state)
        {
            int smallest_constraint = dims + 1;
            Point result = new Point(0, 0);
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                    if (state[i, j].Key == 0)
                    {
                        int state_constraint = state[i, j].Value.Count;
                        if (state_constraint < smallest_constraint)
                        {
                            smallest_constraint = state_constraint;
                            result = new Point(i, j);
                        }
                    }
            return result;
        }

        // Adjust the domains of all cells that share a constraint with cell (x,y)
        static KeyValuePair<int, List<int>>[,] AdjustDomains(KeyValuePair<int, List<int>>[,] field, int x, int y)
        {
            int number = field[x, y].Key;

            // Adjust the rows and colums
            for (int i = 0; i < dims; i++)
            {
                if (i != x && field[i, y].Key == 0)
                {
                    List<int> adjustedList = new List<int>(field[i, y].Value);
                    adjustedList.Remove(number);
                    KeyValuePair<int, List<int>> newPair = new KeyValuePair<int, List<int>>(field[i, y].Key, adjustedList);
                    field[i, y] = newPair;
                }
            }
            for (int j = 0; j < dims; j++)
            {
                if (j != y && field[x, j].Key == 0)
                {
                    List<int> adjustedList = new List<int>(field[x, j].Value);
                    adjustedList.Remove(number);
                    KeyValuePair<int, List<int>> newPair = new KeyValuePair<int, List<int>>(field[x, j].Key, adjustedList);
                    field[x, j] = newPair;
                }
            }

            // Get the block location and dimensions
            int blockdims = (int)Math.Sqrt(dims);
            int block_x = x / blockdims, block_y = y / blockdims;

            // Adjust the blocks
            for (int i = block_x * blockdims; i < block_x * blockdims + blockdims; i++)
            {
                for (int j = block_y * blockdims; j < block_y * blockdims + blockdims; j++)
                {
                    if (!(i == x && j == y) && field[i, j].Key == 0)
                    {
                        List<int> adjustedList = new List<int>(field[i, j].Value);
                        adjustedList.Remove(number);
                        KeyValuePair<int, List<int>> newPair = new KeyValuePair<int, List<int>>(field[i, j].Key, adjustedList);
                        field[i, j] = newPair;
                    }
                }
            }

            return field;
        }

    }

}
