using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using TestContext = NUnit.Framework.TestContext;

namespace Day24
{
    [TestFixture]
    public class UnitTest1
    {
        private static Stopwatch stopwatch;

        [TestCase(0,1, 2)]
        [TestCase(0,2, 8)]
        [TestCase(0,4, 2)]
        [TestCase(3,4, 8)]
        [TestCase(2,1, 6)]
        public void Examples(int from, int to, int steps)
        {
            var lines = File.ReadAllLines($"{TestContext.CurrentContext.TestDirectory}\\testinput.txt");
            var routeFinder = new RouteFinder(lines);
            var minDist = routeFinder.GetDistance(routeFinder.Targets[from], routeFinder.Targets[to]);
            Assert.AreEqual(steps, minDist);
        }

        [Test]
        public void OutputCombinations()
        {
            var combinations = new List<Tuple<int, int>>();
            var distances = new Dictionary<Tuple<int,int>, int>();
            var lines = File.ReadAllLines($"{TestContext.CurrentContext.TestDirectory}\\input.txt");
            var routeFinder = new RouteFinder(lines);

            LogManager.GetCurrentClassLogger().Info("==============Start");
            for (int y = 0; y < 8; y++)
            {
                for (int x = y + 1; x < 8; x++)
                {
                    var pair = new Tuple<int, int>(y, x);
                    var dist = routeFinder.GetDistance(routeFinder.Targets[pair.Item1], routeFinder.Targets[pair.Item2]);
                    LogManager.GetCurrentClassLogger().Info($"{y} -> {x} = {dist}");
                    distances.Add(Tuple.Create(y, x), dist);
                    distances.Add(Tuple.Create(x, y), dist); //symmetrical?!
                }
            }

            int bestTourDist = int.MaxValue;
            int[] bestTour = null;
            GetAllPossibleTours(ints =>
            {
                if (ints[0] == 0)
                {
                    int tourDist = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        tourDist += distances[Tuple.Create(ints[i - 1], ints[i])];
                    }
                    tourDist += distances[Tuple.Create(ints.Last(), 0)];
                    if (tourDist < bestTourDist)
                    {
                        bestTourDist = tourDist;
                        bestTour = ints.ToArray();
                    }
                }
            });
            LogManager.GetCurrentClassLogger().Info($"Best tour = {string.Join("-", bestTour.Select(i => i.ToString()))}, distance = {bestTourDist}");

            Console.Out.WriteLine(combinations.Count);

        }

        [Test]
        public void CountCombinations()
        {
            int combinations = 0;
            GetAllPossibleTours(ints =>
            {
                //if (ints[0] == 0)
                {
                    combinations++;
                }
            });

            Console.Out.WriteLine(combinations);

        }

        private void GetAllPossibleTours(Action<int[]> process)
        {
            GeneratePossibleTours(new int[] { 0 }, process);
        }

