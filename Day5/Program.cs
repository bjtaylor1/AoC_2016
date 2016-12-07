using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = GetPassword2("ffykfhsq");

        }

        static string GetPassword2(string prefix)
        {
            var passwordChars = new char?[8];~
            int i = 0;
            while (passwordChars.Any(c => c == null))
            {
                if (IsHashInteresting(prefix + i))
                {
                    var value = GetMd5Hash(prefix + i);
                    var pos = value[5];
                    int posInt;
                    if(int.TryParse(pos.ToString(), out posInt) && posInt >= 0 && posInt <= 7 && passwordChars[posInt] == null)
                    if (pos >= '0' && pos <= '7')
                    {
                        passwordChars[posInt] = value[6];
                    }
                }
                i++;
            }
            var thePassword = new string(passwordChars.Cast<char>().ToArray());
            return thePassword;
        }

        static string GetPassword(string prefix)
        {
            var password = new StringBuilder();
            int i = 0;
            while (password.Length < 8)
            {
                if (IsHashInteresting(prefix + i))
                {
                    var value = GetMd5Hash(prefix + i)[5];
                    password.Append(value);
                }
                i++;
            }
            return password.ToString();
        }

        private static MD5 md5 = MD5.Create();
        static string Hash(string input)
        {
            var bytes = Encoding.ASCII.GetBytes(input);
            var hashed = md5.ComputeHash(bytes);
            var result = Encoding.ASCII.GetString(hashed);
            return result;
        }

        static bool IsHashInteresting(string input)
        {
            byte[] data = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            return data[0] == 0 && data[1] == 0 && data[2] <= 0xf;
        }

        static string GetMd5Hash(string input)
        {
            byte[] data = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

            var res = data.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2"))).ToString();
            return res;
        }
    }
}
