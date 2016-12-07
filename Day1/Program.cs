using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            //examples:

/*
            Console.Out.WriteLine(new Pos().ApplyAll("R8, R4, R4, R8").Total);
            Console.Out.WriteLine(new Pos().ApplyAll("R2, L3").Total);
            Console.Out.WriteLine(new Pos().ApplyAll("R2, R2, R2").Total);
            Console.Out.WriteLine(new Pos().ApplyAll("R5, L5, R5, R3").Total);
*/

            //answer:
            Console.Out.WriteLine(new Pos().ApplyAll("L4, R2, R4, L5, L3, L1, R4, R5, R1, R3, L3, L2, L2, R5, R1, L1, L2, R2, R2, L5, R5, R5, L2, R1, R2, L2, L4, L1, R5, R2, R1, R1, L2, L3, R2, L5, L186, L5, L3, R3, L5, R4, R2, L5, R1, R4, L1, L3, R3, R1, L1, R4, R2, L1, L4, R5, L1, R50, L4, R3, R78, R4, R2, L4, R3, L4, R4, L1, R5, L4, R1, L2, R3, L2, R5, R5, L4, L1, L2, R185, L5, R2, R1, L3, R4, L5, R2, R4, L3, R4, L2, L5, R1, R2, L2, L1, L2, R2, L2, R1, L5, L3, L4, L3, L4, L2, L5, L5, R2, L3, L4, R4, R4, R5, L4, L2, R4, L5, R3, R1, L1, R3, L2, R2, R1, R5, L4, R5, L3, R2, R3, R1, R4, L4, R1, R3, L5, L1, L3, R2, R1, R4, L4, R3, L3, R3, R2, L3, L3, R4, L2, R4, L3, L4, R5, R1, L1, R5, R3, R1, R3, R4, L1, R4, R3, R1, L5, L5, L4, R4, R3, L2, R1, R5, L3, R4, R5, L4, L5, R2").Total);
        }
    }

    class Pos
    {
        double heading;
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Total { get { return Math.Abs(XPos + YPos); } }
        private List<Tuple<int,int>> locationsVisited = new List<Tuple<int, int>>();

        public Pos ApplyAll(string movements)
        {
            foreach (var movement in movements.Split(',').Select(s => s.Trim()))
            {
                if(ApplySingle(movement)) break;
            }
            return this;
        }

        public bool ApplySingle(string movement)
        {
            if (movement[0] == 'R')
                heading += Math.PI/2;
            else if (movement[0] == 'L')
                heading -= Math.PI/2;
            else throw new ArgumentOutOfRangeException(nameof(movement));
            int amount = int.Parse(movement.Substring(1));
            int x = (int)(Math.Sin(heading));
            int y = (int) (Math.Cos(heading));
            for (int n = 0; n < amount; n++)
            {
                XPos += x;
                YPos += y;
                var thisLocation = new Tuple<int, int>(XPos, YPos);
                if (locationsVisited.Contains(thisLocation)) return true;
                locationsVisited.Add(thisLocation);
            }
            return false;
        }
    }
}
