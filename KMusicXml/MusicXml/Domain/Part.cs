using System.Collections.Generic;
using System.Management.Instrumentation;

namespace MusicXml.Domain
{
	public class Part
	{
		internal Part()
		{
			Id = string.Empty;
			Name = string.Empty; 
			Measures = new List<Measure>();	

			// Fab
			MidiChannel = 0;
			MidiProgram = 0;
			Volume = 0;
			Pan = 0;
		}

		public string Id { get; internal set; }
		
		public string Name { get; internal set; }
		
		public List<Measure> Measures { get; internal set; }

        // Fab
        public int MidiChannel { get; internal set; }
        public int MidiProgram { get; internal set; }
        public int Volume { get; internal set; }
        public int Pan { get; internal set; }
    }
}
