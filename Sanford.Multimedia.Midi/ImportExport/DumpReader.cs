using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sanford.Multimedia.Midi
{
    public class DumpReader
    {
        private Track track = new Track();
        private List<Track> newTracks;
        private List<MidiNote> newNotes;

        private StreamReader stream;

        private Sequence sequence;
        private int Format = 1;
        private int Numerator = 0;
        private int Denominator = 0;
        private int Division = 0;
        private int Tempo = 0;


        private int currenttrack = -1;
        private int Channel = 0;
        private string TrackName = "Track1";
        private string InstrumentName = "AcousticGrandPiano";
        private int ProgramChange = 0;
        private int Volume = 0;
        private int Pan = 0;
        private int Reverb = 0;

        MidiNote n;
        

        // Constructor
        public DumpReader()
        {
            MidiTags.ResetTags();       
        }

        public Sequence Read(StreamReader strm)
        {
            this.stream = strm;
            string line = string.Empty;
            string[] array;

            while ((line = stream.ReadLine()) != null)
            {
                
                // Remove 'space' and '"'
                if (line.Contains("\""))
                {
                    // comma is the separator and there is a comma into the text
                    // Example: 2, 972, Lyric_t, "\Montez, Chris"
                    // put the text item having one or several comas into a single item
                    string text = line.Split('"', '"')[1];
                    if (text.Contains(','))
                    {
                        line = line.Replace(text, "lognx");  // hope this will never exist lol
                        array = line.Split(',').Select(p => p.Trim()).Select(p => p.Replace("\"", "")).ToArray();
                        array[array.Length - 1] = text;
                    }
                    else
                    {
                        // split normal
                        array = line.Split(',').Select(p => p.Trim()).Select(p => p.Replace("\"", "")).ToArray();
                    }
                }
                else // split normal
                    array = line.Split(',').Select(p => p.Trim()).Select(p => p.Replace("\"","")).ToArray();


                // Read lines, only a minimalistic amount of events is treated...
                // TODO: interpret more events
                if (array.Length > 2)
                {
                    
                    // 0, 0, Header, format, nTracks, division                    
                    if (array.Contains("Header"))
                    {
                        ReadHeader(array);
                    }
                    else if (array.Contains("Time_signature"))
                    {
                        ReadTimeSignature(array);
                    }
                    else if (array.Contains("Tempo"))
                    {
                        ReadTempo(array);
                    }
                    else if (array.Contains("Start_track"))
                    {
                        newNotes = new List<MidiNote>();
                        currenttrack++;
                    }
                    else if (array.Contains("Title_t"))
                    {
                        ReadTrackName(array);
                    }
                    else if (array.Contains("Instrument_name_t"))
                    {
                        ReadInstrumentName(array);
                    }
                    else if (array.Contains("Program_c"))
                    {
                        ReadProgramChange(array);
                        CreateTrack();
                    }
                    else if (array.Contains("Note_on_c"))
                    {
                        StartMidiNote(array);
                    }
                    else if (array.Contains("Note_off_c"))
                    {
                        StopMidiNote(array);
                    }
                    else if (array.Contains("Lyric_t"))
                    {
                        ReadMetaLyric(array);
                    }
                    else if (array.Contains("Text_t"))
                    {
                        ReadMetaText(array);
                    }
                    else if (array.Contains("End_track"))
                    {                        
                        //track = null;
                    }
                    else if (array.Contains("End_of_file"))
                    {
                        CreateSequence();
                    }
                }
            }

            return sequence;
        }

        #region header
        private void ReadHeader(string[] ar)
        {
            if (ar.Length != 6)
                throw new ArgumentException("Header Length");
            // 0, 0, Header, format, nTracks, division
            // Format
            Format = Convert.ToInt32(ar[3]);
            // Tracks Count
            newTracks = new List<Track>(Convert.ToInt32(ar[4]));
            // Division
            Division = Convert.ToInt32(ar[5]);
        }

        private void ReadTimeSignature(string[] ar)
        {
            if (ar.Length != 7)
                throw new ArgumentException("Time Signature Length");
            // Track, Time, Time_signature, Num, Denom, Click, NotesQ
            Numerator = Convert.ToInt32(ar[3]);
            Denominator = Convert.ToInt32(ar[4]);            
        }

        private void ReadTempo(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("Tempo Length");
            // Track, Time, Tempo, Number
            Tempo = Convert.ToInt32(ar[3]);
        }
        #endregion


        #region Track
        private void ReadTrackName(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("TrackName Length");
            // Track, Time, Title_t, Text
            TrackName = ar[3];
        }

        private void ReadInstrumentName(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("ProgramChange Length");
            // Track, Time, Instrument_name_t, Text
            InstrumentName = ar[2];
        }

        private void ReadProgramChange(string[] ar)
        {
            if (ar.Length != 5)
                throw new ArgumentException("ProgramChange Length");
            // Track, Time, Program_c, Channel, Program_num            
            Channel = Convert.ToInt32(ar[3]);
            ProgramChange = Convert.ToInt32(ar[4]);
        }

        private void ReadMetaLyric(string[] ar)
        {
            string sy = string.Empty;
            if (ar.Length != 4)
            {
                if (ar.Length < 4)
                    throw new ArgumentException("Meta Lyric Length");

                for (int i = 3; i < ar.Length; i++)
                {
                    sy += ar[i];
                }
            }
            else
            {
                sy = ar[3];
            }
            // Format: Track, Time, Lyric_t, Text
            int ticks = Convert.ToInt32(ar[1]);            
            byte[] newdata = Encoding.Default.GetBytes(sy);
            MetaMessage mtMsg = new MetaMessage(MetaType.Lyric, newdata);
            track.Insert(ticks, mtMsg);
            manageMetaLyrics(newdata, ticks);
        }

        private void ReadMetaText(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("Meta Text Length");
            // Format: Track, Time, Text_t, Text
            int ticks = Convert.ToInt32(ar[1]);
            string sy = ar[3];
            byte[] newdata = Encoding.Default.GetBytes(sy);
            MetaMessage mtMsg = new MetaMessage(MetaType.Text, newdata);
            track.Insert(ticks, mtMsg);
            manageMetaText(newdata, ticks);
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
        #endregion


        #region notes

        /// <summary>
        /// Note_on_c = Note on event, but sometimes also a note off
        /// </summary>
        /// <param name="ar"></param>
        private void StartMidiNote(string[] ar)
        {
            if (ar.Length != 6)
                throw new ArgumentException("Note On Length");

            // format of line: Track, Time, Note_on_c, Channel, Note, Velocity
            // format of Midinote: starttime, channel, notenumber, duration, velocity, selected
            int velocity = Convert.ToInt32(ar[5]);

            // velocity > 0 is a note on
            if (velocity > 0)
            {
                n = new MidiNote(Convert.ToInt32(ar[1]), Convert.ToInt32(ar[3]), Convert.ToInt32(ar[4]), 0, velocity, false);
                newNotes.Add(n);
            }
            else
            {
                // Note_on_c & velocity = 0 can be a note off in some midi files !!!!
                MidiNote no;
                int ticks = Convert.ToInt32(ar[1]);
                int notenumber = Convert.ToInt32(ar[4]);
                if (newNotes.Count > 0)
                {
                    for (int i = 0; i < newNotes.Count; i++)
                    {
                        no = newNotes[i];
                        if (no.Duration == 0 && no.Number == notenumber)
                        {
                            no.Duration = Convert.ToInt32(ar[1]) - n.StartTime;
                            track.addNote(no, false);
                            newNotes.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Note_off_c = Note off event
        /// </summary>
        /// <param name="ar"></param>
        private void StopMidiNote(string[] ar)
        {
            if (ar.Length != 6)
                throw new ArgumentException("Note Off Length");
            // format of line: Track, Time, Note_off_c, Channel, Note, Velocity
            MidiNote no;
            int ticks = Convert.ToInt32(ar[1]);
            int notenumber = Convert.ToInt32(ar[4]);
            if (newNotes.Count > 0)
            {
                for (int i = 0; i < newNotes.Count; i++)
                {
                    no = newNotes[i];
                    if (no.Duration == 0 && no.Number == notenumber)
                    {
                        no.Duration = Convert.ToInt32(ar[1]) - n.StartTime;
                        track.addNote(no, false);
                        newNotes.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        #endregion


        #region lyrics management

        /// <summary>
        /// Manage lyrics Meta Lyric
        /// </summary>
        /// <param name="data"></param>
        private void manageMetaLyrics(byte[] data, int ticks)
        {
            string sy = string.Empty;

            switch (OpenMidiFileOptions.TextEncoding)
            {
                case "Ascii":
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;

                case "Chinese":
                    Encoding chinese = Encoding.GetEncoding("gb2312");
                    sy = chinese.GetString(data);
                    char[] c = sy.ToCharArray();
                    char x = c[1];

                    char y;
                    string cr = "\r";
                    for (int i = 0; i < c.Length; i++)
                    {
                        y = c[i];
                        if ((int)c[i] == 65292 || (int)c[i] == 12290)
                        {
                            c[i] = cr[0];
                        }
                    }
                    sy = new string(c);
                    break;

                case "Japanese":
                    Encoding japanese = Encoding.GetEncoding("932");
                    sy = japanese.GetString(data);
                    break;

                case "Korean":
                    Encoding korean = Encoding.GetEncoding("ks_c_5601-1987");
                    sy = korean.GetString(data);
                    break;

                default:
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;
            }

            try
            {
                if (sy != "")
                {
                    // Elimine caractères bizarres dans certains fichiers    
                    sy = cleanLyric(sy);
                    

                    string s = sy.Trim();
                    string reste = string.Empty;

                    // Commence par \r
                    if (sy.Replace("\r", "@").Trim() == "@")
                    {
                        track.TotalLyricsL += "\r";
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });
                    }
                    else if (sy.Substring(0, 1) == "\r" && sy.Length > 2)
                    {
                        reste = sy.Substring(1, sy.Length - 1);

                        track.TotalLyricsL += "\r";
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });

                        track.TotalLyricsL += reste;
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                    }
                    else if (sy.Length > 2 && sy.Substring(sy.Length - 2, 2) == "\r")
                    {
                        // Fini par \r
                        reste = sy.Substring(0, sy.Length - 2);

                        track.TotalLyricsL += reste;
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });

                        track.TotalLyricsL += "\r";
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });
                    }
                    else
                    {
                        if (s != "")
                        {
                            // Pas de retour chariot
                            track.TotalLyricsL += sy;
                            track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = sy, TicksOn = ticks });
                        }
                    }

                } // s != ""                 
            }
            catch (Exception ely)
            {
                Console.Write(ely.Message);
            }
        }

        /// <summary>
        /// Manage Lyrics Meta text
        /// </summary>
        /// <param name="data"></param>
        private void manageMetaText(byte[] data, int ticks)
        {
            // Lyric element:
            string sy = string.Empty;

            switch (OpenMidiFileOptions.TextEncoding)
            {
                case "Ascii":
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;

                case "Chinese":
                    Encoding chinese = Encoding.GetEncoding("gb2312");
                    sy = chinese.GetString(data);
                    char[] c = sy.ToCharArray();
                    char x = c[1];

                    char y;
                    string cr = "\r";
                    for (int i = 0; i < c.Length; i++)
                    {
                        y = c[i];
                        if ((int)c[i] == 65292 || (int)c[i] == 12290)
                        {
                            c[i] = cr[0];
                        }
                    }
                    sy = new string(c);
                    break;

                case "Japanese":
                    Encoding japanese = Encoding.GetEncoding("932");
                    sy = japanese.GetString(data);
                    break;

                case "Korean":
                    Encoding korean = Encoding.GetEncoding("ks_c_5601-1987");
                    sy = korean.GetString(data);
                    break;

                default:
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;
            }

            char[] arr;
            arr = sy.ToCharArray();
            int cv;
            for (int i = 0; i < arr.Length; i++)
            {
                char c = arr[i];
                cv = Convert.ToInt32(c);
                if (cv > 339)
                {

                    switch (cv)
                    {
                        case 352:
                            arr[i] = 'è';
                            break;
                        case 710:
                            arr[i] = 'ê';
                            break;
                        case 8218:
                            arr[i] = 'é';
                            break;
                        case 8225:
                            arr[i] = 'ç';
                            break;
                        case 8230:
                            arr[i] = 'à';
                            break;
                        default:
                            break;
                    }
                }
            }
            sy = new string(arr);

            try
            {
                if (sy != string.Empty)
                {
                    if (sy.Substring(0, 1) == "@" && ticks == 0)
                    {
                        // Old tags: text begining with @ character
                        MidiTags.Copyright += sy + "\r";

                        // New tags (non standard)
                        if (sy.Substring(0, 2) == "@#")
                            extractMidiTags(sy);
                        else
                        {
                            extractOldMidiTags(sy);
                        }
                    }
                    else if ((sy.Substring(0, 1) != "@") && ticks >= 0)
                    {
                        // Elimine caractères bizarres dans certains fichiers                    
                        sy = Regex.Replace(sy, "\0.$", "");
                        sy = sy.Replace("\0", " ");

                        // caractères non ascii ?

                        // Insere retours chariots
                        if ((sy.Substring(0, 1) == "/") || (sy.Substring(0, 1) == "\\"))
                        {
                            sy = sy.Replace("/", "\r");
                            sy = sy.Replace("\\", "\r");
                        }

                        string s = sy.Trim();
                        string reste = string.Empty;

                        // contient \r avec des espaces ou non
                        if (sy.Replace("\r", "@").Trim() == "@")
                        {
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });
                            track.TotalLyricsT += "\r";
                        }

                        else if (sy.Substring(0, 1) == "\r" && sy.Length > 2)
                        {
                            reste = sy.Substring(1, sy.Length - 1);

                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });
                            track.TotalLyricsT += "\r";

                            track.TotalLyricsT += reste;
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                        }
                        else if (sy.Length > 2 && sy.Substring(sy.Length - 2, 2) == "\r")
                        {
                            // Fini par \r
                            reste = sy.Substring(0, sy.Length - 2);

                            track.TotalLyricsT += reste;
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });

                            track.TotalLyricsT += "\r";
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "\r", TicksOn = ticks });
                        }
                        else
                        {
                            if (s != "")
                            {
                                // Pas de retour chariot
                                track.TotalLyricsT += sy;
                                track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = sy, TicksOn = ticks });
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                Console.Write(err.Message);
            }
        }

        private string cleanLyric(string l)
        {
            l = Regex.Replace(l, "\0.$", "");
            l = l.Replace("\0", " ");

            l = l.Replace("/", "\r");
            l = l.Replace("\\", "\r");

            l = l.Replace("\r\n", "\r");
            l = l.Replace("\n", "\r");

            return l;
        }


        private void extractOldMidiTags(string str)
        {
            /*
            Midi file tags
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?        
            */
            string id1 = "@K";
            string id2 = "@L";
            string id3 = "@W";
            string id4 = "@T";
            string id5 = "@I";
            string id6 = "@V";


            if (str.IndexOf(id1, 0) == 0)
            {
                MidiTags.KTag.Add(str.Substring(id1.Length));
                return;
            }
            if (str.IndexOf(id2, 0) == 0)
            {
                MidiTags.LTag.Add(str.Substring(id2.Length));
                return;
            }
            if (str.IndexOf(id3, 0) == 0)
            {
                MidiTags.WTag.Add(str.Substring(id3.Length));
                return;
            }
            if (str.IndexOf(id4, 0) == 0)
            {
                MidiTags.TTag.Add(str.Substring(id4.Length));
                return;
            }
            if (str.IndexOf(id5, 0) == 0)
            {
                MidiTags.ITag.Add(str.Substring(id5.Length));
                return;
            }
            if (str.IndexOf(id6, 0) == 0)
            {
                MidiTags.VTag.Add(str.Substring(id6.Length));
                return;
            }


        }

        /// <summary>
        /// Extract tags like mp3 (huhuhu)
        /// </summary>
        /// <param name="tx"></param>
        private void extractMidiTags(string str)
        {
            // Fabrice : pure invention, no standard seems to exist
            // @#Title   Song Title
            // @#Artist    Artist
            // @#Album   Album
            // @#Copyright   Copyright
            // @#Date   Date
            // @#Editor   Editor
            // @#Genre   Genre        
            // @#Evaluation   Evaluation

            string id1 = "@#Title=";
            string id2 = "@#Artist=";
            string id3 = "@#Album=";
            string id4 = "@#Copyright=";
            string id5 = "@#Date=";
            string id6 = "@#Editor=";
            string id7 = "@#Genre=";
            string id8 = "@#Evaluation=";
            string id9 = "@#Comment=";


            if (str.IndexOf(id1, 0) == 0)
            {
                MidiTags.TagTitle = str.Substring(id1.Length);
                return;
            }

            if (str.IndexOf(id2, 0) == 0)
            {
                MidiTags.TagArtist = str.Substring(id2.Length);
                return;
            }

            if (str.IndexOf(id3, 0) == 0)
            {
                MidiTags.TagAlbum = str.Substring(id3.Length);
                return;
            }

            if (str.IndexOf(id4, 0) == 0)
            {
                MidiTags.TagCopyright = str.Substring(id4.Length);
                return;
            }

            if (str.IndexOf(id5, 0) == 0)
            {
                MidiTags.TagDate = str.Substring(id5.Length);
                return;
            }

            if (str.IndexOf(id6, 0) == 0)
            {
                MidiTags.TagEditor = str.Substring(id6.Length);
                return;
            }

            if (str.IndexOf(id7, 0) == 0)
            {
                MidiTags.TagGenre = str.Substring(id7.Length);
                return;
            }

            if (str.IndexOf(id8, 0) == 0)
            {
                MidiTags.TagEvaluation = str.Substring(id8.Length);
                return;
            }

            if (str.IndexOf(id9, 0) == 0)
            {
                MidiTags.TagComment = str.Substring(id9.Length);
                return;
            }

        }

        #endregion


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
                Numerator = Numerator,
                Denominator = Denominator,
                Tempo = Tempo,
                Time = new TimeSignature(Numerator, Denominator, Division, Tempo),
            };

            // Tracks to sequence            
            sequence.tracks = newTracks;
            
            // Insert Tempo in track 0
            if (sequence.tracks.Count > 0)
                sequence.tracks[0].insertTempo(Tempo);

            // Tags to sequence
            sequence.CloneTags();


        }

        #endregion
    }
}
