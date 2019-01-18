/*
 * Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */
using System;
using System.Collections.Generic;

namespace Sanford.Multimedia.Midi.Score
{
    /** @class SymbolWidths
     * The SymbolWidths class is used to vertically align notes in different
     * tracks that occur at the same time (that have the same starttime).
     * This is done by the following:
     * - Store a list of all the start times.
     * - Store the width of symbols for each start time, for each track.
     * - Store the maximum width for each start time, across all tracks.
     * - Get the extra width needed for each track to match the maximum
     *   width for that start time.
     *
     * See method SheetMusic.AlignSymbols(), which uses this class.
     */

    public class SymbolWidths
    {

        /** Array of maps (starttime -> symbol width), one per track */
        private Dictionary<int, int>[] widths;        

        /** Map of starttime -> maximum symbol width */
        private Dictionary<int, int> maxwidths;
        private Dictionary<int, int> stmaxwidths; //FAB

        /** An array of all the starttimes, in all tracks */
        private int[] starttimes;
        private int[] ststarttimes; //FAB
        private int measurelen;

        /** Initialize the symbol width maps, given all the symbols in
         * all the tracks, plus the lyrics in all tracks.
         */
        public SymbolWidths(List<MusicSymbol>[] tracks, List<LyricSymbol>[] tracklyrics, int measlen)
        {
                        
            measurelen = measlen;

            /* Get the symbol widths for all the tracks */
            widths = new Dictionary<int, int>[tracks.Length];
            
            for (int track = 0; track < tracks.Length; track++)
            {
                if (tracks[track] != null)
                {
                    widths[track] = GetTrackWidths(tracks[track]);                    
                }
            }
            
            maxwidths = new Dictionary<int, int>();

            /* Calculate the maximum symbol widths */
            foreach (Dictionary<int, int> dict in widths)
            {
                if (dict != null)
                {
                    foreach (int time in dict.Keys)
                    {
                        if (!maxwidths.ContainsKey(time) ||
                            (maxwidths[time] < dict[time]))
                        {

                            maxwidths[time] = dict[time];
                        }
                    }
                }
            }

            if (tracklyrics != null)
            {
                foreach (List<LyricSymbol> lyrics in tracklyrics)
                {
                    if (lyrics == null)
                    {
                        continue;
                    }
                    foreach (LyricSymbol lyric in lyrics)
                    {
                        int width = lyric.MinWidth;
                        int time = lyric.StartTime;
                        if (!maxwidths.ContainsKey(time) ||
                            (maxwidths[time] < width))
                        {

                            maxwidths[time] = width;
                        }
                    }
                }
            }

            //FAB
            // Create a dictionary with max widths relative to 1 measure  
            stmaxwidths = new Dictionary<int, int>();
            int nummeasure = 0;
            int start = 0;            

            //int noire = measurelen / 4;
            //int croche = measurelen / 8;
            //int doublecroche = measurelen / 16;
            int triplecroche = measurelen / 32;
            //int quadruplecroche = measurelen / 64;
            
            List<int> tpcList = new List<int>();
            for (int i = 0; i < 32; i++ )
            {
                int t = i * triplecroche;
                tpcList.Add(t);
                stmaxwidths[t] = 12;
            }

            /* Calculate the maximum symbol widths */
            foreach (Dictionary<int, int> dict in widths)
            {
                if (dict != null)
                {

                    foreach (int time in dict.Keys)
                    {
                        int r = -1;
                        
                        nummeasure = time / measurelen;
                        start = time - nummeasure * measurelen;                        

                        r = tpcList.FindIndex(id => id == start);
                        if (r != -1)
                        {
                            if (start >= 0 && (!stmaxwidths.ContainsKey(start) || (stmaxwidths[start] < dict[time])))
                            {

                                stmaxwidths[start] = dict[time];
                            }
                        }
                    }
                }
            }

            if (tracklyrics != null)
            {
                foreach (List<LyricSymbol> lyrics in tracklyrics)
                {
                    if (lyrics == null)
                    {
                        continue;
                    }
                    foreach (LyricSymbol lyric in lyrics)
                    {
                        int width = lyric.MinWidth;
                        int time = lyric.StartTime;

                        nummeasure = time / measurelen;
                        start = time - nummeasure * measurelen;

                        if (!stmaxwidths.ContainsKey(start) ||
                            (stmaxwidths[start] < width))
                        {

                            stmaxwidths[start] = width;
                        }
                    }
                }
            }
            // END FAB
            

            /* Store all the start times to the starttime array */
            starttimes = new int[maxwidths.Keys.Count];
            maxwidths.Keys.CopyTo(starttimes, 0);
            Array.Sort<int>(starttimes);

            ststarttimes = new int[stmaxwidths.Keys.Count];
            stmaxwidths.Keys.CopyTo(ststarttimes, 0);
            Array.Sort<int>(ststarttimes);


        }


        /** Create a table of the symbol widths for each starttime in the track. */
        private static Dictionary<int, int> GetTrackWidths(List<MusicSymbol> symbols)
        {
            Dictionary<int, int> widths = new Dictionary<int, int>();

            foreach (MusicSymbol m in symbols)
            {
                int start = m.StartTime;
                int w = m.MinWidth;

                if (m is BarSymbol)
                {
                    continue;
                }
                else if (widths.ContainsKey(start))
                {
                    widths[start] += w;
                }
                else
                {
                    widths[start] = w;
                }
            }
            return widths;
        }

        /** Given a track and a start time, return the extra width needed so that
         * the symbols for that start time align with the other tracks.
         */
        public int GetExtraWidth(int track, int start)
        {            
            if (!widths[track].ContainsKey(start))
            {
                if (!maxwidths.ContainsKey(start))
                {
                    return stGetWidth(start);
                    //return 12;                        // FAB a corriger
                }
                else
                {
                    return maxwidths[start];
                }
            }
            else
            {
                return maxwidths[start] - widths[track][start];
            }            
        }
     
        /// <summary>
        /// FAB : return the max width relative to 1 measure
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private int stGetWidth(int start)
        {
            int nummeasure = start / measurelen;
            int mestart = start - nummeasure * measurelen;
            return stmaxwidths[mestart];
        }


        /** Return an array of all the start times in all the tracks */
        public int[] StartTimes
        {
            get { return starttimes; }
        }

        public int[] stStartTimes
        {
            get { return ststarttimes; }
        }
    }
}
