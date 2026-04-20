using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kar
{

    public enum KaraokeDisplayTypes
    {        
        None = 0,
        FixedLines = 1,
        ScrollingLinesBottomUp = 2,
        ScrollingLinesTopDown = 3,
        TwoLinesSwapped = 4,
        FourLinesSwapped = 5,
    }


    /// <summary>
    /// Represents a syllable with associated text, start time, and duration information.
    /// </summary>
    /// <remarks>A syllable instance encapsulates the text of the syllable, its start time in milliseconds,
    /// and its duration in milliseconds. If the duration is not specified, a default value of 45 milliseconds is used.
    /// This class is typically used in applications involving speech processing, karaoke timing, or linguistic analysis
    /// where precise timing of syllables is required.</remarks>
    [Serializable()]
    public class Syllable
    {
        public enum CharTypes
        {
            Text = 1,
            Information = 2,
            LineFeed = 3,
            ParagraphSep = 4,
        }
        public CharTypes CharType { get; set; }
        public string Text { get; set; }

        public string Chord { get; set; }

        //public bool IsChord = false;
        public bool IsChord
        {
            get {  return (CharType == CharTypes.Text && Chord != string.Empty && Text.IndexOf("--") == 0); }
        }

        public double StartTime { get; set; }
        public double Duration { get; set; }    // syllable duration

        public int TicksOn {  get; set; }
        public int TicksOff { get; set; }

        public int Beat { get; set; }


        public Syllable(string text, double startTime, double duration)
        {
            CharType = CharTypes.Text;
            Text = text;
            Beat = 0;
            Chord = string.Empty;            
            StartTime = startTime;
            Duration = duration;
            TicksOn = 0;
            TicksOff = 0;
        }

        public Syllable(string text, double startTime)
        {
            CharType = CharTypes.Text;
            Text = text;
            Beat = 0;
            Chord = string.Empty;            
            StartTime = startTime;
            Duration = 45;                // Default duration of 45 milliseconds if not specified
            TicksOn = 0;
            TicksOff = 0;
        }

        public Syllable()
        {
            CharType = CharTypes.Text;
            Text = string.Empty;
            Beat = 0;
            Chord = string.Empty;            
            StartTime = 0;
            Duration = 45;                // Default duration of 45 milliseconds if not specified
            TicksOn = 0;
            TicksOff = 0;
        }

    }

    /// <summary>
    /// Represents a line composed of a sequence of syllables, providing access to the syllables and their timing
    /// information.
    /// </summary>
    /// <remarks>A line typically corresponds to a single spoken or sung phrase, with each syllable containing
    /// its own timing and text. The start and end times of the line are determined by the first and last syllables,
    /// respectively. Modifying the collection of syllables will affect the line's timing and text
    /// representation.</remarks>
    [Serializable()]
    public class kLine
    {
        public List<Syllable> Syllables { get; set; }
        //public double StartTime => Syllables.First().StartTime;
        //public double EndTime => Syllables.Last().StartTime + Syllables.Last().Duration;

        public kLine(List<Syllable> syllables)
        {
            Syllables = syllables;
        }
        public kLine()
        {
            Syllables = new List<Syllable>();
        }

        /// <summary>
        /// Adds the specified syllable to the collection.
        /// </summary>
        /// <param name="syllable">The syllable to add to the collection. Cannot be null.</param>
        public void Add(Syllable syllable)
        {
            Syllables.Add(syllable);
        }

        /// <summary>
        /// Returns the full line of text by concatenating the text of all syllables.
        /// </summary>
        /// <returns>A string containing the concatenated text of all syllables. Returns an empty string if
        /// there are no syllables.</returns>
        public override string ToString()
        {
            return string.Join("", Syllables.Select(s => s.Text));
        }
    }

    /// <summary>
    /// Represents a collection of lyric lines, providing access to their timing and enumeration.
    /// </summary>
    /// <remarks>The Lyrics class allows management and traversal of a sequence of Line objects, each
    /// representing a segment of lyrics with associated timing. It supports enumeration and provides properties to
    /// access the start and end times of the entire lyrics collection. The class is suitable for scenarios where lyrics
    /// need to be displayed, synchronized, or manipulated as a group.</remarks>
    public class kLyrics : IEnumerable
    {

        private  readonly string _InternalSepLines = "¼";
        private  string _InternalSepParagraphs = "½";

        public List<kLine> Lines { get; set; }


        public kLyrics(List<kLine> lines)
        {
            Lines = lines;
        }

        public kLyrics()
        {
            Lines = new List<kLine>();
        }

        public void Add(kLine line)
        {
            Lines.Add(line);
        }


        public void Add(int index, kLine line)
        {
            Lines.Insert(index, line);
        }

        public int Count
        {
            get 
            {
                int count = 0;
                foreach (kLine line in Lines)
                {
                    count += line.Syllables.Count;
                }
                return count; 
            }
        }

        public void Include(kLyrics lyrics)
        {
            List<Syllable> lst = new List<Syllable>();
            
            // Convert KLyrics to a single line
            foreach (kLine line in Lines) 
            {
                for (int i = 0; i < line.Syllables.Count; i++)
                {
                    lst.Add(line.Syllables[i]);                    
                }
                
                if (line.Syllables.Last().CharType == Syllable.CharTypes.Text)
                {                   
                    Syllable s = new Syllable()
                    {
                        Text = _InternalSepLines,
                        Chord = string.Empty,
                        CharType = Syllable.CharTypes.LineFeed,
                        TicksOn = line.Syllables.Last().TicksOff,
                        TicksOff = line.Syllables.Last().TicksOff,
                    };
                    lst.Add(s);
                }
            }

            // Add KLyrics lyrics to the single line lst made of KLyrics
            foreach (kLine line in lyrics.Lines)
            {
                for (int i = 0; i < line.Syllables.Count; i++)
                {
                    lst.Add(line.Syllables[i]);
                }
            }
                                    
            // Sort the single line lst by TicksOn
            lst = lst.OrderBy(o => o.TicksOn).ToList();

            // Rebuild Lines structure
            kLine l = new kLine();
            kLyrics lineLyrics = new kLyrics();
            
            for (int i = 0; i < lst.Count;i++)
            {
                switch (lst[i].CharType)
                {
                    case Syllable.CharTypes.LineFeed:
                        if (l.Syllables.Count > 0)
                            lineLyrics.Add(l);
                        l = new kLine();
                        break;
                    case Syllable.CharTypes.ParagraphSep:
                        if (l.Syllables.Count > 0)
                            lineLyrics.Add(l);
                        
                        // line is a single syllable paragraph
                        l = new kLine();
                        l.Add(lst[i]);                        
                        lineLyrics.Add(l);
                        l = new kLine();
                        break;
                    case Syllable.CharTypes.Text:
                        l.Add(lst[i]);
                        break;
                }
            }
            if (l.Syllables.Count > 0)
                lineLyrics.Add(l);

            Lines = null;
            Lines = lineLyrics.Lines;
                                    
        }

        
        public void Include(kLine kLine)
        {
            // Transform KLine into a KLyrics with a single KLine
            kLyrics lineLyrics = new kLyrics();
            lineLyrics.Add(kLine);

            // Include KLYrics lineLyrics into this
            Include(lineLyrics);
        }
        
        
        public int IndexOf(kLine line)
        {
            return Lines.IndexOf(line);
        }

        public kLyrics Clone()
        {
            kLyrics result = new kLyrics();
            kLine line = new kLine();
            for (int i = 0; i < Lines.Count; i++)
            {
                line = new kLine();
                for (int j = 0; j < Lines[i].Syllables.Count; j++)
                {
                    line.Syllables.Add(Lines[i].Syllables[j]);
                }
                result.Add(line);
            }

            return result;  
        }


        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public LineEnum GetEnumerator()
        {
            return new LineEnum(Lines);
        }

    }

    public class LineEnum : IEnumerator
    {
        public List<kLine> Lines;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public LineEnum(List<kLine> list)
        {
            Lines = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < Lines.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public kLine Current
        {
            get
            {
                try
                {
                    return Lines[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
