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
                    for (uint i = blockedIp[0]; i < blockedIp[1] + 1; i++)
                    {
                        fileStream.WriteByte(1);
                    }
                    LogManager.GetCurrentClassLogger().Info($"{blockedIp[0]} - {blockedIp[1]}");
                }
            }
        }

        const int BUFFERSIZE = 0Xfffffff;
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

        private static int GetLowestUnblocked()
        {
            byte[] buffer = new byte[BUFFERSIZE];
            int n = 0;
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

        private static int GetCountUnblocked()
        {
            byte[] buffer = new byte[BUFFERSIZE];
            int n = 0;
            int result = 0;
            using (var fileStream = File.OpenRead("ips.dat"))
            {
                while (n < fileStream.Length)
                {
                    LogManager.GetCurrentClassLogger().Info($"{n}, {((double)n) / fileStream.Length:0.00%}");
                    var n0 = fileStream.Read(buffer, 0, BUFFERSIZE);
                    result += buffer.Take(n0).Count(b => b == 0);
                    n += n0;
                }
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
