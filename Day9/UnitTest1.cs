using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Day9
{
    [TestFixture]
    public class UnitTest1
    {
        [TestCase("ADVENT", "ADVENT")]
        [TestCase("A(1x5)BC", "ABBBBBC")]
        [TestCase("(3x3)XYZ", "XYZXYZXYZ")]
        [TestCase("A(2x2)BCD(2x2)EFG", "ABCBCDEFEFG")]
        [TestCase("(6x1)(1x3)A", "(1x3)A")]
        [TestCase("X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY")]
        public void Examples(string input, string result)
        {
            Assert.AreEqual(result, Processor.Process(input));
        }
    }

    public class Processor
    {
        private static readonly Regex findPattern = new Regex(@"\((\d+)x(\d+)\)", RegexOptions.Compiled);
        public static string Process(string input)
        {
            var output = new StringBuilder();
            int marker = 0;
            Match m;
            while ((m = findPattern.Match(input, marker)).Success)
            {
                output.Append(input.Substring(marker, m.Index - marker));
                int numCharsToRepeat = int.Parse(m.Groups[1].Value);
                int numTimesToRepeat = int.Parse(m.Groups[2].Value);
                marker = m.Index + m.Length;
                var sequence = input.Substring(marker, numCharsToRepeat);
                var sequenceRepeated = string.Join("", Enumerable.Repeat(sequence, numTimesToRepeat));
                output.Append(sequenceRepeated);
                marker += numCharsToRepeat;
            }
            if (marker < input.Length)
            {
                output.Append(input.Substring(marker));
            }
            return output.ToString();
        }
    }
}
