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
            double multcoeff = 1;       // Mutiply everything in order to have Division >= 24
            
            Identification Identification = SC.Identification;
            String MovementTitle = SC.MovementTitle;
            

            // List of tracks
            List<Part> Parts = SC.PartList;

            // Init sequence
            newTracks = new List<Track>(Parts.Count);
            
            Division = Parts[0].Division;
            if (Division == 0)
                Division = 24;

            if (Division < 24) 
            {
                Division = 24;
                multcoeff = 24 / Parts[0].Division;
            }
            
            Tempo = Parts[0].Measures[0].Tempo;
            Format = 1;

            Numerator = Parts[0].Numerator;
            Denominator = Parts[0].Denominator;

            // Calcul longueur mesure
            float mult = 4.0f / Denominator;
            int MeasureLength = Division * Numerator;
            //MeasureLength = Convert.ToInt32(MeasureLength * mult * multcoeff);
            MeasureLength = Convert.ToInt32(MeasureLength * mult);

            int firstmeasure = 10;
            
            // For each track
            foreach (Part part in Parts)
            {
                if (part.Measures[0].Number < firstmeasure)
                    firstmeasure = part.Measures[0].Number;
            }

            // For each track
            foreach (Part part in Parts)
            {
                TrackName = part.Name.Trim();
                Id = part.Id.Trim();
                Channel = part.MidiChannel;
                if (Channel > 15)
                    break;

                ProgramChange = part.MidiProgram;
                Volume = part.Volume;
                Pan = part.Pan;

                // Create track
                CreateTrack();

                
                // https://www.inspiredacoustics.com/en/MIDI_note_numbers_and_center_frequencies
                List<string> Notes = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

                List<Measure> Measures = part.Measures;                               

                // For each measure
                foreach (Measure measure in Measures)
                {
                    decimal W = measure.Width;
                    int notenumber = 0;

                    
                    #region methode 1
                    /*
                    int offset = 0;
                    foreach (string n in measure.Notes)
                    {
                        // NOTE_E4
                        if (n != "NOTE_REST")
                        {
                            if (n.IndexOf("S") == -1 &&  n.IndexOf("#") == -1)
                            {
                                try
                                {
                                    string letter = n.Replace("NOTE_", "").Substring(0, 1);     //E

                                    int octave = Convert.ToInt32(n.Substring(n.Length - 1, 1));  //4
                                    int duration = (int)(measure.Durations[measure.Notes.IndexOf(n)] * multcoeff);
                                                                        
                                    notenumber = 12 + Notes.IndexOf(letter) + 12*octave;

                                    int starttime = 0;
                                    if (firstmeasure > 0)
                                        starttime = offset + (measure.Number - 1) * MeasureLength;
                                    else
                                        starttime = offset + measure.Number * MeasureLength;

                                    MidiNote note = new MidiNote(starttime, Channel, notenumber, duration, 80, false);
                                    newNotes.Add(note);
                                    track.addNote(note, false);
                                    offset += duration;
                                }
                                catch (Exception e)
                                {

                                }

                            }
                            else if (n.IndexOf("S") > 0)
                            {
                                try
                                {
                                    string letter = n.Replace("NOTE_", "").Substring(0, 1) + "#";     //E#
                                    int octave = Convert.ToInt32(n.Substring(n.Length - 1, 1));  //4
                                    int duration = (int)(measure.Durations[measure.Notes.IndexOf(n)] * multcoeff);

                                    notenumber = (21 + Notes.IndexOf(letter)) + 12 * octave;

                                    int starttime = 0;
                                    if (firstmeasure > 0)
                                        starttime = offset + (measure.Number - 1) * MeasureLength;
                                    else
                                        starttime = offset + measure.Number * MeasureLength;


                                    MidiNote note = new MidiNote(starttime, Channel, notenumber, duration, 80, false);
                                    newNotes.Add(note);
                                    track.addNote(note, false);
                                    offset += duration;
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                    */
                    #endregion methode 1

                    

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
                     */


                    #region methode 2
                    List<MeasureElement> lstME = measure.MeasureElements;
                    
                    // Manage the start time of notes
                    int timeline = 0;

                    // For each measureElement in current measure
                    foreach (MeasureElement measureElement in lstME)
                    {
                        object obj = measureElement.Element;
                        MeasureElementType metype = measureElement.Type;

                        switch (metype)
                        {
                            case MeasureElementType.Backup:
                                Console.WriteLine("backup");
                                Backup bkp = (Backup)obj;
                                timeline -= bkp.Duration;
                                break;
                            
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
                                
                                if (note.IsRest)
                                {
                                    timeline += note.Duration;
                                    break;
                                }


                                note.Duration = (int)(note.Duration * multcoeff);

                                int starttime = 0;
                                if (firstmeasure > 0)
                                    starttime = timeline + (measure.Number - 1) * MeasureLength;
                                else
                                    starttime = timeline + measure.Number * MeasureLength;

                                

                                int octave = note.Pitch.Octave;
                                string letter = note.Pitch.Step.ToString();
                                notenumber = 12 + Notes.IndexOf(letter) + 12 * octave;

                                if (note.Pitch.Alter != 0)
                                {
                                    int alter = note.Pitch.Alter;
                                    notenumber += alter;
                                }


                                // Create note
                                CreateMidiNote(note, notenumber, starttime);

                                if (!note.IsChordTone)
                                    timeline += note.Duration;

                                break;

                            case MeasureElementType.Forward:
                                Console.WriteLine("forward");
                                break;                               

                        }
                    }


                    #endregion methode 2

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

        private void CreateMidiNote(Note n, int v, int st)
        {
            if (v < 21)
                return;
            try
            {
                MidiNote note = new MidiNote(st, Channel, v, n.Duration, 80, false);
                newNotes.Add(note);
                track.addNote(note, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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

            // FAB
            if (sequence.Division == 0)
                sequence.Division = 1;

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
