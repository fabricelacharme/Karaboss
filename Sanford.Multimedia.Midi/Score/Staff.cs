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
using System.Drawing;
using static Sanford.Multimedia.Midi.Track;

namespace Sanford.Multimedia.Midi.Score
{
    /* @class Staff
    * The Staff is used to draw a single Staff (a row of measures) in the 
    * SheetMusic Control. A Staff needs to draw
    * - The Clef
    * - The key signature
    * - The horizontal lines
    * - A list of MusicSymbols
    * - The left and right vertical lines
    *
    * The height of the Staff is determined by the number of pixels each
    * MusicSymbol extends above and below the staff.
    *
    * The vertical lines (left and right sides) of the staff are joined
    * with the staffs above and below it, with one exception.  
    * The last track is not joined with the first track.
    */

    public class Staff
    {
        private List<MusicSymbol> symbols;  /** The music symbols in this staff */
        private List<LyricSymbol> lyrics;   /** The lyrics to display (can be null) */
        private List<TempoSymbol> lsttempos;     /** The tempo changes to display (minimum 1) */
        private List<ChordNameSymbol> lstchordnames; 
        private int ytop;                   /** The y pixel of the top of the staff */
        private ClefSymbol clefsym;         /** The left-side Clef symbol */
        private AccidSymbol[] keys;         /** The key signature symbols */
        private bool showMeasures;          /** If true, show the measure numbers */
        private int keysigWidth;            /** The width of the clef and key signature */
        private int width;                  /** The width of the staff in pixels */
        //private int height;                 /** The height of the staff in pixels */
        public int tracknum;               /** The track this staff represents */
        private int totaltracks;            /** The total number of tracks */
        private int starttime;              /** The time (in pulses) of first symbol */
        private int endtime;                /** The time (in pulses) of last symbol */
        private int measureLength;          /** The time (in pulses) of a measure */
        private int staffH;
       
       

        private Clef clef;                   /** FAB: clef of this staff */

        /** Create a new staff with the given list of music symbols,
         * and the given key signature.  The clef is determined by
         * the clef of the first chord symbol. The track number is used
         * to determine whether to join this left/right vertical sides
         * with the staffs above and below. The SheetMusicOptions are used
         * to check whether to display measure numbers or not.
         */
        public Staff(List<MusicSymbol> symbols, KeySignature key, MidiOptions options, int staffH , int tracknum, int totaltracks)
        {

            keysigWidth = SheetMusic.KeySignatureWidth(key);
            this.tracknum = tracknum;
            this.totaltracks = totaltracks;
            this.staffH = staffH;
            Maximized = true;
            
            // FAB
            //showMeasures = (options.showMeasures && tracknum == 0);
            showMeasures = options.showMeasures;

            measureLength = options.time.Measure;
            
            
            clef = FindClef(symbols);

            clefsym = new ClefSymbol(clef, 0, false);
            keys = key.GetSymbols(clef);
            this.symbols = symbols;
            CalculateWidth(options.scrollVert);

            this.Height = CalculateHeight();
            this.Height = staffH;
            
            CalculateStartEndTime();
            FullJustify();
        }


        #region properties

        private bool visible;
        public bool Visible
        {
            get
            { return visible; }
            set
            { visible = value; }
        }
        
        /** Return the width of the staff */
        public int Width
        {
            get { return width; }
        }

        /** Return the height of the staff */
        public int Height { get; set; }

        /** Return the track number of this staff (starting from 0 */
        public int Track
        {
            get { return tracknum; }
        }

        /** Return the starting time of the staff, the start time of
         *  the first symbol.  This is used during playback, to 
         *  automatically scroll the music while playing.
         */
        public int StartTime
        {
            get { return starttime; }
        }

        /** Return the ending time of the staff, the endtime of
         *  the last symbol.  This is used during playback, to 
         *  automatically scroll the music while playing.
         */
        public int EndTime
        {
            get { return endtime; }
            set { endtime = value; }
        }

        /// <summary>
        /// FAB: Clef of the stafff
        /// </summary>
        public Clef Clef
        {
            get { return clef; }
            set { clef = value; }
        }
        
