using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] strs = { 1111, 2222, 3333 };
            var result = strs.Select(str => str.ToString()).Aggregate("", (res, str) => res + str);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
