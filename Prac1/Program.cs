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

            // The sudoku to solve, in a string format (0 = empty cell)
            string s1 =
            "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0";
            string s2 = "2 15 0 0 21 4 0 0 25 9 0 0 0 0 0 18 24 0 0 20 3 0 0 8 5 25 20 0 0 14 17 0 0 24 6 0 0 0 0 0 5 8 0 0 1 9 0 0 18 4 0 0 7 23 0 0 3 8 0 0 24 10 0 18 4 0 0 19 21 0 0 2 15 0 0 0 0 24 17 0 0 18 16 0 0 13 19 0 23 2 0 0 7 22 0 0 1 12 0 0 3 4 0 0 0 0 0 0 0 0 0 0 16 0 0 0 0 0 0 0 0 0 0 17 10 15 23 0 0 0 16 6 0 0 0 0 0 20 0 0 0 0 0 4 18 0 0 0 11 14 0 0 21 22 0 10 12 0 0 0 0 0 0 0 0 0 0 0 13 11 0 3 9 0 0 0 0 5 1 0 0 0 4 15 0 0 17 18 3 0 0 25 2 0 0 0 21 24 0 0 8 2 0 0 0 0 0 1 22 0 0 13 6 4 0 0 7 23 0 0 0 0 0 16 25 20 3 0 0 0 0 0 0 0 24 8 0 0 0 10 12 0 0 0 0 0 0 0 13 1 0 0 9 20 0 0 0 0 0 10 1 0 0 0 6 24 0 0 0 0 0 22 14 0 0 0 0 1 21 0 0 0 11 13 0 0 0 0 0 0 0 9 16 0 0 0 4 18 0 0 0 0 0 0 25 3 0 2 18 0 0 0 0 0 0 0 17 10 0 22 23 0 0 0 0 0 0 22 10 0 0 0 14 17 0 0 0 0 0 0 0 21 13 0 0 0 24 6 0 0 0 0 19 18 0 0 0 0 0 15 12 0 0 0 9 11 0 0 0 0 0 10 3 0 0 17 1 0 0 0 0 0 0 0 25 23 0 0 0 24 8 0 0 0 0 0 0 0 10 9 10 18 0 0 0 0 0 15 7 0 0 3 25 16 0 0 23 21 0 0 0 0 0 24 2 0 0 11 9 0 0 0 13 4 0 0 18 2 6 0 0 22 15 0 0 0 5 1 0 0 0 0 2 16 0 21 23 0 0 0 0 0 0 0 0 0 0 0 11 4 0 18 25 0 0 19 25 0 0 0 6 22 0 0 0 0 0 11 0 0 0 0 0 9 10 0 0 0 7 16 21 17 0 0 0 0 0 0 0 0 0 0 3 0 0 0 0 0 0 0 0 0 0 9 11 0 0 18 12 0 0 21 23 0 0 19 6 0 8 7 0 0 24 10 0 0 16 5 0 0 0 0 16 4 0 0 9 7 0 0 21 23 0 10 22 0 0 12 20 0 0 25 2 0 0 24 13 0 0 11 18 0 0 20 8 0 0 0 0 0 21 4 0 0 9 1 0 0 3 23 5 22 0 0 23 2 0 0 1 12 0 0 0 0 0 25 15 0 0 8 7 0 0 6 18";
            string s3 = "0 15 0 1 0 2 10 14 12 0 0 0 0 0 0 0 0 6 3 16 12 0 8 4 14 15 1 0 2 0 0 0 14 0 9 7 11 3 15 0 0 0 0 0 0 0 0 0 4 13 2 12 0 0 0 0 6 0 0 0 0 15 0 0 0 0 0 0 14 1 11 7 3 5 10 0 0 8 0 12 3 16 0 0 2 4 0 0 0 14 7 13 0 0 5 15 11 0 5 0 0 0 0 0 0 9 4 0 0 6 0 0 0 0 0 0 13 0 16 5 15 0 0 12 0 0 0 0 0 0 0 0 9 0 1 12 0 8 3 10 11 0 15 0 2 12 0 11 0 0 14 3 5 4 0 0 0 0 9 0 6 3 0 4 0 0 13 0 0 11 9 1 0 12 16 2 0 0 10 9 0 0 0 0 0 0 12 0 8 0 6 7 12 8 0 0 16 0 0 10 0 13 0 0 0 5 0 0 5 0 0 0 3 0 4 6 0 1 15 0 0 0 0 0 0 9 1 6 0 14 0 11 0 0 2 0 0 0 10 8 0 14 0 0 0 13 9 0 4 12 11 8 0 0 2 0";
            string[] start = s3.Split();

            dims = (int)Math.Sqrt(start.Length);

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
                for (int i = 0; i < dims; i++)
                    for (int j = 0; j < dims; j++)
                        if (start_state[i, j].Key != 0)
                            AdjustDomains(start_state, i, j);

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

                    for (int n = 1; n <= dims; n++)
                    {
                        next_state = prev_state;
                        next_state[cell.X, cell.Y] = new KeyValuePair<int, List<int>>(n, next_state[cell.X, cell.Y].Value);
                        if (ValidState(ToIntArray(next_state), cell.X, cell.Y))
                        {
                            // Adjust the domains of all cells and check if none of them are empty
                            KeyValuePair<int, List<int>>[,] next_state_clone = (KeyValuePair<int, List<int>>[,])next_state.Clone();
                            KeyValuePair<int, List<int>>[,] adjusted_next_state = AdjustDimensions(next_state_clone, n, cell);
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
            }
            return null;
        }

        static KeyValuePair<int, List<int>>[,] AdjustDimensions(KeyValuePair<int, List<int>>[,] state, int addition, Point location)
        {
            int y = location.Y;
            int x = location.X;

            // Adjust the row
            for (int i = 0; i < dims; i++)
                if (i != x)
                    if (state[i, y].Key == 0)
                    {
                        int[] newList = new int[dims];
                        state[i, y].Value.CopyTo(newList);
                        List<int> new_list = newList.ToList<int>();
                        new_list.Remove(addition);
                        state[i, y] = new KeyValuePair<int, List<int>>(0, new_list);
                    }
            // Adjust the column
            for (int j = 0; j < dims; j++)
                if (j != y)
                    if (state[x, j].Key == 0)
                    {
                        int[] newList = new int[dims];
                        state[x,j].Value.CopyTo(newList);
                        List<int> new_list = newList.ToList<int>();
                        new_list.Remove(addition);
                        state[x,j] = new KeyValuePair<int, List<int>>(0, new_list);
                    }

            // Get the block location and dimensions
            int blockdims = (int)Math.Sqrt(dims);
            int block_x = location.X / blockdims, block_y = location.Y / blockdims;

            // Adjust the blocks
            for (int i = block_x * blockdims; i < block_x * blockdims + blockdims; i++)
                for (int j = block_y * blockdims; j < block_y * blockdims + blockdims; j++)
                    if (!(i == x && j == y))
                        if (state[i, j].Key == 0)
                        {
                            int[] newList = new int[dims];
                            state[i, j].Value.CopyTo(newList);
                            List<int> new_list = newList.ToList<int>();
                            new_list.Remove(addition);
                            state[i, j] = new KeyValuePair<int, List<int>>(0, new_list);
                        }

            return state;
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
                    if (state[i, j].Key == 0 && (state[i, j].Value == null || state[i, j].Value.Count == 0))
                        return true;
            return false;
        }

        // Returns the location of the cell with the lowest domain size 
        static Point MostConstrainedVariable(KeyValuePair<int, List<int>>[,] state)
        {
            int smallestConstraint = dims + 100;
            Point result = new Point(0, 0);
            for (int x = 0; x < dims; x++)
                for (int y = 0; y < dims; y++)
                    if (state[x, y].Key == 0)
                    {
                        int stateConstraint = state[x, y].Value.Count;
                        if (stateConstraint < smallestConstraint)
                        {
                            smallestConstraint = stateConstraint;
                            result = new Point(x, y);
                        }
                    }

            return result;
        }

        static void AdjustDomains(KeyValuePair<int, List<int>>[,] field, int x, int y)
        {
            int number = field[x, y].Key;

            // Adjust the rows and colums
            for (int i = 0; i < dims; i++)
            {
                if (i != x && field[i, y].Key == 0 && field[i, y].Value.Contains(number))
                {
                    int[] newList = new int[dims];
                    field[i, y].Value.CopyTo(newList);
                    List<int> new_list = newList.ToList<int>();
                    new_list.Remove(number);
                    field[i, y] = new KeyValuePair<int, List<int>>(0, new_list);
                }
            }
            for (int j = 0; j < dims; j++)
            {
                if (j != y && field[x, j].Key == 0 && field[x, j].Value.Contains(number))
                {
                    int[] newList = new int[dims];
                    field[x, j].Value.CopyTo(newList);
                    List<int> new_list = newList.ToList<int>();
                    new_list.Remove(number);
                    field[x, j] = new KeyValuePair<int, List<int>>(0, new_list);
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
                    if ((i != x && j != y) && field[i, j].Key == 0 && field[i, j].Value.Contains(number))
                    {
                        int[] newList = new int[dims];
                        field[i, j].Value.CopyTo(newList);
                        List<int> new_list = newList.ToList<int>();
                        new_list.Remove(number);
                        field[i, j] = new KeyValuePair<int, List<int>>(0, new_list);
                    }
                }
            }
        }
    }

}
