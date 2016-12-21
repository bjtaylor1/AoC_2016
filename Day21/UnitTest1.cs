using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day21
{
    [TestFixture]
    public class UnitTest1
    {

        [TestCase("swap position 4 with position 0", "abcde", "ebcda")]
        [TestCase("swap letter d with letter b", "ebcda", "edcba")]
        [TestCase("reverse positions 0 through 4", "edcba", "abcde" )]
        [TestCase("rotate left 1 step", "abcde", "bcdea")]
        [TestCase("move position 1 to position 4", "bcdea", "bdeac")]
        [TestCase("move position 3 to position 0", "bdeac", "abdec")]
        [TestCase("rotate based on position of letter b", "abdec", "ecabd")]
        [TestCase("rotate based on position of letter d", "ecabd", "decab")]
        public void Examples(string line, string password, string expectedPassword)
        {
            Scrambler.ProcessLine(line, ref password);
            Assert.AreEqual(expectedPassword, password);
        }
    }

    public class Scrambler
    {
        public static void ProcessLine(string line, ref string password)
        {
            Match m;
            char[] passwordChars = password.ToCharArray();
            if ((m = Regex.Match(line, @"^swap position (\d+) with position (\d+)$")).Success)
            {
                var posX = int.Parse(m.Groups[1].Value);
                var posY = int.Parse(m.Groups[2].Value);
                SwapPositions(passwordChars, posX, posY);
            }
            else if ((m = Regex.Match(line, @"^swap letter (\w) with letter (\w)$")).Success)
            {
                var cX = m.Groups[1].Value.Single();
                var cY = m.Groups[2].Value.Single();
                var posX = Array.IndexOf(passwordChars, cX);
                var posY = Array.IndexOf(passwordChars, cY);
                SwapPositions(passwordChars, posX, posY);
            }
            else if ((m = Regex.Match(line, @"^rotate (left|right) (\d+) steps?$")).Success)
            {
                var dirString = m.Groups[1].Value;
                var steps = int.Parse(m.Groups[2].Value);
                passwordChars = Rotate(passwordChars, dirString, steps);
            }
            else if ((m = Regex.Match(line, @"^rotate based on position of letter (\w)$")).Success)
            {
                var cX = m.Groups[1].Value.Single();
                var posX = Array.IndexOf(passwordChars, cX);
                var steps = 1 + posX + (posX >= 4 ? 1 : 0);
                passwordChars = Rotate(passwordChars, "right", steps);
            }
            else if ((m = Regex.Match(line, @"^reverse positions (\d+) through (\d+)$")).Success)
            {
                var start = int.Parse(m.Groups[1].Value);
                var end = int.Parse(m.Groups[2].Value);
                var length = end + 1 - start;
                var reversed = passwordChars.Skip(start).Take(length).Reverse().ToArray();
                Array.Copy(reversed, 0, passwordChars, start, length);
            }
            else if ((m = Regex.Match(line, @"^move position (\d+) to position (\d+)$")).Success)
            {
                var fromPos = int.Parse(m.Groups[1].Value);
                var toPos = int.Parse(m.Groups[2].Value);
                var fromChar = passwordChars[fromPos];
                var passwordCharsList = passwordChars.ToList();
                passwordCharsList.RemoveAt(fromPos);
                passwordCharsList.Insert(toPos, fromChar);
                passwordChars = passwordCharsList.ToArray();
            }
            else throw new ArgumentException($"Unrecognized instruction '{line}'", nameof(line));

            password = new string(passwordChars);
        }

        private static char[] Rotate(char[] passwordChars, string dirString, int steps)
        {
            int dir;
            if (dirString == "left") dir = 1;
            else if (dirString == "right") dir = -1;
            else throw new InvalidOperationException($"Invalid dir {dirString}");
            var newChars = passwordChars.Select((c, i) =>
            {
                var newIndex = (Math.Abs(steps*dir) * passwordChars.Length + i + steps*dir)%passwordChars.Length;
                return passwordChars[newIndex];
            }).ToArray();
            passwordChars = newChars;
            return passwordChars;
        }

        private static void SwapPositions(char[] passwordChars, int posX, int posY)
        {
            char cX = passwordChars[posX];
            char cY = passwordChars[posY];
            passwordChars[posX] = cY;
            passwordChars[posY] = cX;
        }
    }
}
