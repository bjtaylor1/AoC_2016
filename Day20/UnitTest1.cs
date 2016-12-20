using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day20
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MergeContigious1()
        {
            Assert.AreEqual("1-6, 8-9", string.Join(", ", IPReader.MergeContigious(new uint[][]
            {
                new uint[] {1,2}, 
                new uint[] {3,4}, 
                new uint[] {5,6}, 
                new uint[] {8,9}
            }).Select(u => string.Join("-", u))));
        }

        [TestMethod]
        public void MergeContigious2()
        {
            Assert.AreEqual("1-4, 6-9", string.Join(", ", IPReader.MergeContigious(new uint[][]
            {
                new uint[] {1,2}, 
                new uint[] {3,4}, 
                new uint[] {6,9}, 
                new uint[] {8,9}
            }).Select(u => string.Join("-", u))));
        }

        [TestMethod]
        public void Example()
        {
            Assert.AreEqual((uint)3, IPReader.GetLowestUnblocked(new uint[][]
            {
                new uint[]{5,8},
                new uint[] {0,2},
                new uint[] {4,7}
            }, 0, 9));
        }

        [TestMethod]
        public void DoPuzzle()
        {
            var blocked = GetBlockedIpsOfInput();
            var lowestUnblocked = IPReader.GetLowestUnblocked(blocked, 0, 4294967295);
            Assert.IsTrue(lowestUnblocked > 1770166); //wrong!

            Console.Out.WriteLine(lowestUnblocked);
        }

        [TestMethod]
        public void Overlap()
        {
            Assert.IsTrue(IPReader.Overlap(new uint[] {0,10}, new uint[] {5,15}));
            Assert.IsTrue(IPReader.Overlap(new uint[] {uint.MaxValue - 20, uint.MaxValue - 10}, new uint[] {uint.MaxValue - 15,uint.MaxValue}));
            Assert.IsFalse(IPReader.Overlap(new uint[] {uint.MaxValue - 20, uint.MaxValue - 10}, new uint[] {uint.MaxValue - 5,uint.MaxValue}));
            Assert.IsFalse(IPReader.Overlap(new uint[] { 0, 5}, new uint[] { 10, 15 }));
            Assert.IsTrue(IPReader.Overlap(new uint[] { 0, 5}, new uint[] { 6, 15 }));
        }

        [TestMethod]
        public void ExamplePart2()
        {
            var blocked = new[]
            {
                new uint[] {5, 8},
                new uint[] {0, 2},
                new uint[] {4, 7}
            };
            Assert.AreEqual(2, IPReader.GetCountUnblocked(blocked, 0, 9));
        }

        [TestMethod]
        public void GetNumAllowed()
        {
            var blocked = GetBlockedIpsOfInput();
            Console.Out.WriteLine(IPReader.GetCountUnblocked(blocked, 0, 4294967295));
        }

        private static uint[][] GetBlockedIpsOfInput()
        {
            return File.ReadAllLines("input.txt")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.Split('-').Select(uint.Parse).OrderBy(i => i).ToArray())
                .ToArray();
        }
    }

    public class IPReader
    {
        public static uint GetLowestUnblocked(uint[][] blocked, uint lower, uint upper)
        {
            uint min = 0;
            var contigious = MergeContigious(blocked);
            foreach (var pair in contigious)
            {
                if (min >= pair[0] && min <= pair[1])
                {
                    min = pair[1] + 1;
                }
            }
            return min;
        }

        public static long GetCountUnblocked(uint[][] blocked, uint lower, uint upper)
        {
            var contigious = MergeContigious(blocked);
            var unallowed = contigious.Sum(pair => 1 + pair[1] - pair[0]);
            var allowed = 1 + (upper - lower - unallowed);
            return allowed;
        }

        public static uint[][] MergeContigious(uint[][] blockedArray)
        {
            uint[][] contigious = blockedArray;
            var didSomething = false;
            do
            {
                contigious = MergeContigiousOnce(contigious, out didSomething);
                contigious = contigious.OrderBy(u => u[0]).ToArray();
            } while (didSomething);
            
            for (int i = 1; i < contigious.Length; i++)
            {
                if (contigious[i][0] < contigious[i - 1][1]) throw new InvalidOperationException("Overlapping");
            }

            return contigious;
        }

        public static uint[][] MergeContigiousOnce(uint[][] blockedArray, out bool didSomething)
        {
            var blocked = blockedArray.ToList();
            var contigious = new List<uint[]>();
            didSomething = false;
            for (int index = 0; index < blocked.Count; index++)
            {
                int iContigious;
                var pair = blocked[index];
                while ((iContigious = blocked.FindIndex(index + 1, p => Overlap(p,pair))) != -1)
                {
                    var toMerge = blocked[iContigious];
                    pair[0] = Math.Min(pair[0], toMerge[0]);
                    pair[1] = Math.Max(pair[1], toMerge[1]);
                    blocked.RemoveAt(iContigious);
                    didSomething = true;
                }
                contigious.Add(pair);
            }
            return contigious.ToArray();
        }

        public static bool Overlap(uint[] p1, uint[] p2)
        {
            var dontOverlap = UBound(p1[1]) < p2[0]
                || LBound(p1[0]) > p2[1];
            return !dontOverlap;
        }

        private static uint LBound(uint p)
        {
            if (p == 0) return 0;
            return p - 1;
        }

        private static uint UBound(uint p)
        {
            if (p == uint.MaxValue) return uint.MaxValue;
            return p + 1;
        }
    }
}
