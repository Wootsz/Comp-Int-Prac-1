using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Prac1
{
    public class Program
    {
        static int dims;        // Dimensions of the sudoku
        static bool found;      // If a solution has been found
        
        static void Main(string[] args)
        {
            Stack<int[,]> stack = new Stack<int[,]>();
            List<int> start_list = new List<int>{
                0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 3, 0, 8, 5,
                0, 0, 1, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 5, 0, 7, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 1, 0, 0,
                0, 9, 0, 0, 0, 0, 0, 0, 0,
                5, 0, 0, 0, 0, 0, 0, 7, 3,
                0, 0, 2, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 4, 0, 0, 0, 9 };
            dims = (int)Math.Sqrt(start_list.Count);
            int[,] start_state = new int[dims, dims];
            int c = 0;
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                {
                    start_state[i, j] = start_list[c];
                    c++;
                }

            List<Point> basePoints = new List<Point>();
            for (int j = 0; j < dims; j++)
                for (int i = 0; i < dims; i++)
                    if (start_state[i, j] != 0)
                        basePoints.Add(new Point(i, j));

            stack.Push(start_state);
            int[,] end = BackTracking(stack);


            Console.WriteLine("-----------------------");
            for (int j = 0; j < dims; j++)
            {
                for (int i = 0; i < dims; i++)
                {
                    Console.Write(end[i, j] + "|");
                }
                Console.WriteLine("");
                Console.WriteLine("-----------------------");
            }
            Console.ReadLine();
        }

        static int[,] BackTracking(Stack<int[,]> stack)
        {
            if(stack.Count == 0) { return null; }
            else
            {
                int[,] state = stack.Pop();
                // Check if the current state is a goal state
                if (Goal(state))
                {
                    found = true;
                    return state;
                }
                // If not, do backtracking with sucessor states
                else
                {
                    Point cell = FirstEmptyCell(state);
                    Console.WriteLine(cell);
                    int n = 1;
                    while (n <= dims && !found)
                    {
                        bool valid = false;
                        int[,] next_state = state;
                        // Find a valid move
                        while (!valid && n <= dims)
                        {
                            next_state = state;
                            next_state[cell.X, cell.Y] = n;
                            n++;
                            valid = ValidState(next_state, cell.X, cell.Y);
                            
                                
                            
                        }
                        Console.WriteLine("Found valid state.");
                        // Go into recursion by adding next_state to the stack
                        stack.Push(next_state);
                        return BackTracking(stack);

                    }
                    stack.Pop();
                }
            }
            ///!!!!!!!!!!!!!!!!!!!!!
            return null;
        }

        static Point FirstEmptyCell(int[,] field)
        {
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                    if (field[i, j] == 0)
                        return new Point(i, j);
            return new Point(0,0);
        }

        // Check if a state is the goal state
        static bool Goal(int[,] field)
        {
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                    if (field[i,j] == 0 || !ValidState(field, i, j))
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

//            Console.WriteLine("true");
            return true;
        }

    }
    
}