        /// <summary>
        /// Is staff maximized or minimized
        /// </summary>
        public bool Maximized { get; set; }

        #endregion properties


        /** Find the initial clef to use for this staff.  Use the clef of
         * the first ChordSymbol.
         */
        private Clef FindClef(List<MusicSymbol> list)
        {
            foreach (MusicSymbol m in list)
            {
                if (m is ChordSymbol)
                {
                    ChordSymbol c = (ChordSymbol)m;
                    return c.Clef;
                }
            }
            return Clef.Treble;
        }

 
        /** Calculate the width of this staff */
        private void CalculateWidth(bool scrollVert)
        {
            if (scrollVert)
            {
                width = SheetMusic.PageWidth;
                return;
            }
            width = keysigWidth;
            foreach (MusicSymbol s in symbols)
            {
                width += s.Width;
            }
        }


        /** Calculate the start and end time of this staff. */
        private void CalculateStartEndTime()
        {
            starttime = endtime = 0;
            if (symbols.Count == 0)
            {
                return;
            }
            starttime = symbols[0].StartTime;
            foreach (MusicSymbol m in symbols)
            {
                if (endtime < m.StartTime)
                {
                    endtime = m.StartTime;
                }
                if (m is ChordSymbol)
                {
                    ChordSymbol c = (ChordSymbol)m;
                    if (endtime < c.EndTime)
                    {
                        endtime = c.EndTime;
                    }
                }
            }
        }


        /** Full-Justify the symbols, so that they expand to fill the whole staff. */
        private void FullJustify()
        {
            if (width != SheetMusic.PageWidth)
                return;

            int totalwidth = keysigWidth;
            int totalsymbols = 0;
            int i = 0;

            while (i < symbols.Count)
            {
                int start = symbols[i].StartTime;
                totalsymbols++;
                totalwidth += symbols[i].Width;
                i++;
                while (i < symbols.Count && symbols[i].StartTime == start)
                {
                    totalwidth += symbols[i].Width;
                    i++;
                }
            }

            int extrawidth = (SheetMusic.PageWidth - totalwidth - 1) / totalsymbols;
            if (extrawidth > SheetMusic.NoteHeight * 2)
            {
                extrawidth = SheetMusic.NoteHeight * 2;
            }
            i = 0;
            while (i < symbols.Count)
            {
                int start = symbols[i].StartTime;
                symbols[i].Width += extrawidth;
                i++;
                while (i < symbols.Count && symbols[i].StartTime == start)
                {
                    i++;
                }
            }
        }

        /** Draw the lyrics */
        private void DrawLyrics(Graphics g, Rectangle clip, Pen pen)
        {
            /* Skip the left side Clef symbol and key signature */
            int xpos = keysigWidth;
            //int ypos = height - 12;
            int ypos = this.Height - 20; //FAB
            string t = string.Empty;

            foreach (LyricSymbol lyric in lyrics)
            {
                if ( (xpos + lyric.X >= clip.X - lyric.MinWidth - 50) && (xpos + lyric.X <= clip.X + clip.Width + 50))
                {
                    t = lyric.Text;
                    t = t.Replace("½","");
                    t = t.Replace("¼", "");

                    g.DrawString(t,
                             SheetMusic.LetterFont,
                             Brushes.Black,
                             xpos + lyric.X, ypos);
                }
            }
        }

        /// <summary>
        /// Draw a black note plus the value of the tempo (BPM)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        /// <param name="pen"></param>
        private void DrawTempos(Graphics g, Rectangle clip, Pen pen)
        {
            int xpos = keysigWidth;
            int ypos = 15;                            
            string tx = string.Empty;
            System.Drawing.Brush brush = System.Drawing.Brushes.Black;

            foreach (TempoSymbol TempoSymbol in lsttempos)
            {
                if ((xpos + TempoSymbol.X >= clip.X - 50) && (xpos + TempoSymbol.X <= clip.X + clip.Width + 50))
                {                    
                    TempoSymbol.Draw(g, pen, xpos, ypos);                    
                }
            }
        }


