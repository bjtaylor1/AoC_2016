using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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
        [TestCase("(5x2)(5x2)ADVENT", "(1x3)A")]
        [TestCase("X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY")]
        public void Examples(string input, string result)
        {
            Assert.AreEqual(result, Processor.ProcessAndReturnString(input, true));
        }

        [TestCase("(3x3)XYZ", "XYZXYZXYZ")]
        [TestCase("X(8x2)(3x3)ABCY", "XABCABCABCABCABCABCY")]
        public void ExamplesV2(string input, string result)
        {
            Assert.AreEqual(result, Processor.ProcessAndReturnString(input, true));
            Assert.AreEqual(result.Length, Processor.ProcessAndReturnLength(input));
        }

        [Test]
        public void ExampleV2_1()
        {
            Assert.AreEqual(string.Join("", Enumerable.Repeat("A", 241920)), 
                Processor.ProcessAndReturnString("(27x12)(20x12)(13x14)(7x10)(1x12)A", true));
            Assert.AreEqual(241920, 
                Processor.ProcessAndReturnLength("(27x12)(20x12)(13x14)(7x10)(1x12)A"));
        }

        [Test]
        public void ExampleV2_2()
        {
            var result = Processor.ProcessAndReturnLength("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN");
            Assert.AreEqual(445, result);
        }


        [Test]
        public void DoInput()
        {
            var input = File.ReadAllText($"{TestContext.CurrentContext.TestDirectory}\\input.txt").Trim();
            var result = Processor.ProcessAndReturnLength(input);
            Console.Out.WriteLine(result);
        }
    }


    public class Processor
    {
        private static readonly Regex findPattern = new Regex(@"\((\d+)x(\d+)\)", RegexOptions.Compiled);

        public static string ProcessAndReturnString(string input, bool v2 = false)
        {
            var outputFile = Process(input, v2);
            var result = File.ReadAllText(outputFile);
            return result;
        }

        public static long ProcessAndReturnLength(string input)
        {
            int index = 0;
            return ProcessAndReturnLength(input, ref index);
        }

        private static readonly Regex regex = new Regex(@"(?:[A-Z]+)|\((\d+)x(\d+)\)", RegexOptions.Compiled);
        private static long ProcessAndReturnLength(string input, ref int index)
        {
            long length = 0;
            Match match;
            
            while((match = regex.Match(input, index)).Success)
            {
                int numCharsToRepeat, numTimesToRepeat;
                index = match.Index + match.Length;
                if (int.TryParse(match.Groups[1].Value, out numCharsToRepeat) && int.TryParse(match.Groups[2].Value, out numTimesToRepeat))
                {
                    var stringToRepeat = input.Substring(index, numCharsToRepeat);
                    index += stringToRepeat.Length;
                    var repeatedString = string.Join("", Enumerable.Repeat(stringToRepeat, numTimesToRepeat));
                    int subIndex = 0;
                    var lengthOfRepeatedStringDecompressed = ProcessAndReturnLength(repeatedString, ref subIndex);
                    length += lengthOfRepeatedStringDecompressed;
                    
                }
                else
                {
                    length += match.Length;
                }
                
            }
            return length;
        }

        private static string Process(string input, bool v2)
        {
            bool found = true;
            var inputFile = $"{TestContext.CurrentContext.TestDirectory}\\inputfile.txt";
            File.WriteAllText(inputFile, input);
            string outputFile = inputFile;
            if (v2)
            {
                while (found)
                {
                    outputFile = ProcessSingleFile(inputFile, out found);
                    if (found) File.Copy(outputFile, inputFile, true);
                }
            }
            else
            {
                outputFile = ProcessSingleFile(inputFile, out found);
            }
            return outputFile;
        }

        private static string ProcessSingleFile(string inputFile, out bool found)
        {
            var outputFile = $"{TestContext.CurrentContext.TestDirectory}\\output.txt";

            using (var output = new StreamWriter(outputFile))
            {
                using (var inputStream = File.OpenRead(inputFile))
                {
                    Marker m;
                    found = false;
                    string extraText;
                    while ((m = Marker.FindNext(inputStream, out extraText)) != null)
                    {
                        found = true;
                        byte[] charsToRepeat = new byte[m.NumCharsToRepeat];
                        inputStream.Read(charsToRepeat, 0, m.NumCharsToRepeat);
                        output.Write(extraText);
                        var repeatedString = Encoding.ASCII.GetString(charsToRepeat);
                        for (int i = 0; i < m.NumTimesToRepeat; i++)
                        {
                            output.Write(repeatedString);
                        }
                    }
                    output.Write(extraText);

                }
            }
            return outputFile;
        }

        private static string ProcessSingle(string input, out bool found)
        {
            var output = new StringBuilder();
            int marker = 0;
            Match m;
            found = false;
            while ((m = findPattern.Match(input, marker)).Success)
            {
                found = true;
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

    class Marker
    {
        public int NumTimesToRepeat { get; }
        public int NumCharsToRepeat { get; }

        private static readonly Regex findPattern = new Regex(@"^(\d+)x(\d+)$", RegexOptions.Compiled);

        public static Marker FindNext(Stream streamReader, out string extraText)
        {
            int i;
            var preceedingText = new StringBuilder();
            Marker marker = null;
            while ((i = streamReader.ReadByte()) != -1)
            {
                var c = (char) i;
                if (c == '(')
                {
                    marker = new Marker(streamReader);
                    break;
                }
                else preceedingText.Append(c);
            }
            extraText = preceedingText.ToString();
            return marker;
        }

        public Marker(Stream streamReader)
        {
            int i;
            char c;
            var sb = new StringBuilder();
            while ((i = streamReader.ReadByte()) != -1 && (c = (char)i) != ')')
            {
                sb.Append(c);
            }
            var match = findPattern.Match(sb.ToString());
            if(!match.Success) throw new InvalidOperationException($"Invalid pattern near {streamReader.Position} = {sb}");
            NumCharsToRepeat = int.Parse(match.Groups[1].Value);
            NumTimesToRepeat = int.Parse(match.Groups[2].Value);
        }
    }
}
