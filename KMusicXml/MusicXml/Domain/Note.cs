using System.Collections.Generic;
using System.Xml;

namespace MusicXml.Domain
{
	public class Note
	{
		public enum TieTypes
		{
			None,
			Start,
			Stop,
		}
		
		internal Note()
		{
			Type = string.Empty;
			Duration = -1;
			Voice = 1;
			Staff = -1;
			IsChordTone = false;

			IsDrums = false;
			DrumPitch = 0;

			Velocity = 80;
			
			OctaveChange = 0;
			ChromaticTranspose = 0;

			// Fab
			Pitch = new Pitch();

			TieType = TieTypes.None;
		}

		
		public TieTypes TieType {  get; internal set; }
		
		// Just to determinate if the note has to be played or not (pulsation with harmony)
		public string Stem {  get; internal set; }

		public string Type { get; internal set; }
		
		public int Voice { get; internal set; }

		public int Duration { get; internal set; }
        		
		public int Velocity { get; internal set; }

        // FAB : for verses (several lyrics on the same note with different "number")
        public List<Lyric> Lyrics { get; internal set; }

		public int ChromaticTranspose { get; internal set; }
		public int OctaveChange { get; internal set; }

		public Pitch Pitch { get; internal set; }

		public int Staff { get; internal set; }

		public bool IsChordTone { get; internal set; }

		public bool IsDrums { get; internal set; }

		public int DrumPitch { get; internal set; }

		public bool IsRest { get; internal set; }
		
        public string Accidental { get; internal set; }
    }
}