        private void DrawChordNames(Graphics g, Rectangle clip, Pen pen)
        {
            /* Skip the left side Clef symbol and key signature */
            int xpos = keysigWidth;            
            int ypos = 20; // same as tempos ... Check this
            string t = string.Empty;

            foreach (ChordNameSymbol chord in lstchordnames)
            {
                if ((xpos + chord.X >= clip.X - chord.MinWidth - 50) && (xpos + chord.X <= clip.X + clip.Width + 50))
                {
                    t = chord.Text;

                    g.DrawString(t,
                             SheetMusic.ChordNameFont,
                             Brushes.Black,
                             xpos + chord.X, ypos);
                }
            }
        }

        /** Draw the measure numbers for each measure */
        private void DrawMeasureNumbers(Graphics g, Rectangle clip, Pen pen)
        {

            /* Skip the left side Clef symbol and key signature */
            int xpos = keysigWidth;
            int ypos = ytop - SheetMusic.NoteHeight * 3;

            //Brush br = Brushes.Black;
            Brush br = Brushes.Gray;

            foreach (MusicSymbol s in symbols)
            {
                if ((xpos <= clip.X + clip.Width + 50) && (xpos + s.Width + 50 >= clip.X))
                {

                    if (s is BarSymbol)
                    {
                        int measure = 1 + s.StartTime / measureLength;

                        
                        g.DrawString("" + measure,
                                     SheetMusic.LetterFont,
                                     br,
                                     xpos + SheetMusic.NoteWidth / 2,
                                     ypos);


                    }
                }
                xpos += s.Width;

            }
        }     

        /** Draw the five horizontal lines of the staff */
        private void DrawHorizLines(Graphics g, Pen pen)
        {
            int line = 1;

            // FAB : Haut de la portée
            //int y = ytop - SheetMusic.LineWidth;
            int y = ytop;
            //int y = 30;


            pen.Width = 1;
            for (line = 1; line <= 5; line++)
            {
                g.DrawLine(pen, SheetMusic.LeftMargin, y, width - 1, y);
                y += SheetMusic.LineWidth + SheetMusic.LineSpace;
            }
            pen.Color = Color.Black;

        }

        /** Draw the vertical lines at the far left and far right sides. */
        private void DrawEndLines(Graphics g, Pen pen)
        {
            pen.Width = 1;

            /* Draw the vertical lines from 0 to the height of this staff,
             * including the space above and below the staff, with two exceptions:
             * - If this is the first track, don't start above the staff.
             *   Start exactly at the top of the staff (ytop - LineWidth)
             * - If this is the last track, don't end below the staff.
             *   End exactly at the bottom of the staff.
             */
            int ystart, yend;

            if (tracknum == 0)
            {
                ystart = ytop - SheetMusic.LineWidth;
                //ystart = ytop - 4 * SheetMusic.LineWidth;
            }
            else
                ystart = 0;

            if (tracknum == (totaltracks - 1))
            {
                //yend = ytop + 3 * SheetMusic.NoteHeight;
                //yend = ytop;
                yend = ytop + 23 * SheetMusic.LineWidth;
            }
            else
            {
                //yend = height;
                yend = this.Height; // FAB
            }

            g.DrawLine(pen, SheetMusic.LeftMargin, ystart,
                            SheetMusic.LeftMargin, yend);

            g.DrawLine(pen, width - 1, ystart, width - 1, yend);

        }

        
        #region methods

        /** Calculate the height of this staff.  Each MusicSymbol contains the
         * number of pixels it needs above and below the staff.  Get the maximum
         * values above and below the staff.
         */
        public int CalculateHeight()
        {
            int above = 0;
            int below = 0;
            int hheight = 0;

            foreach (MusicSymbol s in symbols)
            {
                above = Math.Max(above, s.AboveStaff);
                below = Math.Max(below, s.BelowStaff);
            }
            above = Math.Max(above, clefsym.AboveStaff);
            below = Math.Max(below, clefsym.BelowStaff);
            if (showMeasures)
            {
                above = Math.Max(above, SheetMusic.NoteHeight * 3);
            }

            // Haut de la portée
            //ytop = above + SheetMusic.NoteHeight;
            //ytop += 10; //FAB


            // Z CITY
            //ytop = 33; // FAB
            ytop = staffH / 3;

            hheight = SheetMusic.NoteHeight * 5 + ytop + below;
            if (lyrics != null)
            {
                hheight += 12;
            }

            /* Add some extra vertical space between the last track
             * and first track.
             */
            if (tracknum == totaltracks - 1)
                hheight += SheetMusic.NoteHeight * 3;

            return hheight;
        }


