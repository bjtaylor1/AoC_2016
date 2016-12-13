using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Day13
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RenderExample()
        {
            Console.Out.WriteLine();
            Console.Out.WriteLine("  0123456789");
            for (int y = 0; y <= 6; y++)
            {
                Console.Out.Write($"{y} ");
                for (int x = 0; x <= 9; x++)
                {
                    Console.Out.Write(new Pos(x,y, 10, 0).IsFilled() ? "#" : " ");
                }
                Console.Out.WriteLine();
            }
        }

        [TestMethod]
        public void RouteExample()
        {
            var stepsToDest = Room.GetDistance(7, 4, 10);
            Assert.AreEqual(11, stepsToDest);
        }

        [TestMethod]
        public void RoutePart1()
        {
            var stepsToDest = Room.GetDistance(31,39,1358);
            Console.Out.WriteLine(stepsToDest);
            
        }


        [TestMethod]
        public void Part2()
        {
            var countUnder50 = Room.GetCountUnder(1358, 50);
            Console.Out.WriteLine(countUnder50);
        }
    }

    public static class Room
    {
        public static List<Pos> Visited { get; }= new List<Pos>();
        public static int GetDistance(int targetX, int targetY, int n)
        {
            Visited.Add(new Pos(1,1,n, 0));
            while (true)
            {
                Visited.Sort((lhs, rhs) => lhs.CrudeDistanceFrom(targetX, targetY).CompareTo(rhs.CrudeDistanceFrom(targetX, targetY)));
                List<Pos> moves = null;
                foreach (var orig in Visited)
                {
                    var allMoves = new List<Pos>
                    {
                        orig.Left(),
                        orig.Right(),
                        orig.Down(),
                        orig.Up()
                    };
                    var validUnfilledMoves = allMoves.Where(p => p.IsValid() && !p.IsFilled());
                    moves = validUnfilledMoves.Where(p => !Visited.Contains(p)).ToList();
                    if (moves.Any())
                    {
                        break;
                    }
                }
                if(moves == null || !moves.Any()) throw new InvalidOperationException("Stuck!");

                var dest = moves.FirstOrDefault(p => p.X == targetX && p.Y == targetY);
                if (dest != null) return dest.Depth;
                Visited.AddRange(moves);

            }
        }

        public static int GetCountUnder(int n, int maxDepth)
        {
            Visited.Add(new Pos(1,1,n, 0));
            while (true)
            {
                List<Pos> moves = null;
                foreach (var orig in Visited)
                {
                    var allMoves = new List<Pos>
                    {
                        orig.Left(),
                        orig.Right(),
                        orig.Down(),
                        orig.Up()
                    };
                    var validUnfilledMoves = allMoves.Where(p => p.IsValid() && !p.IsFilled() && p.Depth <= maxDepth);
                    moves = validUnfilledMoves.Where(p => !Visited.Contains(p)).ToList();
                    if (moves.Any())
                    {
                        break;
                    }
                }
                if (moves == null || !moves.Any())
                {
                    return Visited.Count;
                };

                Visited.AddRange(moves);

            }
        }



    }

    public class Pos : IEquatable<Pos>
    {
        private readonly int n;

        public bool IsValid()
        {
            return X >= 0 && Y >= 0;
        }

        public bool IsFilled()
        {
            var blah = X * X + 3 * X + 2 * X * Y + Y + Y * Y  + n;
            int result = 0;
            for (int guff = blah; guff > 0; guff >>= 1)
            {
                result += (guff & 1);
            }
            var filled = result % 2 == 1;
            return filled;
        }

        public int CrudeDistanceFrom(int targetX, int targetY)
        {
            return (targetX - X) + (targetY - Y);
        }

        public Pos(int x, int y, int n, int depth)
        {
            this.n = n;
            X = x;
            Y = y;
            Depth = depth;
        }

        public int X { get; }
        public int Y { get; }
        public int Depth { get;  }

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

        public Pos Right() { return new Pos(X+1, Y, n, Depth+1);}
        public Pos Left() { return new Pos(X-1, Y, n, Depth + 1);}
        public Pos Up() { return new Pos(X, Y+1, n, Depth + 1);}
        public Pos Down() { return new Pos(X, Y-1, n, Depth + 1);}

        public override string ToString()
        {
            return $"{X},{Y} (depth {Depth})";
        }
    }
}
