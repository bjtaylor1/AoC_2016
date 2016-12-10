using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day10
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Example()
        {
            var exampleLines = File.ReadAllLines("ExampleInput.txt");
            foreach (var exampleLine in exampleLines)
            {
                Processor.ProcessLine(exampleLine);
            }
            Processor.OutputBins[0].Should().Contain(5);
            Processor.OutputBins[1].Should().Contain(2);
            Processor.OutputBins[2].Should().Contain(3);

            Processor.Bots.Single(b => b.Value.High == 5 && b.Value.Low == 2).Key.Should().Be(2);
        }

        [TestMethod]
        public void ProcessInput()
        {
            var lines = File.ReadAllLines("input.txt");
            foreach (var line in lines)
            {
                Processor.ProcessLine(line);
            }

            var bot = Processor.Bots.Single(b => b.Value.High == 61 && b.Value.Low == 17);
            Console.Out.WriteLine(bot.Key);
        }
    }

    public class Processor
    {
        public static readonly ConcurrentDictionary<int, Bot> Bots = new ConcurrentDictionary<int, Bot>();
        public static readonly ConcurrentDictionary<int, List<int>> OutputBins = new ConcurrentDictionary<int, List<int>>();

        public static Bot GetBot(int index)
        {
            var bot = Bots.GetOrAdd(index, i => new Bot(i));
            return bot;
        }

        public static void AddToOutputBin(int index, int chip)
        {
            OutputBins.GetOrAdd(index, i => new List<int>()).Add(chip);
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
            if (chips.Count == 2 && instruction != null) ProcessInstruction();
        }

        private void ProcessInstruction()
        {
            High = chips.Max();
            Low = chips.Min();
            chips.Clear();
            instruction.GiveHighTo.Apply(High);
            instruction.GiveLowTo.Apply(Low);
        }

        public int Low { get; set; }

        public int High { get; set; }

        public void SetInstruction(Instruction newInstruction)
        {
            if(instruction != null) throw new InvalidOperationException($"Bot {index} has already got an instruction!");
            instruction = newInstruction;
            if(chips.Count == 2 && instruction != null) ProcessInstruction();
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
            Index = int.Parse(match.Groups[2].Value);
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
                    Processor.AddToOutputBin(Index, chip);
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
