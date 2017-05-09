using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prac1
{
    public class Program
    {
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
            int dims = (int)Math.Sqrt(start_list.Count);
            int[,] start_state = new int[dims, dims];
            int c = 0;
            for (int i = 0; i < dims; i++)
                for (int j = 0; j < dims; j++)
                {
                    start_state[i, j] = start_list[c];
                    c++;
                }
            stack.Push(start_state);
            BackTracking(stack);
        }

        static void BackTracking(Stack<int[,]> stack)
        {
            if(stack.Count == 0) { return; }
            else
            {

            }
        }
    }
    
}
