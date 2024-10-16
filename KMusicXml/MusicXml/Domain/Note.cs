using System.Collections.Generic;
using System.Xml;

namespace MusicXml.Domain
{
	public class Note
	{
		internal Note()
		{
			Type = string.Empty;
			Duration = -1;
			Voice = 1;
			Staff = -1;
			IsChordTone = false;

			// Fab
			Pitch = new Pitch();
		}

		public string Type { get; internal set; }
		
		public int Voice { get; internal set; }

		public int Duration { get; internal set; }
		
		
		// FAB : for verses (several lyrics on the same note with different "number")
		public List<Lyric> Lyrics { get; internal set; }

		public Pitch Pitch { get; internal set; }

		public int Staff { get; internal set; }

		public bool IsChordTone { get; internal set; }

		public bool IsRest { get; internal set; }
		
        public string Accidental { get; internal set; }
    }
}