        private void GeneratePossibleTours(int[] cumulative, Action<int[]> process)
        {
            bool b = cumulative.Take(6).SequenceEqual(new[] {0, 7,6,1,5,2});

            if (cumulative.Length == 8)
            {
                process(cumulative);
            }
            else
            {
                for (int next = 0; next < 8; next++)
                {
                    if (!cumulative.Contains(next))
                    {
                        GeneratePossibleTours(cumulative.Concat(new [] {next}).ToArray(), process);
                    }
                }
            }
        }


        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            stopwatch = new Stopwatch();
        }

        [TestCase(0,1)]
        [TestCase(0,2)]
        [TestCase(0,3)]
        [TestCase(0,4)]
        [TestCase(0,5)]
        [TestCase(0,6)]
        [TestCase(0,7)]
        public void TestGetDist(int from, int to)
        {
            stopwatch.Start();
            var lines = File.ReadAllLines($"{TestContext.CurrentContext.TestDirectory}\\input.txt");
            var routeFinder = new RouteFinder(lines);
            var minDist = routeFinder.GetDistance(routeFinder.Targets[from], routeFinder.Targets[to]);
            Console.Out.WriteLine(minDist);
            stopwatch.Stop();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Console.Out.WriteLine(stopwatch.Elapsed);
        }

    }



    public class RouteFinder
    {
        //y most significant (first index)
        public bool[][] Open { get; }
        public Dictionary<int,Pos> Targets { get; } = new Dictionary<int, Pos>();

        public int GetDistance(Pos start, Pos end)
        {
            var positions = new List<Movement> {new Movement(start,0)};
            Movement current;
            while ((current = positions.First()).Pos != end)
            {
                if(current.Visited) throw new InvalidOperationException("Best already visited. Check sort.");

                var newPositions = GetNewPositions(current.Pos)
                    .Where(p => !positions.Any(m => m.Pos == p)) //discard already visited
                    .Select(p => new Movement(p, current.Steps + 1))
                    .ToArray();
                current.Visited = true;
                positions.AddRange(newPositions);
                positions.Sort((m1, m2) =>
                {
                    var comparisons = new Func<Movement, Movement, int>[]
                    {
                        (mm1, mm2) => mm1.Visited.CompareTo(mm2.Visited),
                        (mm1, mm2) => mm1.Steps.CompareTo(mm2.Steps),
                        (mm1, mm2) => mm1.Pos.DistanceFrom(end).CompareTo(mm2.Pos.DistanceFrom(end))
                    };
                    var firstNonZero = SortHelper.FirstNonZero(comparisons, m1, m2);
                    return firstNonZero;
                });
            }
            var distance = current.Steps;
            return distance;
        }

        public Pos[] GetNewPositions(Pos pos)
        {
            var newPos = new List<Pos>();
            if(pos.Y > 0 && Open[pos.Y-1][pos.X]) //up
                newPos.Add(new Pos(pos.Y-1, pos.X));

            if(pos.Y < Open.Length-1 && Open[pos.Y+1][pos.X]) //down
                newPos.Add(new Pos(pos.Y+1, pos.X));

            if(pos.X > 0 && Open[pos.Y][pos.X-1]) //left
                newPos.Add(new Pos(pos.Y, pos.X-1));

            if(pos.X < Open[pos.Y].Length -1 && Open[pos.Y][pos.X + 1]) //right
                newPos.Add(new Pos(pos.Y, pos.X + 1));

            return newPos.ToArray();
        }

        public RouteFinder(string[] lines)
        {
            if (lines.Select(s => s.Length).Distinct().Count() != 1)
            {
                throw new ArgumentException($"Lines not all same length!");
            }

            Open = new bool[lines.Length][];
            for (int y = 0; y< lines.Length; y++)
            {
                var line = lines[y];
                Open[y] = new bool[line.Length];
                for (int x = 0; x < line.Length; x++)
                {
                    int num;
                    char c = line[x];
                    if (int.TryParse(c.ToString(), out num))
                    {
                        Targets.Add(num, new Pos(y, x));
                        Open[y][x] = true;
                    }
                    else if (c == '#')
                    {
                        Open[y][x] = false;
                    }
                    else if (c == '.')
                    {
                        Open[y][x] = true;
                    }
                    else throw new ArgumentException($"Unrecognized {c} at {y},{x}", nameof(lines));
                }
            }
        }
    }

    public class Movement
    {
        public Movement(Pos pos, int steps)
        {
            Pos = pos;
            Steps = steps;
        }

        public Pos Pos { get; }
        public int Steps { get; }
        public bool Visited { get; set; }

        public override string ToString()
        {
            return $"Pos: {Pos}, Steps: {Steps}";
        }
    }

    public class Pos : IEquatable<Pos>
    {
        public Pos(int y, int x)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"Y: {Y}, X: {X}";
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(Pos other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pos) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public static bool operator ==(Pos left, Pos right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Pos left, Pos right)
        {
            return !Equals(left, right);
        }

        public int DistanceFrom(Pos other)
        {
            return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
        }
    }

    public class SortHelper
    {
        public static int FirstNonZero<T>(Func<T, T, int>[] comparisons, T t1, T t2)
        {
            foreach (var func in comparisons)
            {
                var res = func(t1, t2);
                if (res != 0) return res;
            }
            return 0;
        }
    }

}
