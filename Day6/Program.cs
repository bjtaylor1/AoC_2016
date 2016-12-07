using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {

            var word = new string(File.ReadAllLines("input.txt")
                .SelectMany(line => line.Select((Char, Pos) => new {Char, Pos}))
                .GroupBy(a => a.Pos)
                .Select(g => g.Select(a => a.Char).ToArray()) /* all chars as columns */
                .Select(col => col.GroupBy(c => c).OrderBy(g => g.Count()).First().Key)
                .ToArray());

            Console.Out.WriteLine(word);
        }
    }


}
