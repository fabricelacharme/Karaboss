#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChordsAnalyser.cvalue
{
    public class value
    {

        double longa = 0.25;
        double breve = 0.5;
        double semibreve = 1;
        double minim = 2;
        double crotchet = 4;
        double quaver = 8;
        double semiquaver = 16;
        double demisemiquaver = 32;
        double hemidemisemiquaver = 64;

        // British notation is hilarious
        double quasihemidemisemiquaver = 128;
        double semihemidemisemiquaver = 128;

        // From the part of Europe that is traditionally sane with units:
        int whole = 1;
        public int half = 2;
        public int quarter = 4;
        public int eighth = 8;
        int sixteenth = 16;
        int thirty_second = 32;
        int sixty_fourth = 64;
        int hundred_twenty_eighth = 128;

       

        double[] base_values = { 
            0.25,
            0.5,
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128,
        };

        double[] base_quintuplets = {
            0.3125,
            0.625,
            1.25,
            2.5,
            5,
            10,
            20,
            40,
            80,
            160,
        };
        double[] base_triplets = {
            0.375,
            0.75,
            1.5,
            3,
            6,
            12,
            24,
            48,
            96,
            192,
        };
        double[] base_septuplets = {
            0.4375,
            0.875,
            1.75,
            3.5,
            7,
            14,
            28,
            56,
            112,
            224,
        };




        /// <summary>
        /// Return the value of the two combined.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public double add(double value1, double value2)
        {
            /* 
            Example:
            >>> add(eighth, quarter)
            2.6666666666666665
            */
            return 1 / (1.0 / value1 + 1.0 / value2);
        }

        /// <summary>
        /// Return the note value for value1 minus value2.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public double subtract(double value1, double value2)
        {
            /*
            There are no exceptions for producing negative values, which can be
            useful for taking differences.
            
            Example:
            >>> subtract(quarter, eighth)
            8.0
            */
            return 1 / (1.0 / value1 - 1.0 / value2);
        }

        /// <summary>
        /// Return the dotted note value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nr"></param>
        /// <returns></returns>
        public double dots(double value, int nr = 1)
        {
            /*
            A dot adds half the duration of the note.A second dot adds half of what
            was added before, etc. So a dotted eighth note has the length of three
            sixteenth notes. An eighth note with two dots has the length of seven
            thirty second notes.

            Examples:
            >>> round(dots(eighth), 6)
            5.333333
            >>> round(dots(eighth, 2), 6)
            4.571429
            >>> round(dots(quarter), 6)
            2.666667
            */
            return (0.5 * value) / (1.0 - Math.Pow(0.5 ,(nr + 1)));
        }

        /// <summary>
        /// Return the triplet note value.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public double triplet(double value) {
            /*
            A triplet divides the base value above into three parts.So a triplet
            eighth note is a third of a quarter note.

            Examples:
            >>> triplet(eighth)
            12.0
            >>> triplet(4)
            6.0
            */            
            return tuplet(value, 3, 2);
        }

        /// <summary>
        /// Return the quintuplet note value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double quintuplet(double value) {
            /*
            A quintuplet divides the base value two above into five parts.So a
            quintuplet eighth note is a fifth of a half note.

            Examples:
            >>> quintuplet(8)
            10.0
            >>> quintuplet(4)
            5.0
            */           
            return tuplet(value, 5, 4);
        }


        public double septuplet(double value, bool in_fourths = true)
        {
            /*"""Return the septuplet note value.

            The usage of a septuplet is ambigious: seven notes can be played either
            in the duration of four or eighth notes.

            If in_fourths is set to True, this function will use 4, otherwise 8
            notes.So a septuplet eighth note is respectively either 14 or 7.

            Notice how
            >>> septuplet(8, False) == septuplet(4, True)
            True

            Examples:
            >>> septuplet(8)
            14.0
            >>> septuplet(8, False)
            7.0
            */
            if (in_fourths)
                return tuplet(value, 7, 4);
            else
                return tuplet(value, 7, 8);
        }

        /// <summary>
        /// Return a tuplet.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rat1"></param>
        /// <param name="rat2"></param>
        /// <returns></returns>
        private double tuplet(double value, int rat1, float rat2)
        {
            /*
            A tuplet can be written as a ratio.For example: 5:4 means that you play
            5 notes in the duration of 4 (a quintuplet), 3:2 means that you play 3
            notes in the duration of 2 (a triplet), etc.This function calculates
            the note value when playing in rat1:rat2.

            Example:
            >>> tuplet(8, 3, 2)
            12.0
            */
            return (rat1 * value) / (float)rat2;
        }


        public (double, int, int, int) determine(int value) {
            /* Analyse the value and return a tuple containing the parts it's made of.

            The tuple respectively consists of the base note value, the number of
            dots, and the ratio(see tuplet).

            Examples:
            >>> determine(8)
            (8, 0, 1, 1)
            >>> determine(12)
            (8, 0, 3, 2)
            >>> determine(14)
            (8, 0, 7, 4)

            This function recognizes all the base values, triplets, quintuplets,
            septuplets and up to four dots.The values are matched on range.
            */
            int i = -2;
            double v = -1;
            

            i = -2;
            foreach (double vv in base_values) {
                if (value == vv)
                    return (value, 0, 1, 1);
                if (value < vv)
                {
                    v = vv;
                    break;
                }
                i += 1;
            }
            float scaled = (float)value / (float)Math.Pow(2,i);
            if (scaled >= 0.9375) // base value
                return (base_values[i], 0, 1, 1);
            else if (scaled >= 0.8125)
            {
                // septuplet: scaled = 0.875
                return (base_values[i + 1], 0, 7, 4);
            }
            else if (scaled >= 17 / 24.0) {
                // triplet: scaled = 0.75
                return (base_values[i + 1], 0, 3, 2);
            }
            else if (scaled >= 31 / 48.0) {
                // dotted note (one dot): scaled = 2/3.0
                return (v, 1, 1, 1);
            }
            else if (scaled >= 67 / 112.0) {
                // quintuplet: scaled = 0.625
                return (base_values[i + 1], 0, 5, 4);
            }
            double d = 3;
            int x;
            for (x = 2; x <= 5; x++)
            {
                d += Math.Pow(2, x);
                if (scaled == Math.Pow(2.0,  x / d))
                    return (v, x, 1, 1);
            }
            return (base_values[i + 1], 0, 1, 1);
        }


    }
}