        /** Add the lyric symbols that occur within this staff.
         *  Set the x-position of the lyric symbol. 
         */
        public void AddLyrics(List<LyricSymbol> tracklyrics)
        {
            if (tracklyrics == null) return;
            
            lyrics = new List<LyricSymbol>();

            int xpos = 0;
            int symbolindex = 0;

            foreach (LyricSymbol lyric in tracklyrics)
            {
                if (lyric.StartTime < starttime)
                {
                    continue;
                }
                if (lyric.StartTime >= endtime)
                {
                    break;
                }
                /* Get the x-position of this lyric */
                while (symbolindex < symbols.Count &&  symbols[symbolindex].StartTime < lyric.StartTime)
                {
                    xpos += symbols[symbolindex].Width;
                    symbolindex++;
                }
                lyric.X = xpos;
                if (symbolindex < symbols.Count && (symbols[symbolindex] is BarSymbol))
                {
                    lyric.X += SheetMusic.NoteWidth;
                }
                lyrics.Add(lyric);
            }
            if (lyrics.Count == 0)
            {
                lyrics = null;
            }
        }

        
        public void AddChordNames(List<ChordNameSymbol> chordNames)
        {
            if (chordNames == null) return;
            
            int xpos = 0;
            int symbolindex = 0;
            lstchordnames = new List<ChordNameSymbol>();
            
            foreach (ChordNameSymbol chord in chordNames)
            {
                if (chord.StartTime < starttime)
                {
                    continue;
                }
                if (chord.StartTime >= endtime)
                {
                    break;
                }
                /* Get the x-position of this lyric */
                while (symbolindex < symbols.Count && symbols[symbolindex].StartTime < chord.StartTime)
                {
                    xpos += symbols[symbolindex].Width;
                    symbolindex++;
                }
                chord.X = xpos;
                if (symbolindex < symbols.Count && (symbols[symbolindex] is BarSymbol))
                {
                    chord.X += SheetMusic.NoteWidth;
                }
                lstchordnames.Add(chord);
                if (lstchordnames.Count == 0) lstchordnames = null;
            }


        }

        /// <summary>
        /// Add Tempo symbols to the first staff
        /// </summary>
        /// <param name="TempoSymbols"></param>
        public List<TempoSymbol> AddTempos(List<TempoSymbol> TempoSymbols)
        {
            if (TempoSymbols == null)
             return null;

            int xpos = 0;
            int symbolindex = 0;
            lsttempos = new List<TempoSymbol>();

            foreach (TempoSymbol TempoSymbol in TempoSymbols) 
            {
                if (TempoSymbol.StartTime < starttime)
                {
                    continue;
                }
                if (TempoSymbol.StartTime >= endtime)
                {
                    break;
                }

                /* Get the x-position of this TempoSymbol */
                while (symbolindex < symbols.Count && symbols[symbolindex].StartTime < TempoSymbol.StartTime)
                {
                    xpos += symbols[symbolindex].Width;
                    symbolindex++;
                }
                TempoSymbol.X = xpos;
                if (symbolindex < symbols.Count && (symbols[symbolindex] is BarSymbol))
                {
                    TempoSymbol.X += SheetMusic.NoteWidth;
                }
                lsttempos.Add(TempoSymbol);
            }
            
            if (lsttempos.Count == 0)
            {
                lsttempos = null;
            }
            return lsttempos;
        }

