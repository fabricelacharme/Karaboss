using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser.cvalue;
using ChordsAnalyser.ckeys;

namespace ChordsAnalyser
{
    public class tests
    {
        public tests()
        {
            cvalue.value v = new cvalue.value();
            ckeys.nkeys k = new ckeys.nkeys();


            Console.WriteLine(k.get_notes("F"));

            /*
              >>> round(dots(eighth), 6)
            5.333333
            >>> round(dots(eighth, 2), 6)
            4.571429
            >>> round(dots(quarter), 6)
            2.666667
            */
            Console.WriteLine(Math.Round(v.dots(v.eighth),6));
            Console.WriteLine(Math.Round(v.dots(v.quarter),6));
            


            /*
             *  Examples:
            >>> determine(8)
            (8, 0, 1, 1)
            >>> determine(12)
            (8, 0, 3, 2)
            >>> determine(14)
            (8, 0, 7, 4)
            */
            Console.WriteLine(v.determine(8));
            Console.WriteLine(v.determine(12));
            Console.WriteLine(v.determine(14));

            /*
             *Example:
            >>> add(eighth, quarter)
            2.6666666666666665
            */
            Console.WriteLine(v.add(v.eighth, v.quarter));



        }


    }
}
