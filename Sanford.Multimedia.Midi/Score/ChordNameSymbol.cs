#region License

/* Copyright (c) 2025 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion

namespace Sanford.Multimedia.Midi.Score
{
    public class ChordNameSymbol
    {
        private int starttime;   /** The start time, in pulses */
        private string text;     /** The lyric text */
        private int x;           /** The x (horizontal) position within the staff */

        public ChordNameSymbol(int starttime, string text)
        {
            this.starttime = starttime;
            this.text = text;
        }

        public int StartTime
        {
            get { return starttime; }
            set { starttime = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int MinWidth
        {
            get { return minWidth(); }
        }

        /* Return the minimum width in pixels needed to display this lyric.
        * This is an estimation, not exact.
        */
        private int minWidth()
        {
            float widthPerChar = SheetMusic.LetterFont.GetHeight() * 2.0f / 3.0f;
            float width = text.Length * widthPerChar;
            if (text.IndexOf("i") >= 0)
            {
                width -= widthPerChar / 2.0f;
            }
            if (text.IndexOf("j") >= 0)
            {
                width -= widthPerChar / 2.0f;
            }
            if (text.IndexOf("l") >= 0)
            {
                width -= widthPerChar / 2.0f;
            }
            return (int)width;
        }

        public override string ToString()
        {
            return string.Format("ChordName start={0} x={1} text={2}", starttime, x, text);
        }


    }
}
