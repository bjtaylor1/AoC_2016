using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace Day20b
{
    [TestClass]
    public class UnitTest1
    {
        private static uint[][] GetBlockedIpsOfInput()
        {
            return File.ReadAllLines("input.txt")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.Split('-').Select(uint.Parse).OrderBy(i => i).ToArray())
                .ToArray();
        }



        //[TestInitialize]
        public void Initialize()
        {
            var blockedIps = GetBlockedIpsOfInput();
            using (var fileStream = File.Create("ips.dat"))
            {
                foreach (var blockedIp in blockedIps)
                {
                    fileStream.Seek(blockedIp[0], SeekOrigin.Begin);
                    var count = (int)(blockedIp[1] - blockedIp[0]) + 1;
                    fileStream.Write(Enumerable.Repeat((byte)1, count).ToArray(), 0, count);
                    LogManager.GetCurrentClassLogger().Info($"{blockedIp[0]} - {blockedIp[1]}");
                }
            }
        }

        const int BUFFERSIZE = (int)(uint.MaxValue / 32)+1;
        [TestMethod]
        public void Part1()
        {
            Console.Out.WriteLine(GetLowestUnblocked());
        }

        [TestMethod]
        public void Part2()
        {
            Console.Out.WriteLine(GetCountUnblocked());
        }


        [TestMethod]
        public void Part2_b()
        {
            int count = (int) ((long) 4294967295/64);
            var l = new long[count];
        }

        private static long GetLowestUnblocked()
        {
            byte[] buffer = new byte[BUFFERSIZE];
            long n = 0;
            using (var fileStream = File.OpenRead("ips.dat"))
            {
                while (n < fileStream.Length)
                {
                    LogManager.GetCurrentClassLogger().Info($"{n}, {((double)n) / fileStream.Length:0.00%}");
                    var n0 = fileStream.Read(buffer, 0, BUFFERSIZE);
                    int i = Array.IndexOf(buffer, (byte)0, 0, n0);
                    if (i != -1) return n + i;
                    n += n0;
                }
                throw new InvalidOperationException("Not found!");
            }
        }

        private static long GetCountUnblocked()
        {
            byte[] buffer = new byte[BUFFERSIZE];
            long result = 0;
            long n = 0;
            using (var fileStream = File.OpenRead("ips.dat"))
            {
                int n0;
                do
                {
                    n0 = fileStream.Read(buffer, 0, BUFFERSIZE);
                    var toExamine = buffer.Take(n0).ToArray();
                    var count = toExamine.Count(b => b == 0);
                    var i = toExamine.Select((b, index) => new {b, index = (long)index}).Where(a => a.b == 0).Select(a => a.index + n).ToArray();
                    result += count;
                    LogManager.GetCurrentClassLogger().Info($"{n0}, {(double) fileStream.Position/fileStream.Length:0.00%}, result = {result}");
                    n += n0;
                } while (n0 > 0);
            }
            return result;
        }

        [TestMethod]
        public void TestMethod1()
        {
            Console.Out.WriteLine(new FileInfo("ips.dat").Length);
        }
    }
}
