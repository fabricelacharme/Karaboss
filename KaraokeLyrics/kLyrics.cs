using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kar
{
    /// <summary>
    /// Represents a syllable with associated text, start time, and duration information.
    /// </summary>
    /// <remarks>A syllable instance encapsulates the text of the syllable, its start time in milliseconds,
    /// and its duration in milliseconds. If the duration is not specified, a default value of 45 milliseconds is used.
    /// This class is typically used in applications involving speech processing, karaoke timing, or linguistic analysis
    /// where precise timing of syllables is required.</remarks>
    public class Syllable
    {
        public enum CharTypes
        {
            Text = 1,
            LineFeed = 2,
            ParagraphSep = 3,
        }
        public CharTypes CharType { get; set; }
        public string Text { get; set; }

        public string Chord { get; set; }
        public bool IsChord = false;

        public double StartTime { get; set; }
        public double Duration { get; set; }    // syllable duration

        public int TicksOn {  get; set; }
        public int TicksOff { get; set; }


        public Syllable(string text, double startTime, double duration)
        {
            CharType = CharTypes.Text;
            Text = text;
            Chord = string.Empty;
            IsChord = false;
            StartTime = startTime;
            Duration = duration;
            TicksOn = 0;
            TicksOff = 0;
        }

        public Syllable(string text, double startTime)
        {
            CharType = CharTypes.Text;
            Text = text;
            Chord = string.Empty;
            IsChord= false;
            StartTime = startTime;
            Duration = 45;                // Default duration of 45 milliseconds if not specified
            TicksOn = 0;
            TicksOff = 0;
        }

        public Syllable()
        {
            CharType = CharTypes.Text;
            Text = string.Empty;
            Chord = string.Empty;
            IsChord = false;
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
    public class kLine
    {
        public List<Syllable> Syllables { get; set; }
        public double StartTime => Syllables.First().StartTime;
        public double EndTime => Syllables.Last().StartTime + Syllables.Last().Duration;

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
        public List<kLine> Lines { get; set; }


        //public double StartTime => Lines.First().StartTime;
        //public double EndTime => Lines.Last().EndTime;

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


        public int IndexOf(kLine line)
        {
            return Lines.IndexOf(line);
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
