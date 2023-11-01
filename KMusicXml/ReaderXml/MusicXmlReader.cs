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
        // 2 tracks can be create for the same part
        private Track track1 = new Track();
        private Track track2 = new Track();
        
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
            newTracks = new List<Track>(Parts.Count);


            Tempo = Parts[0].Tempo;
            Format = 1;

            Numerator = Parts[0].Numerator;
            Denominator = Parts[0].Denominator;


            int firstmeasure = 10;
            
            // Search for First measure & tempo max
            foreach (Part part in Parts)
            {
                if (part.Tempo > Tempo)
                    Tempo = part.Tempo;
                
                if (part.Measures[0].Number < firstmeasure)
                    firstmeasure = part.Measures[0].Number;
            }

            // Search common Division for all parts
            int commondivision = Parts[0].Division;
            if (commondivision < 24)
                commondivision = 24;

            foreach (Part part in Parts)
            {
                 if (part.Division > commondivision)
                    commondivision = part.Division;
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

                /*
                 *  Attention, certaines partitions ont une division différente pour chaque piste !!!
                 *  Exemple BeetAnGeSample.xml
                 *  Part 1 : division 24
                 *  Part 2 : division 96
                 *  
                 *  Conclusion, il fait gérer chaque piste séparément
                 */
                double multcoeff = 1;       // Mutiply everything in order to have common Division
                Division = part.Division;
                if (Division != commondivision)
                {
                    Division = commondivision;
                    multcoeff = (double)commondivision/part.Division;  //commondivision / part.Division;
                }

                // Calcul longueur mesure
                float mult = 4.0f / Denominator;
                int MeasureLength = Division * Numerator;
                MeasureLength = Convert.ToInt32(MeasureLength * mult);


                // Create track
                // TODO : create 2 tracks sometimes 
                CreateTrack1();

                if (part.Staves > 1)
                    CreateTrack2();

                // https://www.inspiredacoustics.com/en/MIDI_note_numbers_and_center_frequencies
                List<string> Notes = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

                List<Measure> Measures = part.Measures;

                // Manage the start time of notes
                int timeline = 0;
                int offset = 0;                
                int starttime = 0;

                // For each measure
                foreach (Measure measure in Measures)
                {
                    decimal W = measure.Width;
                    int notenumber = 0;                                       

                    #region methode 2
                    List<MeasureElement> lstME = measure.MeasureElements;
                                                           
                    // For each measureElement in current measure
                    foreach (MeasureElement measureElement in lstME)
                    {
                        
                        object obj = measureElement.Element;
                        MeasureElementType metype = measureElement.Type;

                        switch (metype)
                        {
                            case MeasureElementType.Backup:                               
                                Backup bkp = (Backup)obj;                                
                                timeline -= (int)(bkp.Duration * multcoeff);
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
                                
                                note.Duration = (int)(note.Duration * multcoeff);

                                if (note.IsRest)
                                {                                        
                                    timeline += note.Duration;
                                    break;
                                }

                                // Take into account previous note                                
                                if (note.IsChordTone)
                                    offset = 0;
                                timeline += offset;                                                               

                                // For the next note (if not chord)
                                offset = note.Duration;

                                starttime = timeline;
                                int octave = note.Pitch.Octave;
                                string letter = note.Pitch.Step.ToString();
                                notenumber = 12 + Notes.IndexOf(letter) + 12 * octave;

                                if (note.Pitch.Alter != 0)
                                {
                                    int alter = note.Pitch.Alter;
                                    notenumber += alter;
                                }

                                // Create note
                                if (note.Staff <= 1)                                                                    
                                    CreateMidiNote1(note, notenumber, starttime);                                
                                else                                                                                                                                           
                                    CreateMidiNote2(note, notenumber, starttime);
                                
                                if (note.Lyric.Text != null)
                                {
                                    CreateLyric(note, starttime);
                                }


                                break;


                            case MeasureElementType.Forward:                                
                                Forward fwd = (Forward)obj;
                                timeline += (int)(fwd.Duration * multcoeff);
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

        /// <summary>
        /// Create new track and add it to the list newtracks
        /// </summary>
        private void CreateTrack1()
        {
            track1 = new Track()
            {
                MidiChannel = Channel,
                Name = TrackName,
                InstrumentName = InstrumentName,
                ProgramChange = ProgramChange,
                Volume = Volume,
                Pan = Pan,
                Reverb = Reverb,
                Numerator = Numerator,
                Denominator = Denominator
            };

            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, track1.MidiChannel, track1.ProgramChange, 0);
            track1.Insert(0, message);
           
            track1.insertTimesignature(Numerator, Denominator);

            newTracks.Add(track1);
        }

        /// <summary>
        /// Create new track and add it to the list newtracks
        /// </summary>
        private void CreateTrack2()
        {
            track2 = new Track()
            {
                MidiChannel = Channel,
                Name = TrackName,
                InstrumentName = InstrumentName,
                ProgramChange = ProgramChange,
                Volume = Volume,
                Pan = Pan,
                Reverb = Reverb,
                Numerator = Numerator,
                Denominator = Denominator
            };

            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, track2.MidiChannel, track2.ProgramChange, 0);
            track2.Insert(0, message);

            track2.insertTimesignature(Numerator, Denominator);

            newTracks.Add(track2);
        }

        #endregion tracks


        #region notes

        /// <summary>
        /// Create a MIDI note in the current rack
        /// </summary>
        /// <param name="n"></param>
        /// <param name="v"></param>
        /// <param name="st"></param>
        private void CreateMidiNote1(Note n, int v, int st)
        {
            
            if (v < 21)
                return;
            
            // TODO : the note may be created in a second track
            // if 2 tracks in the same Part (piano left & right for ex)
            try
            {
                MidiNote note = new MidiNote(st, Channel, v, n.Duration, 80, false);
                newNotes.Add(note);
                track1.addNote(note, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void CreateMidiNote2(Note n, int v, int st)
        {
            if (v < 21)
                return;

            // TODO : the note may be created in a second track
            // if 2 tracks in the same Part (piano left & right for ex)
            try
            {
                MidiNote note = new MidiNote(st, Channel, v, n.Duration, 80, false);
                newNotes.Add(note);
                track2.addNote(note, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion notes


        #region lyrics
        private void CreateLyric(Note n, int t)
        {
            try
            {
                string currentElement = n.Lyric.Text;
                byte[] newdata;
                switch (OpenMidiFileOptions.TextEncoding)
                {
                    case "Ascii":
                        //sy = System.Text.Encoding.Default.GetString(data);
                        newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                        break;
                    case "Chinese":
                        System.Text.Encoding chinese = System.Text.Encoding.GetEncoding("gb2312");
                        newdata = chinese.GetBytes(currentElement);
                        break;
                    case "Japanese":
                        System.Text.Encoding japanese = System.Text.Encoding.GetEncoding("shift_jis");
                        newdata = japanese.GetBytes(currentElement);
                        break;
                    case "Korean":
                        System.Text.Encoding korean = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                        newdata = korean.GetBytes(currentElement);
                        break;
                    case "Vietnamese":
                        System.Text.Encoding vietnamese = System.Text.Encoding.GetEncoding("windows-1258");
                        newdata = vietnamese.GetBytes(currentElement);
                        break;
                    default:
                        newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                        break;
                }
                
                MetaMessage mtMsg;
                // si lyrics de type lyrics
                mtMsg = new MetaMessage(MetaType.Lyric, newdata);

                
                switch (n.Lyric.Syllabic)
                {
                    case Syllabic.Begin: break;
                    case Syllabic.Single:
                        currentElement += " ";
                        break;
                    case Syllabic.End:
                        currentElement += " ";
                        break;
                    case Syllabic.None: break;
                        
                }
                // Update Track.Lyrics List
                Track.Lyric L = new Track.Lyric()
                {
                    Element = currentElement,
                    TicksOn = t,
                    Type = Track.Lyric.Types.Text,
                };

                // Insert new message
                if (n.Staff <= 1)
                {
                    track1.Insert(t, mtMsg);
                    track1.Lyrics.Add(L);
                    track1.TotalLyricsL += currentElement;
                }
                else
                {
                    track2.Insert(t, mtMsg);
                    track2.Lyrics.Add(L);
                    track2.TotalLyricsL += currentElement;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion lyrics

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

            // Insert Tempo in track 0
            if (sequence.tracks.Count > 0)
                sequence.tracks[0].insertTempo(Tempo);

            // Tags to sequence
            sequence.CloneTags();
        }

        #endregion sequence

    }
}
