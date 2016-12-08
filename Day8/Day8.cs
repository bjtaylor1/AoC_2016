using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day8
{
    [TestClass]
    public class Day8
    {
        [TestMethod]
        public void Example1()
        {
            var disp = new Display(7, 3);
            disp.ApplyInstruction("rect 3x2");
            Assert.AreEqual("\n" + 
                "###....\n" + 
                "###....\n" + 
                ".......\n", disp.Render());
        }

        [TestMethod]
        public void Example2()
        {
            var disp = new Display(7, 3);
            disp.ApplyInstruction("rect 3x2");
            disp.ApplyInstruction("rotate column x=1 by 1");
            Assert.AreEqual("\n" +
                "#.#....\n" +
                "###....\n" + 
                ".#.....\n", disp.Render());
        }

        [TestMethod]
        public void Example3()
        {
            var disp = new Display(7, 3);
            disp.ApplyInstruction("rect 3x2");
            disp.ApplyInstruction("rotate column x=1 by 1");
            disp.ApplyInstruction("rotate row y=0 by 4");
            Assert.AreEqual("\n" +
                "....#.#\n" + 
                "###....\n" + 
                ".#.....\n", disp.Render());
        }

        [TestMethod]
        public void Example4()
        {
            var disp = new Display(7, 3);
            disp.ApplyInstruction("rect 3x2");
            disp.ApplyInstruction("rotate column x=1 by 1");
            disp.ApplyInstruction("rotate row y=0 by 4");
            disp.ApplyInstruction("rotate column x=1 by 1");
            Assert.AreEqual("\n" +
                ".#..#.#\n" + 
                "#.#....\n" + 
                ".#.....\n", disp.Render());
        }
    }

    public class Display
    {
        public Display(int width, int height)
        {
            Pixels = new int[width][];
            for (int i = 0; i < width; i++)
            {
                Pixels[i] = Enumerable.Repeat(0, height).ToArray();
            }
        }

        public int[][] Pixels { get; }

        public void ApplyInstruction(string instruction)
        {
            Match m;
            if ((m = Regex.Match(instruction, @"^rect (\d+)x(\d+)$")).Success)
            {
                var wide = int.Parse(m.Groups[1].Value);
                var tall = int.Parse(m.Groups[2].Value);
                TurnOn(wide, tall);
            }
            else if ((m = Regex.Match(instruction, @"^rotate row y=(\d+) by (\d+)$")).Success)
            {
                var row = int.Parse(m.Groups[1].Value);
                var amount = int.Parse(m.Groups[2].Value);
                RotateRow(row, amount);
            }
            else if ((m = Regex.Match(instruction, @"^rotate column x=(\d+) by (\d+)$")).Success)
            {
                var col = int.Parse(m.Groups[1].Value);
                var amount = int.Parse(m.Groups[2].Value);
                RotateCol(col, amount);
            }
            else throw new ArgumentException($"Unrecognized instruction {instruction}");
        }

        public void RotateCol(int col, int amount)
        {
            var newCol = Transform(Pixels[col],amount);
            Pixels[col] = newCol;
        }

        public void RotateRow(int row, int amount)
        {
            var rowPixels = Pixels.Select(col => col[row]).ToArray();
            var newRowPixels = Transform(rowPixels, amount);
            for (int col = 0; col < Pixels.Length; col++)
            {
                Pixels[col][row] = newRowPixels[col];
            }
        }

        private static int[] Transform(int[] input, int amount)
        {
            var newPixels = input.Select((i, index) =>
            {
                var newIndex = (input.Length + index - amount)%input.Length;
                return input[newIndex];
            }).ToArray();
            return newPixels;
        }

        public void TurnOn(int wide, int tall)
        {
            for (int col = 0; col < wide; col++)
            {
                for (int row = 0; row < tall; row++)
                {
                    Pixels[col][row] = 1;
                }
            }
        }

        public string Render()
        {
            var renderings = new[] {".", "#"};
            var s = new StringBuilder("\n");
            for (int row = 0; row < Pixels[0].Length; row++)
            {
                s.Append(string.Join("", Pixels.Select(col => renderings[col[row]]).ToArray()) + "\n");
            }
            return s.ToString();
        }
    }
}
