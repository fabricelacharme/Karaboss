using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChordsAnalyser.ccmeter
{
    public class meter
    {
        Tuple<int,int> common_time = Tuple.Create(4, 4);
        Tuple<int, int> cut_time = Tuple.Create(2, 2);

        /// <summary>
        /// """Return true when log2(duration) is an integer."""
        /// </summary>
        /// <param name=""></param>
        private bool valid_beat_duration(int duration) {

            if (duration == 0)
                return false;
            else if (duration == 1)
                return true;
            else 
            {
                int r = duration;
                while (r != 1) {
                    if (r % 2 == 1)
                        return false;
                    r /= 2;
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// """Return true if meter is a valid tuple representation of a meter.
        /// </summary>
        /// <param name=""></param>
        private bool is_valid(Tuple<int,int> tmeter) 
        {
            /*
            Examples for meters are(3,4) for 3/4, (4,4) for 4/4, etc.
            */
            return tmeter.Item1 > 0 && valid_beat_duration(tmeter.Item2);
        }

        /// <summary>
        /// """Return true if meter is a compound meter, false otherwise.
        /// </summary>
        /// <param name=""></param>
        private bool is_compound(Tuple<int,int> tmeter) 
        {
            /*
            Examples:
            >>> is_compound((3,4))
            false
            >>> is_compound((4,4))
            false
            >>> is_compound((6,8))
            true
            */
            return is_valid(tmeter) &&  tmeter.Item1 % 3 == 0 && 6 <= tmeter.Item1;
        }

        /// <summary>
        /// """Return true if meter is a simple meter, false otherwise.
        /// </summary>
        /// <param name=""></param>
        private bool is_simple(Tuple<int,int> tmeter) 
        {
            /*
            Examples:
            >>> is_simple((3,4))
            true
            >>> is_simple((4,4))
            true
            */
            return is_valid(tmeter);
        }

        /// <summary>
        /// """Return true if meter is an asymmetrical meter, false otherwise.
        /// </summary>
        /// <param name=""></param>
        private bool is_asymmetrical(Tuple<int, int> tmeter) 
        {
            /*
            Examples:
            >>> is_asymmetrical((3,4))
            true
            >>> is_asymmetrical((4,4))
            false
            */
            return is_valid(tmeter) && tmeter.Item1 % 2 == 1;
        }
    }
}
