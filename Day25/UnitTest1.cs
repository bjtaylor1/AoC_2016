using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using NUnit.Framework;

namespace Day25
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void SolveQuickly()
        {
            var answer = Convert.ToInt32(Enumerable.Range(1, 10).Select(i => string.Join("", Enumerable.Repeat("10", i)))
                .First(s => Convert.ToInt32(s, 2) > 2550), 2) - 2550;
        }

        [Test]
        public void SolveEvenQuicker()
        {
            var answer = Enumerable.Range(2550, Int16.MaxValue).First(s => Regex.IsMatch(Convert.ToString(s, 2), @"^(10)+$")) - 2550;
            Console.Out.WriteLine(answer);
        }

        [Test]
        public void ReverseEngineer()
        {
            int i = 1;
            bool b = false;
            while (i <= 2550)
            {
                if (b) i *= 2;
                else i += 1;
                LogManager.GetCurrentClassLogger().Info(i);
                b = !b;
            }
        }

        //[TestCase("input.txt")]
        [TestCase("optimizedinput.txt")]
        public void TestMethod1(string file)
        {
            var instructions = File.ReadAllLines($"{TestContext.CurrentContext.TestDirectory}\\{file}").ToArray();
            int? found = null;
            for (int i = 0; i < 1000; i++)
            {
                var state = new State(instructions);
                state.Registers["a"] = i;
                var expected = 0;
                var repetition = 0;
                state.ApplyAll(i1 =>
                {
                    if (i1 != expected) return int.MaxValue;
                    expected = 1 - expected;
                    if (repetition++ == 20)
                    {
                        found = i;
                        return int.MaxValue;
                    }
                    return null;
                });
                if (found.HasValue) break;
            }
            LogManager.GetCurrentClassLogger().Info($"Found: {found}");

        }


    }

    public class State
    {
        public Dictionary<string, int> Registers { get; } = new Dictionary<string, int>
            {
                {"a", 519},
                {"b", 0},
                {"c", 0},
                {"d", 0}
            };

        private readonly string[] instructions;

        public State(string[] instructions)
        {
            this.instructions = instructions;
        }

        public void ApplyAll(Func<int, int?> output)
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                var beforeStates = string.Join(" ", Registers.Values.Select(r => r.ToString("00")));
                var j = ApplyInstruction(i, output);
                if (j == int.MaxValue) return;
                var afterStates = string.Join(" ", Registers.Values.Select(r => r.ToString("00")));
                //LogManager.GetCurrentClassLogger().Info($"{i:00}:   {beforeStates}   {instructions[i].PadRight(8)}   {afterStates}");
                if (j.HasValue) i += j.Value - 1;
            }
        }

        public int? ApplyInstruction(int i, Func<int, int?> output)
        {
            var instruction = instructions[i];

            int? toSkip = null;

            int simplificationIndex = instruction.IndexOf(':');
            if (simplificationIndex != -1) instruction = instruction.Substring(simplificationIndex + 1).Trim();

            if (!instruction.StartsWith("#"))
            {
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
                else if (instruction.StartsWith("mul"))
                {
                    Registers[parts[3]] = Registers["a"] + Registers[parts[1]]*Registers[parts[2]];
                    Registers[parts[1]] = 0;
                    Registers[parts[2]] = 0;
                }
                else if (instruction.StartsWith("div"))
                {
                    Registers[parts[3]] = Registers[parts[1]]/Registers[parts[2]];
                    var rem = Registers[parts[1]]%Registers[parts[2]];
                    Registers[parts[1]] = rem;
                    Registers[parts[2]] = 0;
                }
                else if (instruction.StartsWith("out"))
                {
                    return output(Registers[parts[1]]);
                }

                else throw new ArgumentException($"Unrecognized instruction {instruction}");
            }
            return toSkip;
        }

        private int count;

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
