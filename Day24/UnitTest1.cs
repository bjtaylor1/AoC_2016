using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day24
{
    [TestFixture]
    public class UnitTest1
    {
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
