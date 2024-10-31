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