        /** Draw this staff. Only draw the symbols inside the clip area */
        public void Draw(Graphics g, Rectangle clip, Rectangle selRect, Pen pen)
        {
            int xpos = SheetMusic.LeftMargin + 5;
            int yy = ytop + SheetMusic.LineWidth;
            
            /* Draw the left side Clef symbol */
            g.TranslateTransform(xpos, 0);
            clefsym.Draw(g, pen, yy, this.tracknum, selRect, xpos);

            g.TranslateTransform(-xpos, 0);
            xpos += clefsym.Width;


            /* Draw the key signature */
            foreach (AccidSymbol a in keys)
            {
                g.TranslateTransform(xpos, 0);
                a.Draw(g, pen, yy, this.tracknum, selRect, xpos);
                g.TranslateTransform(-xpos, 0);
                xpos += a.Width;
            }

            /* Draw the actual notes, rests, bars.  Draw the symbols one 
             * after another, using the symbol width to determine the
             * x position of the next symbol.
             *
             * For fast performance, only draw symbols that are in the clip area.
             */
            foreach (MusicSymbol s in symbols)
            {
                // deselect all MusicSymbol if selection rectangle is null
                if (selRect.Width == 0)
                    s.Selected = false;
                


                // Draw only in clip area
                if ((xpos <= clip.X + clip.Width + 50) && (xpos + s.Width + 50 >= clip.X))
                {                    
                    g.TranslateTransform(xpos, 0);
                    s.Draw(g, pen, yy, this.tracknum, selRect, xpos);  // FAB add xpos and staff
                    g.TranslateTransform(-xpos, 0);
                }               
                xpos += s.Width;
            }

            DrawHorizLines(g, pen);
            DrawEndLines(g, pen);

            // Draw measure numbers
            if (showMeasures)           
                DrawMeasureNumbers(g, clip, pen);
            

            if (lyrics != null)            
                DrawLyrics(g, clip ,pen);            

            // Draw tempo symbols
            if (lsttempos != null)
                DrawTempos(g, clip, pen);

            // Draw ChordNames
            if (lstchordnames != null)
                DrawChordNames(g, clip, pen);
        }


        public List<MidiNote> getSelectedNotes()
        {
            List<MidiNote> L = new List<MidiNote>();
            foreach (MusicSymbol s in symbols)
            {                
                // s is a chordsymbol
                if (s.GetType() == typeof(ChordSymbol))
                {
                    // Pour une raison que j'ignore, on perd la valeur Selected du symbole, mais pas celle des notes ...
                    //if (s.Selected)
                    //{
                        ChordSymbol C = (ChordSymbol)s;

                        foreach (MidiNote n in C.Notes)
                        {
                            if (n.Selected)
                                L.Add(n);
                        }
                    //}
                }
            }
            return L;
        }

        public void ClearSelectedNotes()
        {
            foreach (MusicSymbol s in symbols)
            {
                // s is a chordsymbol
                if (s.GetType() == typeof(ChordSymbol))
                {
                    ChordSymbol C = (ChordSymbol)s;
                    foreach (MidiNote n in C.Notes)
                    {
                        n.Selected = false;                            
                    }                    
                }                
            }
        }


