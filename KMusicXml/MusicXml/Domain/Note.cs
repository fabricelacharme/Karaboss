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
			Both,
		}

        public enum Articulations
        {
            accent,
            strongaccent,
            staccato,
            tenuto,
            detachedlegato,
            staccatissimo,
            spiccato,
            scoop,
            plop,
            doit,
            falloff,
            breathmark,
            caesura,
            stress,
            unstress,
            softaccent,
            otherarticulation,
        }

        #region declarations

        public string Accidental { get; internal set; }        
        public Articulations Articulation {  get; internal set; }                       
        
        public int ChromaticTranspose { get; internal set; }
        
        public int DrumInstrument { get; internal set; }
        public int Duration { get; internal set; }

        public bool IsChordTone { get; internal set; }
        public bool IsDrums { get; internal set; }       
        public bool IsRest { get; internal set; }

        // FAB : for verses (several lyrics on the same note with different "number")
        public List<Lyric> Lyrics { get; internal set; }

        public int OctaveChange { get; internal set; }

        public Pitch Pitch { get; internal set; }
        public Pitch PitchDrums { get; internal set; }

        public int Staff { get; internal set; }
        // Just to determinate if the note has to be played or not (pulsation with harmony)
        public string Stem { get; internal set; }


        public int TieDuration { get; internal set; }
        public TieTypes TieType { get; internal set; }
        public string Type { get; internal set; }

        public int Velocity { get; internal set; }
        public int Voice { get; internal set; }

        #endregion declarations

        /// <summary>
        /// Constructor
        /// </summary>
        internal Note()
		{
			Type = string.Empty;
			Duration = 0;
			TieDuration = 0;
			Voice = 1;
			Staff = -1;
			IsChordTone = false;

			IsDrums = false;
			DrumInstrument = 0;

			Velocity = 80;
			
			OctaveChange = 0;
			ChromaticTranspose = 0;

			// Fab
			Pitch = new Pitch();
			PitchDrums = new Pitch();

			TieType = TieTypes.None;
			
		}


        public Note Clone()
        {
            Note res = new Note()
            {                                                
                Accidental = Accidental,                                                                               
                Articulation = Articulation,

                ChromaticTranspose = ChromaticTranspose,
                
                DrumInstrument = DrumInstrument,
                Duration = Duration,

                IsChordTone = IsChordTone,
                IsDrums = IsDrums,
                IsRest = IsRest,
                                
                Lyrics = Lyrics,
                
                OctaveChange = OctaveChange,

                Pitch = Pitch,
                PitchDrums = PitchDrums,

                Staff = Staff,
                Stem = Stem,

                TieDuration = TieDuration,
                TieType = TieType,
                Type = Type,

                Velocity = Velocity,
                Voice = Voice,
            };
            return res;
        }
    }
}
