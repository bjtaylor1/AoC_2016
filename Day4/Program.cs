using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                        Console.Out.WriteLine(new Room("aaaaa-bbb-z-y-x-123[abxyz]").IsReal);
                        Console.Out.WriteLine(new Room("a-b-c-d-e-f-g-h-987[abcde]").IsReal);
                        Console.Out.WriteLine(new Room("not-a-real-room-404[oarel]").IsReal);
                        Console.Out.WriteLine(new Room("totally-real-room-200[decoy]").IsReal);

            */
            var rooms = File.ReadAllLines("input.txt").Select(s => new Room(s));
            var realRooms = rooms.Where(r => r.IsReal).ToArray();

            var sectorIds = realRooms.Sum(r => r.Number);

            foreach (var realRoom in realRooms)
            {
                realRoom.Decrypt();
                Console.Out.WriteLine(realRoom);
            }
            File.WriteAllLines("output.txt", realRooms.OrderBy(r => r.ToString()).Select(r => r.ToString()));
/*
            var testRoom= new Room("qzmt-zixmtkozy-ivhz-343[asdf]");
            testRoom.Decrypt();
*/
        }
    }

    class Room
    {
        private static readonly Regex regex = new Regex(@"^(?<name>[a-z\-]+)\-(?<number>\d+)\[(?<checksum>[a-z]+)\]$");
        private string name;
        public int Number { get; }
        public bool IsReal { get; }

        public Room(string s)
        {
            var match = regex.Match(s);
            if(!match.Success) throw new ArgumentException($"Invalid string {s}", nameof(s));
            name = match.Groups["name"].Value;
            Number = int.Parse(match.Groups["number"].Value);
            var checksum = match.Groups["checksum"].Value;

            var intendedChecksum = name.Replace("-", "")
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => g.Key)
                .Take(5)
                .ToArray();
            var intendedChecksumString = new string(intendedChecksum);
            IsReal = intendedChecksumString == checksum;
        }

        public void Decrypt()
        {
            var nameChars = name.ToArray();
            for (int i = 0; i < Number; i++)
            {
                for (int c = 0; c < nameChars.Length; c++)
                {
                    if (nameChars[c] == 'z') nameChars[c] = 'a';
                    else if (nameChars[c] == '-') nameChars[c] = ' ';
                    else if(nameChars[c] >= 'a' && nameChars[c] <= 'y') nameChars[c]++;
                }
            }
            name = new string(nameChars);
        }

        public override string ToString()
        {
            return $"{name}-{Number}";
        }
    }
}
