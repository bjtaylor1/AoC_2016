using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                var triangleSpecs = File.ReadAllLines("input.txt")
                    .Select(line => Regex.Split(line, "\\s+").Where(s => !string.IsNullOrEmpty(s.Trim())))
                    .Select(s => s.Select(int.Parse).ToArray())
                    .ToArray();
                if(triangleSpecs.Any(i => i.Length != 3)) throw new ArgumentException();

                var numPossible = triangleSpecs.Count(IsPossible);
                Console.Out.WriteLine(numPossible);
            }

            {
                var triangleSpecs = File.ReadAllLines("input.txt")
                    .Select(line => Regex.Split(line, "\\s+").Where(s => !string.IsNullOrEmpty(s.Trim())))
                    .Select(s => s.Select(int.Parse).ToArray())
                    .ToArray();
                if(triangleSpecs.Any(i => i.Length != 3)) throw new ArgumentException();

                int numPossible = 0;
                for (int i = 0; i < triangleSpecs.Length - 2; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var triangleSpec = new[] {triangleSpecs[i][j], triangleSpecs[i + 1][j], triangleSpecs[i + 2][j]};
                        if (IsPossible(triangleSpec)) numPossible ++;
                    }
                }
                Console.Out.WriteLine(numPossible);
            }
        }

        static bool IsPossible(int[] i)
        {
            Array.Sort(i);
            var isPossible = i[0] + i[1] > i[2];
            return isPossible;
        }


    }
}
