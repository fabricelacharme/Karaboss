using Sanford.Multimedia.Midi;

namespace MusicXml.Domain
{
	/*
	 *  <time>
     *     <beats>4</beats>
     *     <beat-type>4</beat-type>
     *   </time>	 	 
	 */ 
	
	public enum TimeSymbols
	{
		Normal, Common, Cut, SingleNumber
	}

	public class Time
	{
		internal Time()
		{
			Beats = 0;
			Mode = string.Empty;

			// Fab
			//Tempo = 0;
		}

		public int Beats { get; internal set; }
        public int BeatType { get; internal set; }

        // Not really the mode rather which note gets the beat
        public string Mode { get; internal set; }
		
		public TimeSymbols Symbol { get; internal set; }

        // Fab
        //public float Tempo { get; internal set; }

    }
}
