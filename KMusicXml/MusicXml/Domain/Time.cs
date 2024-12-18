using Sanford.Multimedia.Midi;

namespace MusicXml.Domain
{
	public enum TimeSymbol
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
			Tempo = 0;
		}

		public int Beats { get; internal set; }
		
		// Not really the mode rather which note gets the beat
		public string Mode { get; internal set; }
		
		public TimeSymbol Symbol { get; internal set; }

        // Fab
        public float Tempo { get; internal set; }

    }
}
