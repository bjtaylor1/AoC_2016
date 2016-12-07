using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
/*
            Console.Out.WriteLine(new IPv7("abba[mnop]qrst").SupportsTLS());
            Console.Out.WriteLine(new IPv7("abcd[bddb]xyyx").SupportsTLS());
            Console.Out.WriteLine(new IPv7("aaaa[qwer]tyui").SupportsTLS());
            Console.Out.WriteLine(new IPv7("ioxxoj[asdfgh]zxcvbn").SupportsTLS());

            var allIPs = File.ReadAllLines("input.txt").Select(s => new IPv7(s)).ToArray();
            var countSupportTls = allIPs.Count(i => i.SupportsTLS());
            Console.Out.WriteLine(allIPs.Count());
            Console.Out.WriteLine(countSupportTls);
*/


            Console.Out.WriteLine(new IPv7_2("aba[bab]xyz").SupportsSSL);
            Console.Out.WriteLine(new IPv7_2("xyx[xyx]xyx").SupportsSSL);
            Console.Out.WriteLine(new IPv7_2("aaa[kek]eke").SupportsSSL);
            Console.Out.WriteLine(new IPv7_2("zazbz[bzb]cdb").SupportsSSL);

            var allIps = File.ReadAllLines("input.txt").Select(s => new IPv7_2(s)).ToArray();
            Console.Out.WriteLine($"{allIps.Count(i => i.SupportsSSL)} out of {allIps.Length}");
        }
    }

    class IPv7_2
    {
        private readonly List<string> supernets = new List<string>(); //outside
        private readonly List<string> hypernets = new List<string>(); //inside

        public IPv7_2(string input)
        {
            bool inSquareBrackets = false;
            var s = new StringBuilder();
            foreach (char c in input)
            {
                if (c == '[')
                {
                    if(inSquareBrackets) throw new InvalidOperationException("Nested square brackets!");
                    supernets.Add(s.ToString());
                    s.Clear();
                    inSquareBrackets = true;
                }
                else if (c == ']')
                {
                    if (!inSquareBrackets) throw new InvalidOperationException("Too many closing square brackets!");
                    hypernets.Add(s.ToString());
                    s.Clear();
                    inSquareBrackets = false;
                }
                else s.Append(c);
            }
            if(inSquareBrackets) throw new InvalidOperationException("Too many opening square brackets!");
            if(s.Length > 0) supernets.Add(s.ToString());

            var abasInSupernets = supernets.SelectMany(GetAbas);
            SupportsSSL  = abasInSupernets.Any(m => hypernets.Any(h => h.Contains(Reverse(m))));
        }

        public bool SupportsSSL { get; }

        private static List<string> GetAbas(string s)
        {
            var abas = new List<string>();
            for (int i = 0; i < s.Length - 2; i++)
            {
                if (s[i] != s[i + 1] && s[i] == s[i + 2])
                {
                    abas.Add(s.Substring(i,3));
                }
            }
            return abas;
        }

        private static string Reverse(string aba)
        {
            if(aba.Length != 3) throw new InvalidOperationException($"Invalid aba {aba}");
            if(aba[0] != aba[2] || aba[0] == aba[1]) throw new InvalidOperationException($"Invalid aba {aba}");
            var bab = new string(new[] {aba[1], aba[0], aba[1]});
            return bab;
        }
    }

    class IPv7
    {
        private static readonly Regex abbaAtAll = new Regex(@"(\w)(\w)\2\1", RegexOptions.Compiled);
        private static readonly Regex abbaInBrackets = new Regex(@"\[[^\]]*(\w)(\w)\2\1[^\[]*\]", RegexOptions.Compiled);
        private bool any;
        private bool inBrackets;

        private static bool IsAbba(Match m)
        {
            bool isAbba = m.Groups[1].Value != m.Groups[2].Value;
            return isAbba;
        }

        public IPv7(string input)
        {
            any = abbaAtAll.Matches(input).Cast<Match>().Any(IsAbba);
            inBrackets = abbaInBrackets.Matches(input).Cast<Match>().Any(IsAbba);
        }

        public bool SupportsTLS()
        {
            return any && !inBrackets;
        }
    }
}
