using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace Day11
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestGetMovableCombinations()
        {
            var state = new State {Floors = new[] {new List<string> {"AB", "AC", "AD"},}};
            state.GetMovableCombinations().Select(s => string.Join(",", s.OrderBy(i => i))).Should().HaveCount(6).And.Contain(new[]
            {
                "AB",
                "AC",
                "AD",
                "AB,AC",
                "AB,AD",
                "AC,AD"
                });
        }

        [TestCase(false, "THG", "LIM")]
        [TestCase(false, "THG", "LIM", "PRG", "PRM")]
        [TestCase(true, "THG", "LIG", "LIM")]
        [TestCase(true, "THG", "THM", "LIG", "LIM")]
        [TestCase(false, "THG", "THG", "THM", "LIG", "LIM", "LIM")]
        [TestCase(true, "THG", "LIG", "PRG")]
        [TestCase(true, "THM", "LIM", "PRM")]
        public void IsValidTest(bool isValid, params string[] items)
        {
            var state = new State {Floors = new[] {new List<string>(items)}};
            state.IsValid().Should().Be(isValid);
        }


        
    }

    public class State
    {
        public List<string>[] Floors = new List<string>[3];
        public int Pos = 0;
        private State state;

        public State()
        {
        }

        public State(State state, int movement)
        {
            Floors = state.Floors.ToArray();
            Pos = state.Pos + movement;
        }

        public bool IsValid()
        {
            var isValid = Floors.All(ContentsCompatible);
            return isValid;

        }

        private static bool ContentsCompatible(List<string> items)
        {
            //remove pairs
            var microchips = items.Where(s => s.EndsWith("M")).ToList();
            var generators = items.Where(s => s.EndsWith("G")).ToList();
            for (int index = microchips.Count - 1; index >= 0; index--)
            {
                var microchip = microchips[index];
                var firstMatchingGenerator = generators.FindIndex(g => g.StartsWith(microchip.Substring(0, 2)));
                if (firstMatchingGenerator != -1)
                {
                    generators.RemoveAt(firstMatchingGenerator);
                    microchips.RemoveAt(index);
                }
            }
            var valid = !(microchips.Any() && generators.Any());
            return valid;
        }

        public string[][] GetMovableCombinations()
        {
            var movableCombinations = new List<string[]>();
            for (int i = 0; i < Floors[Pos].Count; i++)
            {
                movableCombinations.Add(new []{Floors[Pos][i]});
                for (int j = i + 1; j < Floors[Pos].Count; j++)
                {
                    movableCombinations.Add(new [] {Floors[Pos][i], Floors[Pos][j]});
                }
            }
            return movableCombinations.ToArray();
        }

        public State ApplyMove(string[] items, int floorChange)
        {
            var newState = new State(this, floorChange);
            newState.Floors[Pos].RemoveAll(items.Contains);
            newState.Floors[Pos + floorChange].AddRange(items);
            return newState;
        }

        public State[] GetTransitions()
        {
            var states = new List<State>();
            var movableCombinations = GetMovableCombinations();
            if (Pos > 0)
            {
                states.AddRange(movableCombinations.Select(s => ApplyMove(s, Pos - 1)));
            }
            if (Pos < 3)
            {
                states.AddRange(movableCombinations.Select(s => ApplyMove(s, Pos + 1)));
            }
            return states.ToArray();
        }
    }
}
