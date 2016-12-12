using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Day12
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Example()
        {
            var state = new State();
            state.ApplyAll(File.ReadAllLines("testinput.txt"));
            Assert.AreEqual(42, state.Registers["a"]);
        }

        [TestMethod]
        public void Part1()
        {
            var state = new State();
            state.ApplyAll(File.ReadAllLines("input.txt"));
            Console.Out.WriteLine(state.Registers["a"]);
        }

        [TestMethod]
        public void Part2()
        {
            var state = new State();
            state.Registers["c"] = 1;
            state.ApplyAll(File.ReadAllLines("input.txt"));
            Console.Out.WriteLine(state.Registers["a"]);
        }
    }

    public class State
    {
        public Dictionary<string, int> Registers { get; } = new Dictionary<string, int>
        {
            {"a", 0},
            {"b", 0},
            {"d", 0}
        };


        public void ApplyAll(string[] instructions)
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                
                var j = ApplyInstruction(instructions[i]);
                //LogManager.GetCurrentClassLogger().Info($"{i:00}:   {string.Join(" ", Registers.Values.Select(r => r.ToString("00")))}:   {instructions[i].PadRight(8)}");
                if (j != 0) i += j - 1;
            }
        }
        public int ApplyInstruction(string instruction)
        {
            int toSkip = 0;
            var parts = instruction.Split(' ');
            if (instruction.StartsWith("cpy"))
            {
                var toCopy = GetValue(parts[1]);
                var copyTo = parts[2];
                Registers[copyTo] = toCopy;
            }
            else if (instruction.StartsWith("inc"))
            {
                var toInc = parts[1];
                Registers[toInc]++;
            }
            else if (instruction.StartsWith("dec"))
            {
                var toInc = parts[1];
                Registers[toInc]--;
            }
            else if (instruction.StartsWith("jnz"))
            {
                var comparator = GetValue(parts[1]);
                if (comparator != 0)
                {
                    toSkip = GetValue(parts[2]);
                }
            }
            else throw new ArgumentException($"Unrecognized instruction {instruction}");
            return toSkip;
        }

        private int GetValue(string expr)
        {
            int exprVal;
            if (!int.TryParse(expr, out exprVal))
            {
                exprVal = Registers[expr];
            }
            return exprVal;
        }
    }
}
