using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day14
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Example()
        {
            var index = new KeyFinder("abc").FindIndex();
            Assert.AreEqual(22728, index);
        }

        [TestMethod]
        public void Feedback1()
        {
            var index = new KeyFinder("cuanljph").FindIndex();
            Assert.IsTrue(index < 24398); //wrong!
            Console.Out.WriteLine(index);
        }
    }

    public class KeyFinder
    {
        private readonly string salt;

        public KeyFinder(string salt)
        {
            this.salt = salt;
        }

        private readonly string[] containsTriple = Enumerable.Repeat("", UInt16.MaxValue).ToArray();
        private readonly string[] nextThousandContainsFive = Enumerable.Repeat("", UInt16.MaxValue).ToArray();
        private readonly List<int> validIndexes = new List<int>();
        private static readonly MD5 md5 = MD5.Create();
        public int FindIndex()
        {
            int limit = int.MaxValue;
            for (int i = 0; i < limit; i++)
            {
                var hash = GetMd5Hash($"{salt}{i}");
                Match m5;
                if ((m5 = isFive.Match(hash)).Success)
                {
                    for (int j = i - 1; j >= Math.Max(0, i - 1000); j--)
                    {
                        var char5 = m5.Groups[1].Value.First();
                        nextThousandContainsFive[j] += char5;
                        if (containsTriple[j].Contains(char5) && !validIndexes.Contains(j))
                        {
                            validIndexes.Add(j);

                            if (validIndexes.Count == 64)
                            {
                                limit = i + 10000;
                            }
                        }
                    }
                }
                Match m3;
                if ((m3 = isTriple.Match(hash)).Success)
                {
                    containsTriple[i] += m3.Groups[1].Value.First();
                }
            }
            validIndexes.Sort();
            var result = validIndexes[63];
            return result;
        }

        private static Regex isTriple = new Regex(@"([\w\d])\1\1", RegexOptions.Compiled);
        private static Regex isFive = new Regex(@"([\w\d])\1\1\1\1", RegexOptions.Compiled);

        static string GetMd5Hash(string input)
        {
            byte[] data = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

            var res = data.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2"))).ToString();
            return res;
        }

    }
}
