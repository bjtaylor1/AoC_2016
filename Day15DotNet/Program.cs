using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day15DotNet
{
    class Program
    {
        static int Main()
        {
            Console.Out.WriteLine(Enumerable.Repeat(0, 0xffffff).Select((d, i) => i).ToArray().First(t => new[]
            {
                new[] {0, 0},
                new[] {0, 7},
                new[] {0, 13},
                new[] {2, 3},
                new[] {2, 5},
                new[] {0, 17},
                new[] {7, 19}
                /* part 2: */, new [] {0,11}
            }.Select((p, i) => new {p, i}).Skip(1).All(a => (a.p[0] + a.i + t)%a.p[1] == 0)));
            return 0;
        }
    }
}
