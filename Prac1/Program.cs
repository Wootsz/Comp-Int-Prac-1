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
        static int dims;                        // Dimensions of the sudoku
        static int expansion_type;              // Type of expansion for the state (1-3)
        static Stack<Point> sorted_domain_sizes;

        //Paths
        static string inputPath = Directory.GetCurrentDirectory() + "\\input";
        static string outputPath = Directory.GetCurrentDirectory() + "\\output" + "\\" + DateTime.Now.ToString("h/mm/ss");

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

            try
            {
                reader = new StreamReader(inputPath + "\\testFile.txt");
            }
            catch
            {
                Console.WriteLine("File not found \nPress key to close. . .");
                Console.Read();
                Environment.Exit(0);

            }
            string input = "";

            while (!reader.EndOfStream)
            {
                //Read line in file
                input += reader.ReadLine();
            }

                // The sudoku to solve, in a string (0 = empty cell)
                string s1 =
                "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            string s2_1, s2_2, s2_3;
            string s3_1, s3_2, s3_3, s3_4, s3_5;
            string s_test_1 =
                "1 2 3 4 5 6 7 8 9 " +
                "4 5 6 7 8 9 1 2 3 " +
                "7 8 9 1 2 3 4 5 6 " +
                "2 3 4 5 6 7 8 9 1 " +
                "5 6 7 8 9 1 2 3 4 " +
                "8 9 1 2 3 4 5 6 7 " +
                "3 4 5 6 7 8 9 1 2 " +
                "6 7 8 9 1 2 3 4 5 " +
                "9 1 2 3 4 5 6 7 8";
            string empty =
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0 " +
                "0 0 0 0 0 0 0 0 0";
            //string[] start = s1.Split();
            string[] start = input.Split();

            // Fill a 2D array with the values from the string[] above
            dims = (int)Math.Sqrt(start.Length);
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

            timer = new Stopwatch();
            timer.Start();
            // Make a stack and put the start_state on top
            Stack<int[,]> stack = new Stack<int[,]>();
            stack.Push(start_state);

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
            else
                Console.WriteLine("Could not find a solution");

            //Stats
            Console.WriteLine("\n\nStatistics:");
            Console.WriteLine("Amount of recursion calls: " + calls);
            timer.Stop();
            Console.WriteLine("Time elapsed: " + timer.Elapsed);
            Console.WriteLine("Method used: " + expansion_type);
     
            //Print results to file
            string outputString = "Solution:\n\n" + solution + "\nStatistics:\nAmount of recursion calls: " + calls + "\nTime elapsed: " + timer.Elapsed + "\nMethod used: " + expansion_type;
            File.WriteAllText(outputPath + "\\testOutputResults.txt", outputString);

            Console.ReadLine();
        }

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

                // DEBUG
                WriteState(state);

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

                    //Console.WriteLine(cell);
                    int[,] next_state;
                    int[,] prev_state = (int[,])state.Clone();

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
                    if(expansion_type == 3)
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

    }

}
