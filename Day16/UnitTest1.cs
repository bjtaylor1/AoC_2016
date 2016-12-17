using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day16
{
    [TestFixture]
    public class UnitTest1
    {
        [TestCase("1", "100")]
        [TestCase("0", "001")]
        [TestCase("11111", "11111000000")]
        [TestCase("111100001010", "1111000010100101011110000")]
        public void TestDragonCurve(string input, string result)
        {
            Assert.AreEqual(result, DataGenerator.DragonCurve(input));
        }

        [TestCase("110010110100", "100")]
        public void Checksum(string input, string result)
        {
            Assert.AreEqual(result, DataGenerator.GetChecksum(input));
        }

        [TestCase("10000", 20, "01100")]
        public void ChecksumOfData(string input, int diskLength, string result)
        {
            var data = DataGenerator.GetData(input, diskLength);
            var checksum = DataGenerator.GetChecksum(data);
            Assert.AreEqual(result, checksum);
        }

        [TestCase("00111101111101000", 272)] /* part 1 */
        [TestCase("00111101111101000", 35651584)] /* part 2 */
        public void DoPuzzle(string input, int diskLength)
        {
            var data = DataGenerator.GetData(input, diskLength);
            var checksum = DataGenerator.GetChecksum(data);
            Console.Out.WriteLine(checksum);
        }
    }

    public class DataGenerator
    {
        public static string GetData(string initial, int length)
        {
            string s = initial;
            while (s.Length < length)
            {
                s = DragonCurve(s);
            }
            return s.Substring(0, length);
        }

        public static string DragonCurve(string a)
        {
            var b = new string(a.Reverse().ToArray());
            b = Regex.Replace(b, "[01]", m => (1 - int.Parse(m.Value)).ToString());
            var result = a + "0" + b;
            return result;
        }

        public static string GetChecksum(string s)
        {
            if(s.Length % 2 != 0) throw new InvalidOperationException("Length must be even");
            var checksum = new StringBuilder();
            for (int i = 0; i < s.Length - 1; i+=2)
            {
                if (s[i] == s[i + 1])
                {
                    checksum.Append("1");
                }
                else
                {
                    checksum.Append("0");
                }
            }
            string result;
            if (checksum.Length%2 == 0)
            {
                result = GetChecksum(checksum.ToString());
            }
            else
            {
                result = checksum.ToString();
            }
            return result;
        }
    }
}
