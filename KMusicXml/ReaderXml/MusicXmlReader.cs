using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXml.Domain;
using Sanford.Multimedia.Midi;

namespace MusicXml
{
    public class MusicXmlReader
    {
        private Track track = new Track();
        private List<Track> newTracks;
        private List<MidiNote> newNotes = new List<MidiNote>();

        private StreamReader stream;

        private Sequence sequence;
        private int Format = 1;
        private int Numerator = 4;
        private int Denominator = 4;
        private int Division = 24;
        private int Tempo = 500000;


        private int currenttrack = -1;
        private int Channel = 0;
        private string TrackName = "Track1";
        private string InstrumentName = "AcousticGrandPiano";
        private int ProgramChange = 1;

        private int ControlChangeData1 = 0;
        private int ControlChangeData2 = 0;

        private int Volume = 0;
        private int Pan = 64;
        private int Reverb = 0;

        MidiNote n;

        // Constructor
        public MusicXmlReader() 
        {
            MidiTags.ResetTags();
        }

        /// <summary>
        /// Read MusicXml score and convert to Midi
        /// </summary>
        /// <param name="SC"></param>
        /// <returns></returns>
        public Sequence Read(MusicXml.Domain.Score SC) 
        {                         
            string Id = null;
            
            Identification Identification = SC.Identification;
            String MovementTitle = SC.MovementTitle;
            

            // List of tracks
            List<Part> Parts = SC.PartList;

            // Init sequence
            ReadHeader(1,480,SC.PartList.Count);

            // Calcul longueur mesure
            float mult = 4.0f / Denominator;
            int MeasureLength = Division * Numerator;
            MeasureLength = Convert.ToInt32(MeasureLength * mult);
            

            // Foreach track
            foreach (Part part in Parts)
            {
                TrackName = part.Name.Trim();
                Id = part.Id.Trim();
                Channel = part.MidiChannel;
                ProgramChange = part.MidiProgram;
                Volume = part.Volume;
                Pan = part.Pan;

                // Create track
                CreateTrack();


                List<string> Notes = new List<string>() { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
                List<int> NotesValues = new List<int>() { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

                List<Measure> Measures = part.Measures;
                foreach (Measure measure in Measures)
                {
                    decimal W = measure.Width;

                    int offset = 0;
                    foreach (string n in measure.Notes)
                    {
                        // NOTE_E4
                        if (n != "NOTE_REST")
                        {
                            if (n.IndexOf("S") == -1 &&  n.IndexOf("#") == -1)
                            {
                                string letter = n.Replace("NOTE_", "").Substring(0, 1);     //E
                                int octave = Convert.ToInt32(n.Substring(n.Length - 1, 1));  //4
                                
                                int duration = measure.Durations[measure.Notes.IndexOf(n)];
                                
                                int notenumber = (21 + Notes.IndexOf(letter)) + 12*(octave - 1);                                
                                
                                int starttime = offset + measure.Number * MeasureLength;

                                MidiNote note = new MidiNote(starttime, Channel, notenumber, duration, 80, false);
                                newNotes.Add(note);
                                track.addNote(note,false);
                                offset += duration;
                            }
                            else if (n.IndexOf("S") >0)
                            {
                                string letter = n.Replace("NOTE_", "").Substring(0, 1) + "#";     //E#
                                int octave = Convert.ToInt32(n.Substring(n.Length - 1, 1));  //4
                                int duration = measure.Durations[measure.Notes.IndexOf(n)];
                                int notenumber = (21 + Notes.IndexOf(letter)) + 12 * (octave - 1);

                                int starttime = offset + measure.Number * MeasureLength;

                                MidiNote note = new MidiNote(starttime, Channel, notenumber, duration, 80, false);
                                newNotes.Add(note);
                                track.addNote(note, false);
                                offset += duration;

                            }
                        }
                    }

                   /*
                    
                    MeasureAttributes measureAttributes = measure.Attributes;
                    if (measureAttributes != null)
                    {                        
                        if (measureAttributes.Divisions > 0)
                            Division = measureAttributes.Divisions;
                        Time t = measureAttributes.Time;
                        Clef clef = measureAttributes.Clef;
                        Key key = measureAttributes.Key;

                        if (measureAttributes.Time.Tempo > 0)
                            Tempo = measureAttributes.Time.Tempo;                        
                    }                   
                    
                    List<MeasureElement> lstME = measure.MeasureElements;   

                    // For each measure
                    foreach (MeasureElement measureElement in lstME)
                    {
                        object obj = measureElement.Element;
                        MeasureElementType metype = measureElement.Type;

                        switch (metype)
                        {
                            case MeasureElementType.Backup:break;
                            
                            case MeasureElementType.Note: 
                                Note note = (Note)obj;
                                string accidental = note.Accidental;
                                int staff = note.Staff;
                                bool isrest = note.IsRest;
                                bool ischordtone = note.IsChordTone;
                                Pitch pitch = note.Pitch;
                                int voice = note.Voice;
                                Lyric lyric = note.Lyric;
                                string ntype = note.Type;
                                int duration = note.Duration;
                                // Create note
                                CreateMidiNote(note);
                                break;

                            case MeasureElementType.Forward:break;                               

                        }
                    }
                   */



                }
            }

            CreateSequence();

            return sequence;
        }

        // =================================================================================================

        #region tracks

        private void ReadHeader(int format, int div, int tracks)
        {
            // Midi format
            Format = format;
            // Tracks Count
            newTracks = new List<Track>(tracks);
            // Division
            Division = div;
        }

        private void ReadTimeSignature(int numerator, int denominator)
        {
            // Track, Time, Time_signature, Num, Denom, Click, NotesQ
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// Create new track and add it to the list newtracks
        /// </summary>
        private void CreateTrack()
        {
            track = new Track()
            {
                MidiChannel = Channel,
                Name = TrackName,
                InstrumentName = InstrumentName,
                ProgramChange = ProgramChange,
                Volume = Volume,
                Pan = Pan,
                Reverb = Reverb,
            };

            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, track.MidiChannel, track.ProgramChange, 0);
            track.Insert(0, message);

            newTracks.Add(track);
        }



        #endregion tracks


        #region notes

        private void CreateMidiNote(Note n)
        {
            int starttime = 180;

            MidiNote note = new MidiNote(starttime, Channel, n.Pitch.Octave, n.Duration, n.Pitch.Alter, false);
            newNotes.Add(note);


        }

        #endregion notes

        #region sequence

        /// <summary>
        /// Create sequence
        /// </summary>
        private void CreateSequence()
        {
            // Create new sequence
            sequence = new Sequence(Division)
            {
                Format = Format,
                OrigFormat = 1,
                Numerator = Numerator,
                Denominator = Denominator,
                Tempo = Tempo,
                Time = new TimeSignature(Numerator, Denominator, Division, Tempo),
            };

            // Tracks to sequence
            for (int i = 0; i < newTracks.Count; i++)
            {
                sequence.Add(newTracks[i]);
            }
            //sequence.tracks = newTracks;

            // Insert Tempo in track 0
            if (sequence.tracks.Count > 0)
                sequence.tracks[0].insertTempo(Tempo);

            // Tags to sequence
            sequence.CloneTags();


        }

        #endregion sequence

    }
}
