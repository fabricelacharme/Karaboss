#region License

/* Copyright (c) 2024 Fabrice Lacharme
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

using System.Collections.Generic;

namespace MusicXml.Domain
{
	public class Measure
	{
		internal Measure()
		{
			Width = -1;
			MeasureElements = new List<MeasureElement>();
			lstVerseNumber = new List<int>();

        }

		public decimal Width { get; internal set; }
		
		// This can be any musicXML element in the measure tag, i.e. note, backup, etc
		public List<MeasureElement> MeasureElements { get; internal set; }
		
		public MeasureAttributes Attributes { get; internal set; }


        public List<string> Notes { get; set; } = new List<string>();

        public List<int> Durations { get; set; } = new List<int>();
        public int Tempo { get; set; } // key = measure, value = tempo
        public int Number { get; set; }

		// FAB : is measure belonging to a Verse?
		public List<int> lstVerseNumber { get; internal set; }	

    }

}
