using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day10
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    public class Processor
    {
        private static readonly ConcurrentDictionary<int, Bot> bots = new ConcurrentDictionary<int, Bot>();
        private static readonly ConcurrentDictionary<int, List<int>> outputBins = new ConcurrentDictionary<int, List<int>>();

        public static Bot GetBot(int index)
        {
            var bot = bots.GetOrAdd(index, i => new Bot(i));
            return bot;
        }

        public static void AddToOutputBin(int index, int chip)
        {
            outputBins.GetOrAdd(index, i => new List<int>()).Add(chip);
        }

        private static readonly Regex valueGoesTo = new Regex(@"^value (\d+) goes to bot (\d+)$");
        private static readonly Regex instructionRegex = new Regex(@"^bot (\d+) gives (.+)$");
        public static void ProcessLine(string line)
        {
            Match m;
            if ((m = valueGoesTo.Match(line)).Success)
            {
                var value = int.Parse(m.Groups[1].Value);
                var bot = int.Parse(m.Groups[2].Value);
                GetBot(bot).GiveChip(value);
            }
            else if ((m = instructionRegex.Match(line)).Success)
            {
                var bot = int.Parse(m.Groups[1].Value);
                var instString = m.Groups[2].Value.Trim();
                var instruction = new Instruction(instString);
                GetBot(bot).SetInstruction(instruction);
            }
            else throw new ArgumentException($"Unrecognized instruction \"{line}\"");
        }


    }

    public class Bot
    {
        private readonly int index;
        private readonly List<int> chips = new List<int>();
        private Instruction instruction;

        public Bot(int index)
        {
            this.index = index;
        }

        public void GiveChip(int val)
        {
            if(chips.Count >= 2) throw new InvalidOperationException($"Bot {index} has already got two chips!");
            chips.Add(val);
            if (chips.Count == 2) ProcessInstruction();
        }

        private void ProcessInstruction()
        {
            if(instruction == null) throw new InvalidOperationException($"Bot {index} has got two chips but no instruction yet!");
            var high = chips.Max();
            var low = chips.Min();
            chips.Clear();
            instruction.GiveHighTo.Apply(high);
            instruction.GiveLowTo.Apply(low);
        }

        public void SetInstruction(Instruction newInstruction)
        {
            if(instruction != null) throw new InvalidOperationException($"Bot {index} has already got an instruction!");
            instruction = newInstruction;
        }
    }

    public class Instruction
    {
        public Instruction(string instruction)
        {
            var match = Regex.Match(instruction, "^low to (.+) and high to (.+)$");
            if(!match.Success) throw new ArgumentException($"Invalid instruction {instruction}");
            GiveLowTo = new Target(match.Groups[1].Value);
            GiveHighTo= new Target(match.Groups[2].Value);
        }

        public Target GiveLowTo { get; }
        public Target GiveHighTo { get; }
    }

    public class Target
    {
        public Target(string s)
        {
            var match = Regex.Match(s, @"(bot|output) (\d+)");
            if(!match.Success) throw new ArgumentException($"Invalid target {s}");
            TargetType = (TargetType) Enum.Parse(typeof(TargetType), match.Groups[1].Value);
            Index = int.Parse(match.Groups[1].Value);
        }
        public int Index { get; }
        public TargetType TargetType { get; }

        public void Apply(int chip)
        {
            switch (TargetType)
            {
                case TargetType.bot:
                    Processor.GetBot(Index).GiveChip(chip);
                    break;
                case TargetType.output:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum TargetType
    {
        bot,
        output
    }
}