        /// <summary>
        /// Using SlNotes, select notes
        /// </summary>
        /// <param name="SlNotes"></param>
        public void RestoreSelectedNotes(List<MidiNote> SlNotes)
        {
            for (int i = 0; i < SlNotes.Count; i++)
            {
                MidiNote note = SlNotes[i];

                foreach (MusicSymbol s in symbols)
                {
                    // s is a chordsymbol
                    if (s.GetType() == typeof(ChordSymbol))
                    {
                        ChordSymbol C = (ChordSymbol)s;
                        foreach (MidiNote n in C.Notes)
                        {
                            if (note.Number == n.Number && note.StartTime == n.StartTime)
                            {
                                n.Selected = true;
                                s.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        public void selectNotes(int ticksFrom, int ticksTo)
        {
            foreach (MusicSymbol s in symbols)
            {
                // s is a chordsymbol
                if (s.GetType() == typeof(ChordSymbol))
                {
                    ChordSymbol C = (ChordSymbol)s;
                    s.Selected = false;

                    foreach (MidiNote midinote in C.Notes)
                    {
                        if (midinote.StartTime >= ticksFrom && midinote.StartTime < ticksTo)
                        {
                            midinote.Selected = true;
                            s.Selected = true;
                            
                        }
                        else
                            midinote.Selected = false;
                    }
                }
            }
        }


        public void findScroll(int currentPulseTime, int prevPulseTime, ref int x_shade)
        {
            /* If there's nothing to unshade, or shade, return */
            if ((starttime > prevPulseTime || endtime < prevPulseTime) &&
                (starttime > currentPulseTime || endtime < currentPulseTime))
            {
                return;
            }

            /* Skip the left side Clef symbol and key signature */
            int xpos = keysigWidth;

            MusicSymbol curr = null;
            ChordSymbol prevChord = null;
            int prev_xpos = 0;

            /* Loop through the symbols. 
             * Unshade symbols where start <= prevPulseTime < end
             * Shade symbols where start <= currentPulseTime < end
             */
            for (int i = 0; i < symbols.Count; i++)
            {
                curr = symbols[i];
                if (curr is BarSymbol)
                {
                    xpos += curr.Width;
                    continue;
                }

                int start = curr.StartTime;
                int end = 0;

                if (i + 2 < symbols.Count && symbols[i + 1] is BarSymbol)
                {
                    end = symbols[i + 2].StartTime;
                }
                else if (i + 1 < symbols.Count)
                {
                    end = symbols[i + 1].StartTime;
                }
                else
                {
                    end = endtime;
                }


                // Sortir dès le premier symbole dont le startTime est supérieur à currentPulsetime
                /* If we've past the previous and current times, we're done. */
                if ((start > prevPulseTime) && (start > currentPulseTime))
                {
                    if (x_shade == 0)
                    {
                        x_shade = xpos;
                    }

                    return;
                }


                // comprend pas !!!!!!!!!!!!!!!
                /* If shaded notes are the same, we're done */
                if ((start <= currentPulseTime) && (currentPulseTime < end) &&
                    (start <= prevPulseTime) && (prevPulseTime < end))
                {

                    x_shade = xpos;
                    return;
                }
 

                /* If symbol is in the current time, draw a shaded background */
                if ((start <= currentPulseTime) && (currentPulseTime < end))
                {
                    x_shade = xpos;

                    //FAB: 09/04/2015
                    return;
                }
  

                if (curr is ChordSymbol)
                {
                    ChordSymbol chord = (ChordSymbol)curr;
                    if (chord.Stem != null && !chord.Stem.Receiver)
                    {
                        prevChord = (ChordSymbol)curr;
                        prev_xpos = xpos;
                    }
                }
                xpos += curr.Width;
            }
        }

        /** Shade all the chords played in the given time.
         *  Un-shade any chords shaded in the previous pulse time.
         *  Store the x coordinate location where the shade was drawn.
         */
        public void ShadeNotes(Graphics g, Rectangle clip ,SolidBrush shadeBrush, Pen pen, int currentPulseTime, int prevPulseTime, ref int x_shade, Rectangle selrect, int OffsetX)
        {
           
            /* If there's nothing to unshade, or shade, return */
            if ((starttime > prevPulseTime || endtime < prevPulseTime) &&
                (starttime > currentPulseTime || endtime < currentPulseTime))
            {
                return;
            }

            int yy = ytop;

            /* Skip the left side Clef symbol and key signature */
            int xpos = keysigWidth;

            MusicSymbol curr = null;
            ChordSymbol prevChord = null;
            int prev_xpos = 0;

            /* Loop through the symbols. 
             * Unshade symbols where start <= prevPulseTime < end
             * Shade symbols where start <= currentPulseTime < end
             */
            for (int i = 0; i < symbols.Count; i++)
            {
                curr = symbols[i];
                if (curr is BarSymbol)
                {
                    xpos += curr.Width;
                    continue;
                }

                int start = curr.StartTime;
                int end = 0;

                if (i + 2 < symbols.Count && symbols[i + 1] is BarSymbol)
                {
                    end = symbols[i + 2].StartTime;
                }
                else if (i + 1 < symbols.Count)
                {
                    end = symbols[i + 1].StartTime;
                }
                else
                {
                    end = endtime;
                }


                // Sortir dès le premier symbole dont le startTime est supérieur à currentPulsetime
                /* If we've past the previous and current times, we're done. */
                if ((start > prevPulseTime) && (start > currentPulseTime))
                {
                    if (x_shade == 0)
                    {
                        x_shade = xpos;
                    }

                    return;
                }

                
                // comprend pas !!!!!!!!!!!!!!!
                /* If shaded notes are the same, we're done */
                if ((start <= currentPulseTime) && (currentPulseTime < end) &&
                    (start <= prevPulseTime) && (prevPulseTime < end))
                {

                    x_shade = xpos;
                    return;
                }

                bool redrawLines = false;

                /* If symbol is in the previous time, draw a white background */
                if ((start <= prevPulseTime) && (prevPulseTime < end))
                {
                    /*
                    g.TranslateTransform(xpos - 2, -2);
                    g.FillRectangle(Brushes.White, 0, 0, curr.Width + 4, this.Height + 4);
                    g.TranslateTransform(-(xpos - 2), 2);
                    g.TranslateTransform(xpos, 0);
                    curr.Draw(g, pen, yy);
                    g.TranslateTransform(-xpos, 0);

                    redrawLines = true;
                    */ 
                }

                /* If symbol is in the current time, draw a shaded background */
                if ((start <= currentPulseTime) && (currentPulseTime < end))
                {
                    x_shade = xpos;

                    //FAB: 09/04/2015
                    return;

                    /*
                    g.TranslateTransform(xpos, 0);
                    g.FillRectangle(shadeBrush, 0, 0, curr.Width, this.Height);
                    curr.Draw(g, pen, yy);
                    g.TranslateTransform(-xpos, 0);
                    redrawLines = true;
                    */ 
                }

                /* If either a gray or white background was drawn, we need to redraw
                 * the horizontal staff lines, and redraw the stem of the previous chord.
                 */
                if (redrawLines)
                {
                    int line = 1;
                    //int y = ytop - 5* SheetMusic.LineWidth;
                    int y = yy;  //FAB
                    //int y = ytop - SheetMusic.LineWidth;

                    pen.Width = 1;
                    g.TranslateTransform(xpos - 2, 0);
                    for (line = 1; line <= 5; line++)
                    {
                        g.DrawLine(pen, 0, y, curr.Width + 4, y);
                        y += SheetMusic.LineWidth + SheetMusic.LineSpace;
                    }
                    g.TranslateTransform(-(xpos - 2), 0);

                    if (prevChord != null)
                    {
                        g.TranslateTransform(prev_xpos, 0);
                        prevChord.Draw(g, pen, yy, this.tracknum, selrect, OffsetX);
                        g.TranslateTransform(-prev_xpos, 0);
                    }
                    if (showMeasures)
                    {
                        DrawMeasureNumbers(g, clip, pen);
                    }
                    if (lyrics != null)
                    {
                        DrawLyrics(g, clip ,pen);
                    }
                }

                if (curr is ChordSymbol)
                {
                    ChordSymbol chord = (ChordSymbol)curr;
                    if (chord.Stem != null && !chord.Stem.Receiver)
                    {
                        prevChord = (ChordSymbol)curr;
                        prev_xpos = xpos;
                    }
                }
                xpos += curr.Width;
            }
        }

        /** Return the pulse time corresponding to the given point.
         *  Find the notes/symbols corresponding to the x position,
         *  and return the startTime (pulseTime) of the symbol.
         */
        public int PulseTimeForPoint(Point point)
        {

            int xpos = keysigWidth;
            int pulseTime = starttime;
            
            foreach (MusicSymbol sym in symbols)
            {
                pulseTime = sym.StartTime;
                if (point.X <= xpos + sym.Width)
                {
                    return pulseTime;
                }
                xpos += sym.Width;
            }
            return pulseTime;
        }

   


        public override string ToString()
        {
            string result = "Staff clef=" + clefsym.ToString() + "\n";
            result += "  Keys:\n";
            foreach (AccidSymbol a in keys)
            {
                result += "    " + a.ToString() + "\n";
            }
            result += "  Symbols:\n";
            foreach (MusicSymbol s in keys)
            {
                result += "    " + s.ToString() + "\n";
            }
            foreach (MusicSymbol m in symbols)
            {
                result += "    " + m.ToString() + "\n";
            }
            result += "End Staff\n";
            return result;
        }

        #endregion methods
    }
}
