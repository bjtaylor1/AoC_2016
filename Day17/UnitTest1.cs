using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Day17
{
    [TestFixture]
    public class UnitTest1
    {
        [TestCase("ihgpwlah", "DDRRRD")]
        [TestCase("kglvqrro", "DDUDRLRRUDRD")]
        [TestCase("ulqzkmiv", "DRURDRUDDLLDLUURRDULRLDUUDDDRR")]
        public void Example(string passcode, string path)
        {
            var iteration = new Iterator(passcode).GetPathToTarget();
            Assert.AreEqual(path, iteration.Path);
        }
    }

    public class Iterator
    {
        public string Passcode { get; }

        public Iterator(string passcode)
        {
            Passcode = passcode;
        }

        public List<Iteration> Targets { get; } = new List<Iteration>();

        public Iteration GetPathToTarget()
        {
            var iterations = new List<Iteration> {new Iteration(new Pos(0,0), "", this)};
            while (iterations.Any(i => !i.Visited)) //!(targets = iterations.Where(i => i.Pos.X == 3 && i.Pos.Y == 3).ToArray()).Any())
            {
                var positions = iterations.Select(i => i.Pos).Distinct().ToArray();
                iterations.Sort((i1, i2) =>
                {
                    {
                        var c = i1.Visited.CompareTo(i2.Visited);
                        if (c != 0) return c;
                    }
                    {
                        var c = i1.ClosenessTo(3, 3).CompareTo(i2.ClosenessTo(3, 3));
                        if (c != 0) return c;
                    }
                    {
                        var c = (-i1.GetNewPositions(positions)).CompareTo(-i2.GetNewPositions(positions));
                        if (c != 0) return c;
                    }
                    {
                        var c = (-i1.AvailableDirections.Length).CompareTo(-i2.AvailableDirections.Length);
                        if (c != 0) return c;
                    }
                    return (-i1.Path.Length).CompareTo(-i2.Path.Length);
                });


                var iteration = iterations.First();
                iteration.Visited = true;
                Iteration[] newImprovements = iteration.Expand();

                foreach (var newImprovement in newImprovements)
                {
                    if (newImprovement.IsTarget())
                    {
                        LogManager.GetCurrentClassLogger().Log(newImprovement.Path == "DRURDRUDDLLDLUURRDULRLDUUDDDRR" ? LogLevel.Info : LogLevel.Warn, newImprovement);
                        Targets.Add(newImprovement);
                    }
                    else
                    {
                        iterations.Add(newImprovement);
                    }
                }

                if (Targets.Any())
                {
                    //purne all branches that lead to a target longer than the best one
                    Targets.Sort((i1, i2) => i1.Path.Length.CompareTo(i2.Path.Length));
                    var bestTarget = Targets.First();
                    var degeneratePathStarts = Targets.Select(i => i.Path.Substring(0, bestTarget.Path.Length)).Distinct().ToArray();
                    var removed = 0;
                    iterations.RemoveAll(i =>
                    {
                        var remove = degeneratePathStarts.Any(s => i.Path.StartsWith(s));
                        if (remove) removed++;
                        return remove;
                    });
                    if(removed > 0) LogManager.GetCurrentClassLogger().Info($"Removed {removed} degenerates");
                }
            }

            var bestIteration = Targets.OrderBy(i => i.Path.Length).First();
            return bestIteration;
        }
    }

    public static class Helpers
    {

        public static bool Improves(this char[] availablePositions, char[] otherAvailablePositions)
        {
            var improves = !availablePositions.All(otherAvailablePositions.Contains);
            return improves;
        }
    }

    public class Iteration 
    {
        private readonly Iterator iterator;

        public bool IsTarget()
        {
            return Pos.X == 3 && Pos.Y == 3;
        }
        public bool Visited { get; set; }

        public override string ToString()
        {
            return $"{Path}, {Pos}";
        }

        public int GetNewPositions(Pos[] existingPositions)
        {
            var newPositions = AvailablePositions.Count(t => !existingPositions.Contains(t.Item1));
            return newPositions;
        }

        public Tuple<Pos,char>[] AvailablePositions { get { return availablePositions.Value; } }
        private Tuple<Pos, char>[] GetAvailablePositions()
        {
            return AvailableDirections.Select(c => new Tuple<Pos, char>(Pos.Move(c), c))
                .Where(a => a.Item1.X >= 0 && a.Item1.X <= 3 && a.Item1.Y >= 0 && a.Item1.Y <= 3)
                .ToArray();
        }

        public Iteration[] Expand()
        {
            var positions = AvailablePositions
                .Select(a => new Iteration(a.Item1, Path + a.Item2, iterator))
                .Where(a => !iterator.Targets.Any(t => t.Path.StartsWith(a.Path)))
                .ToArray();
            return positions;
        }

        public Iteration(Pos pos, string path, Iterator iterator)
        {
            this.iterator = iterator;
            Pos = pos;
            Path = path;
            availableDirections = new Lazy<char[]>(GetAvailableDirections);
            availablePositions = new Lazy<Tuple<Pos, char>[]>(GetAvailablePositions);
        }

        public int ClosenessTo(int x, int y)
        {
            return x - Pos.X + y - Pos.Y;
        }

        private static readonly MD5 md5 = MD5.Create();
        public Pos Pos { get; }
        public string Path { get; }

        private static readonly char[] openChars = "bcdef".ToCharArray();
        private readonly Lazy<char[]> availableDirections;
        private readonly Lazy<Tuple<Pos, char>[]> availablePositions;

        private char[] GetAvailableDirections()
        {
            var hash = GetMd5Hash(iterator.Passcode + Path);
            var result = new List<char>();
            if (openChars.Contains(hash[0])) result.Add('U');
            if (openChars.Contains(hash[1])) result.Add('D');
            if (openChars.Contains(hash[2])) result.Add('L');
            if (openChars.Contains(hash[3])) result.Add('R');
            return result.ToArray();
        }

        public char[] AvailableDirections
        {
            get { return availableDirections.Value; }
        }

        static string GetMd5Hash(string input)
        {
            byte[] data = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(data);
            var res = hash.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2"))).ToString();
            return res;
        }
    }

    public class Pos : IEquatable<Pos>
    {
        public Pos Move(char c)
        {
            if(c == 'U') return new Pos(X, Y-1);
            if(c == 'D') return new Pos(X, Y+1);
            if(c == 'L') return new Pos(X-1, Y);
            if(c == 'R') return new Pos(X+1, Y);
            throw new ArgumentException($"Unrecognized direction {c}");
        }

        public Pos(int x, int y)
        {
            X = x;
            Y = y;
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

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
