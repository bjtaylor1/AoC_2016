using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Day23
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Example()
        {
            var exampleLines = File.ReadAllLines("testinput.txt");
            var state = new State(exampleLines);
            state.ApplyAll();
            Assert.AreEqual(3, state.Registers["a"]);
        }

        [TestMethod]
        public void Part1()
        {
            var lines = File.ReadAllLines("input.txt");
            var state = new State(lines);
            state.Registers["a"] = 7;
            state.ApplyAll();
            Console.Out.WriteLine(state.Registers["a"]);
        }

        [TestMethod]
        public void Part2Induction()
        {
            for (int i = 6; i < 10; i++)
            {
                var lines = File.ReadAllLines("input.txt");
                var state = new State(lines);
                state.Registers["a"] = i;
                state.ApplyAll();
                LogManager.GetCurrentClassLogger().Info($"{i} = {state.Registers["a"]}");
            }
        }

        [TestMethod]
        public void Part2()
        {
            var exampleLines = File.ReadAllLines("input.txt");
            var state = new State(exampleLines);
            state.Registers["a"] = 12;
            state.ApplyAll();
            Console.Out.WriteLine(state.Registers["a"]);
        }
    }

    public class State
    {
        public Dictionary<string, int> Registers { get; } = new Dictionary<string, int>
        {
            {"a", 0},
            {"b", 0},
            {"c", 0},
            {"d", 0}
        };

        private readonly string[] instructions;

        public State(string[] instructions)
        {
            this.instructions = instructions;
        }

        public void ApplyAll()
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                var j = ApplyInstruction(i);
                //LogManager.GetCurrentClassLogger().Info($"{i:00}:   {string.Join(" ", Registers.Values.Select(r => r.ToString("00")))}:   {instructions[i].PadRight(8)}");
                if (j.HasValue) i += j.Value - 1;
            }
        }

        private bool printAll5 = false;
        public int? ApplyInstruction(int i)
        {
            var instruction = instructions[i];
/*            if (i == 5 && printAll5)
            {
                LogManager.GetCurrentClassLogger().Log(i == 5 ? LogLevel.Warn : LogLevel.Info, $"Running {i} = {instruction}    {string.Join(" / ", Registers.Values)}");
            }


            if (i == 5 && Registers["a"] == 0 && Registers["c"] > 1)
            {
                Registers["d"] = Registers["c"]*Registers["d"];
                Registers["c"] = Registers["c"] - 1;
                printAll5 |= Registers["d"] >= 479001600;
                LogManager.GetCurrentClassLogger().Log(i == 5 ? LogLevel.Warn : LogLevel.Info, $"Running {i} = {instruction}    {string.Join(" / ", Registers.Values)}");
                return 0;
            }

            if (i == 5 && Registers["c"] > 1)
            {
                Registers["a"] += Registers["c"]-1;
                Registers["c"] = 1;
                return 0;
            }*/

            int? toSkip = null;
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
            else if (instruction.StartsWith("tgl"))
            {
                var val = GetValue(parts[1]);
                Toggle(val + i);

            }
            else throw new ArgumentException($"Unrecognized instruction {instruction}");
            return toSkip;
        }

        private void Toggle(int i)
        {
            if (i >= instructions.Length || i < 0) return;

            var instruction = instructions[i];
            var parts = instruction.Split(' ');
            if (parts.Length == 2) //one argument
            {
                if (parts[0] == "inc") parts[0] = "dec";
                else parts[0] = "inc";
            }
            else if (parts.Length == 3)
            {
                if (parts[0] == "jnz") parts[0] = "cpy";
                else parts[0] = "jnz";
            }
            instructions[i] = string.Join(" ", parts);
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
