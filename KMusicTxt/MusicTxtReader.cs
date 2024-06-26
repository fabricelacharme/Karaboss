using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Remoting.Channels;

namespace MusicTxt
{
    public class MusicTxtReader
    {
        private BackgroundWorker loadTxtWorker = new BackgroundWorker();

        // txtmusic
        public event EventHandler<AsyncCompletedEventArgs> LoadTxtCompleted;
        public event ProgressChangedEventHandler LoadTxtProgressChanged;

        private bool disposed = false;

        public Sequence seq = new Sequence();

        private Track track = new Track();
        private List<Track> newTracks;
        private List<MidiNote> newNotes;

        private StreamReader stream;

        private Sequence sequence;
        private int Format = 1;
        private int Numerator = 4;
        private int Denominator = 4;
        private int Division = 480;
        private int Tempo = 24;


        private int currenttrack = -1;
        private int Channel = 0;
        private string TrackName = "Track1";
        private string InstrumentName = "AcousticGrandPiano";
        private int ProgramChange = 0;

        private int ControlChangeData1 = 0;
        private int ControlChangeData2 = 0;

        private int Volume = 0;
        private int Pan = 64;
        private int Reverb = 0;

        MidiNote n;

        public string fileName {  get; private set; }

        public MusicTxtReader(string file) 
        {
            fileName = file;
            InitializeBackgroundWorkers();
            MidiTags.ResetTags();
        }

        private void InitializeBackgroundWorkers()
        {
            // txtmusic
            loadTxtWorker.DoWork += new DoWorkEventHandler(LoadTxtDoWork);
            loadTxtWorker.ProgressChanged += new ProgressChangedEventHandler(OnLoadTxtProgressChanged);
            loadTxtWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnLoadTxtCompleted);
            loadTxtWorker.WorkerReportsProgress = true;
        }
        
        bool bSilenceMode = false;

        public void LoadTxtAsync(string fileName, bool silenceMode = false)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (IsTxtBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion
            bSilenceMode |= silenceMode;
            loadTxtWorker.RunWorkerAsync(fileName);
        }


        public bool IsTxtBusy
        {
            get
            {
                return loadTxtWorker.IsBusy;
            }
        }


        #region txtmusic
        private void OnLoadTxtCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadTxtCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnLoadTxtProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadTxtProgressChanged?.Invoke(this, e);
        }

        #endregion txtmusic


        /// <summary>
        /// Load text dump by LoadTxtWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTxtDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {                
                FileStream fstream = File.OpenRead(fileName); ;                
                StreamReader stream = new StreamReader(fstream);

                using (stream)
                {
                    // Your stuff
                    seq = Read(stream);
                }
            }
            catch (Exception ee)
            {
                Console.Write(ee.ToString());
                e.Cancel = true;

            }
        }

        /// <summary>
        /// Load sequence
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Sequence Read(string fileName)
        {
            try
            {                
                FileStream fstream = File.OpenRead(fileName);

                StreamReader stream = new StreamReader(fstream);
                using(stream)
                    return Read(stream);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Load sequence
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
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
                    array = line.Split(',').Select(p => p.Trim()).Select(p => p.Replace("\"", "")).ToArray();


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
                        if (currenttrack >= 0)
                            CreateTrack();
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
                    }
                    else if (array.Contains("Note_on_c"))
                    {
                        StartMidiNote(array);
                    }
                    else if (array.Contains("Note_off_c"))
                    {
                        StopMidiNote(array);
                    }
                    else if (array.Contains("Control_c"))
                    {
                        ReadControlChange(array);
                    }
                    else if (array.Contains("Pitch_bend_c"))
                    {
                        ReadPitchBend(array);
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


        #region Tracks

        /// <summary>
        /// Name of the track
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ReadTrackName(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("TrackName Length");
            // Track, Time, Title_t, Text            
            TrackName = ar[3];

            if (currenttrack >= 0 && currenttrack <= newTracks.Count)
            {
                newTracks[currenttrack].Name = TrackName;
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(TrackName);
                MetaMessage message = new MetaMessage(MetaType.TrackName, bytes);
                newTracks[currenttrack].Insert(0, message);
            }
        }

        /// <summary>
        /// Name of the instrument
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ReadInstrumentName(string[] ar)
        {
            if (ar.Length != 4)
                throw new ArgumentException("ProgramChange Length");

            // Track, Time, Instrument_name_t, Text
            InstrumentName = ar[3];
            if (currenttrack >= 0 && currenttrack <= newTracks.Count)
                newTracks[currenttrack].InstrumentName = InstrumentName;
        }

        /// <summary>
        /// Program change Program_c
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ReadProgramChange(string[] ar)
        {
            if (ar.Length != 5)
                throw new ArgumentException("ProgramChange Length");
            // Track, Time, Program_c, Channel, Program_num
            int ticks = Convert.ToInt32(ar[1]);
            Channel = Convert.ToInt32(ar[3]);
            ProgramChange = Convert.ToInt32(ar[4]);

            if (currenttrack >= 0 && currenttrack <= newTracks.Count)
            {
                if (newTracks[currenttrack].MidiChannel != Channel)
                {
                    newTracks[currenttrack].ChangeChannel(newTracks[currenttrack].MidiChannel, Channel);
                    newTracks[currenttrack].MidiChannel = Channel;
                }
                
                newTracks[currenttrack].ProgramChange = ProgramChange;
                ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, Channel, ProgramChange);
                newTracks[currenttrack].Insert(ticks, message);
            }
        }

        /// <summary>
        /// Control Change Control_c (volume, pan, fader, reverb, chorus)
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="ArgumentException"></exception>
        private void ReadControlChange(string[] ar)
        {
            if (ar.Length != 6)
                throw new ArgumentException("ControlChange Length");

            if (currenttrack < 0 || currenttrack > newTracks.Count)
                return;

            // Track, Time, Control_c, Channel, Data1, Data2
            int ticks = Convert.ToInt32(ar[1]);
            Channel = Convert.ToInt32(ar[3]);
            ControlChangeData1 = Convert.ToInt32(ar[4]);
            ControlChangeData2 = Convert.ToInt32(ar[5]);

            // 7 Volume
            // 10 Pan
            // 11 Fader
            // 91 Reverb
            // 93 chorus   

            switch (ControlChangeData1)
            {
                case 7:
                    Volume = ControlChangeData2;
                    newTracks[currenttrack].Volume = Volume;
                    newTracks[currenttrack].insertVolume(Channel,Volume);
                    break;
                case 10:
                    Pan = ControlChangeData2;
                    newTracks[currenttrack].Pan = Pan;
                    newTracks[currenttrack].insertPan(Channel,Pan);
                    break;                
                case 91:
                    Reverb = ControlChangeData2;
                    newTracks[currenttrack].Reverb = Reverb;
                    newTracks[currenttrack].insertReverb(Channel,Reverb);
                    break;
                default:
                    break;
            }

            // Change channel
            if (newTracks[currenttrack].MidiChannel != Channel)
            {                
                newTracks[currenttrack].ChangeChannel(newTracks[currenttrack].MidiChannel, Channel);
                newTracks[currenttrack].MidiChannel = Channel;
            }

            // Insert all messages
            ChannelMessage message = new ChannelMessage(ChannelCommand.Controller, Channel, ControlChangeData1, ControlChangeData2);
            newTracks[currenttrack].Insert(ticks, message);

        }

        private void ReadPitchBend(string[] ar)
        {
            // Track, Time, Pitch_bend_c, Channel, Data2
            if (ar.Length != 5)
                throw new ArgumentException("PitchBend Length");

            int ticks = Convert.ToInt32(ar[1]);
            int PitchBend = Convert.ToInt32(ar[4]);            
            Channel = Convert.ToInt32(ar[3]);

            ChannelMessageBuilder builder = new ChannelMessageBuilder();
            ChannelMessage pitchBendMessage;
            int mask = 127;

            // Build pitch bend message;
            builder.Command = ChannelCommand.PitchWheel;
            builder.MidiChannel = Channel;

            builder.Data1 = 0;
            builder.Data2 = PitchBend;

            // Build message.
            builder.Build();
            pitchBendMessage = builder.Result;

            //ChannelMessage message = new ChannelMessage(ChannelCommand.PitchWheel, Channel, PitchBend);
            
            if (currenttrack >= 0 && currenttrack <= newTracks.Count)
                newTracks[currenttrack].Insert(ticks, pitchBendMessage);
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
            ResetValues();

            track = new Track()
            {
                MidiChannel = Channel,
                Name = TrackName,
                InstrumentName = InstrumentName,
                ProgramChange = ProgramChange,
                Volume = Volume,
                Pan = Pan,
                Reverb = Reverb,
                Denominator = Denominator,
                Numerator = Numerator
            };

            ChannelMessage message = new ChannelMessage(ChannelCommand.ProgramChange, track.MidiChannel, track.ProgramChange, 0);
            track.Insert(0, message);
            track.insertTimesignature(Numerator, Denominator);
            newTracks.Add(track);

            
        }
        
        private void ResetValues()
        {
            Pan = 64;
            Reverb = 0;
            TrackName = "Track1";
            InstrumentName = "AcousticGrandPiano";

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

            #region text encoding
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
                    Encoding japanese = Encoding.GetEncoding("shift_jis");
                    sy = japanese.GetString(data);
                    break;

                case "Korean":
                    Encoding korean = Encoding.GetEncoding("ks_c_5601-1987");
                    sy = korean.GetString(data);
                    break;

                case "Vietnamese":
                    Encoding vietnamese = Encoding.GetEncoding("windows-1258");
                    sy = vietnamese.GetString(data);
                    break;

                default:
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;
            }
            #endregion

            // Clean special characters
            if (sy.Trim() != string.Empty)
                sy = CleanSpecialChars(sy);

            try
            {
                if (sy != string.Empty)
                {
                    // Elimine caractères bizarres dans certains fichiers    
                    sy = cleanLyric(sy);

                    string s = sy.Trim();
                    string reste = string.Empty;

                    #region extract data

                    string Paragraph1 = "\r\r";
                    int iParagraph1 = sy.LastIndexOf(Paragraph1);
                    string Paragraph2 = "\\";
                    int iParagraph2 = sy.LastIndexOf(Paragraph2);

                    string LineFeed1 = "\r";
                    int iLineFeed1 = sy.LastIndexOf(LineFeed1);
                    string LineFeed2 = "/";
                    int iLineFeed2 = sy.LastIndexOf(LineFeed2);

                    // FAB 12/02/2022
                    // the human imagination is far beyond what an unfortunate computer program can predict
                    // Sometime linefeed is not at the beginning or end of string, but inside....
                    // Something like " /blabla"
                    if (iLineFeed2 > 0)
                    {
                        sy = sy.Remove(iLineFeed2, 1);
                        sy = LineFeed2 + sy;
                        iLineFeed2 = 0;
                    }

                    if (iParagraph1 == 0 || (sy.Length > Paragraph1.Length && iParagraph1 == sy.Length - Paragraph1.Length))
                    {
                        // single paragraph
                        // A forward slash "/" character marks the end of a "paragraph" of lyrics
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Paragraph, Element = "½", TicksOn = ticks });
                        track.TotalLyricsL += "½";

                        if (sy.Length > Paragraph1.Length)
                        {
                            // Text
                            if (iParagraph1 == 0)
                                reste = sy.Substring(Paragraph1.Length, sy.Length - Paragraph1.Length);
                            else
                                reste = sy.Substring(0, iParagraph1);

                            track.TotalLyricsL += reste;
                            track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                        }
                    }
                    else if (iParagraph2 == 0 || (sy.Length > Paragraph2.Length && iParagraph2 == sy.Length - Paragraph2.Length))
                    {
                        // single paragraph
                        // A forward slash "/" character marks the end of a "paragraph" of lyrics
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Paragraph, Element = "½", TicksOn = ticks });
                        track.TotalLyricsL += "½";

                        if (sy.Length > Paragraph2.Length)
                        {
                            // Text
                            if (iParagraph2 == 0)
                                reste = sy.Substring(Paragraph2.Length, sy.Length - Paragraph2.Length);
                            else
                                reste = sy.Substring(0, iParagraph2);

                            track.TotalLyricsL += reste;
                            track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                        }
                    }
                    else if (iLineFeed1 == 0 || (sy.Length > LineFeed1.Length && iLineFeed1 == sy.Length - LineFeed1.Length))
                    {
                        // Single linefeed
                        // A back slash "\" character marks the end of a line of lyrics
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "¼", TicksOn = ticks });
                        track.TotalLyricsL += "¼";

                        if (sy.Length > LineFeed1.Length)
                        {
                            // Text
                            if (iLineFeed1 == 0)
                                reste = sy.Substring(LineFeed1.Length, sy.Length - LineFeed1.Length);
                            else
                                reste = sy.Substring(0, iLineFeed1);

                            track.TotalLyricsL += reste;
                            track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                        }
                    }
                    else if (iLineFeed2 == 0 || (sy.Length > LineFeed2.Length && iLineFeed2 == sy.Length - LineFeed2.Length))
                    {
                        // Single linefeed
                        // A back slash "\" character marks the end of a line of lyrics
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "¼", TicksOn = ticks });
                        track.TotalLyricsL += "¼";

                        if (sy.Length > LineFeed2.Length)
                        {
                            // Text
                            if (iLineFeed2 == 0)
                                reste = sy.Substring(LineFeed2.Length, sy.Length - LineFeed2.Length);
                            else
                                reste = sy.Substring(0, iLineFeed2);

                            track.TotalLyricsL += reste;
                            track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                        }
                    }
                    else if (s != "")
                    {
                        // no linefeed
                        track.TotalLyricsL += sy;
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = sy, TicksOn = ticks });
                    }
                    else if (sy == " ")
                    {
                        // Manage the space separator when lyrics are letter to letter
                        track.TotalLyricsL += "[]";
                        track.Lyrics.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = "[]", TicksOn = ticks });

                    }
                    #endregion                  

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

            #region text encoding
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
                    Encoding japanese = Encoding.GetEncoding("shift_jis");
                    sy = japanese.GetString(data);
                    break;

                case "Korean":
                    Encoding korean = Encoding.GetEncoding("ks_c_5601-1987");
                    sy = korean.GetString(data);
                    break;

                case "Vietnamese":
                    Encoding vietnamese = Encoding.GetEncoding("windows-1258");
                    sy = vietnamese.GetString(data);
                    break;

                default:
                    sy = System.Text.Encoding.Default.GetString(data);
                    break;
            }
            #endregion

            // Clean special characters
            if (sy.Trim() != string.Empty)
                sy = CleanSpecialChars(sy);

            try
            {
                if (sy != string.Empty)
                {

                    // Elimine caractères bizarres dans certains fichiers    
                    sy = cleanLyric(sy);

                    // Tags
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

                        string s = sy.Trim();
                        string reste = string.Empty;

                        #region extract data
                        string Paragraph1 = "\r\r";
                        int iParagraph1 = sy.LastIndexOf(Paragraph1);
                        string Paragraph2 = "\\";
                        int iParagraph2 = sy.LastIndexOf(Paragraph2);

                        string LineFeed1 = "\r";
                        int iLineFeed1 = sy.LastIndexOf(LineFeed1);
                        string LineFeed2 = "/";
                        int iLineFeed2 = sy.LastIndexOf(LineFeed2);



                        if (iParagraph1 == 0 || (sy.Length > Paragraph1.Length && iParagraph1 == sy.Length - Paragraph1.Length))
                        {
                            // single paragraph
                            // A double forward slash "\\" character marks the end of a "paragraph" of lyrics
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Paragraph, Element = "½", TicksOn = ticks });
                            track.TotalLyricsT += "½";

                            if (sy.Length > Paragraph1.Length)
                            {
                                // Text
                                if (iParagraph1 == 0)
                                    reste = sy.Substring(1, sy.Length - Paragraph1.Length);
                                else
                                    reste = sy.Substring(0, iParagraph1);

                                track.TotalLyricsT += reste;
                                track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                            }
                        }
                        else if (iParagraph2 == 0 || (sy.Length > Paragraph2.Length && iParagraph2 == sy.Length - Paragraph2.Length))
                        {
                            // single paragraph
                            // A double forward slash "\\" character marks the end of a "paragraph" of lyrics
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Paragraph, Element = "½", TicksOn = ticks });
                            track.TotalLyricsT += "½";

                            if (sy.Length > Paragraph2.Length)
                            {
                                // Text
                                if (iParagraph2 == 0)
                                    reste = sy.Substring(1, sy.Length - Paragraph2.Length);
                                else
                                    reste = sy.Substring(0, iParagraph2);

                                track.TotalLyricsT += reste;
                                track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                            }
                        }
                        else if (iLineFeed1 == 0 || (sy.Length > LineFeed1.Length && iLineFeed1 == sy.Length - LineFeed1.Length))
                        {
                            // Single linefeed
                            // A back slash "/" character marks the end of a line of lyrics
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "¼", TicksOn = ticks });
                            track.TotalLyricsT += "¼";

                            if (sy.Length > LineFeed1.Length)
                            {
                                // Linefeed at the begining
                                if (iLineFeed1 == 0)
                                    reste = sy.Substring(1, sy.Length - LineFeed1.Length);
                                else
                                    reste = sy.Substring(0, iLineFeed1);

                                track.TotalLyricsT += reste;
                                track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                            }
                        }
                        else if (iLineFeed2 == 0 || (sy.Length > LineFeed2.Length && iLineFeed2 == sy.Length - LineFeed2.Length))
                        {
                            // Single linefeed
                            // A back slash "/" character marks the end of a line of lyrics
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.LineFeed, Element = "¼", TicksOn = ticks });
                            track.TotalLyricsT += "¼";

                            if (sy.Length > LineFeed2.Length)
                            {
                                // Linefeed at the begining
                                if (iLineFeed2 == 0)
                                    reste = sy.Substring(1, sy.Length - LineFeed2.Length);
                                else
                                    reste = sy.Substring(0, iLineFeed2);

                                track.TotalLyricsT += reste;
                                track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = reste, TicksOn = ticks });
                            }
                        }
                        else if (s != "")
                        {
                            // no linefeed
                            track.TotalLyricsT += sy;
                            track.LyricsText.Add(new Track.Lyric() { Type = Track.Lyric.Types.Text, Element = sy, TicksOn = ticks });
                        }

                        #endregion

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

        private string CleanSpecialChars(string sy)
        {
            //byte[] l = Encoding.Default.GetBytes(sy);
            //sy = Encoding.UTF8.GetString(l);

            // FAB 12/04/2019 - UTF8 encoding
            sy = sy.Replace("Ã©", "é");
            sy = sy.Replace("Ã¨", "è");
            sy = sy.Replace("â€™", "’");

            sy = sy.Replace("Ã§", "ç");
            sy = sy.Replace("Ã‡", "Ç");

            sy = sy.Replace("Ãª", "ê");
            sy = sy.Replace("Ã¢", "â");
            sy = sy.Replace("Ã´", "ô");
            sy = sy.Replace("Ã ", "à");
            sy = sy.Replace("Ã¦", "æ");
            sy = sy.Replace("Å“", "œ");
            sy = sy.Replace("Ã®", "î");
            sy = sy.Replace("à®", "î");

            sy = sy.Replace("Ã»", "û");
            sy = sy.Replace("Ãº", "ú");
            sy = sy.Replace("Ã¹", "ù");

            sy = sy.Replace("â€”", "—");


            /*
            var dictionary = new Dictionary<string, string>()
            {
                {"Ã¡", "á"},
                {"Ã€", "À"},
                {"Ã¤", "ä"},
                {"Ã„", "Ä"},
                {"Ã£", "ã"},
                {"Ã¥", "å"},
                {"Ã…", "Å"},
                {"Ã¦", "æ"},
                {"Ã†", "Æ"},
                {"Ã§", "ç"},
                {"Ã‡", "Ç"},
                {"Ã©", "é"},
                {"Ã‰", "É"},
                {"Ã¨", "è"},
                {"Ãˆ", "È"},
                {"Ãª", "ê"},
                {"ÃŠ", "Ê"},                
                {"Ã«", "ë"},
                {"Ã‹", "Ë"},
                {"Ã-­­", "í"},
                {"Ã", "Í"},
                {"Ã¬", "ì"},
                {"ÃŒ", "Ì"},                
                {"Ã®", "î"},
                {"ÃŽ", "Î"},                
                {"Ã¯", "ï"},                                
                {"Ã±", "ñ"},
                {"Ã‘", "Ñ"},                
                {"Ã³", "ó"},
                {"Ã“", "Ó"},
                {"Ã²", "ò"},
                {"Ã’", "Ò"},
                {"Ã´", "ô"},
                {"Ã”", "Ô"},
                {"Ã¶", "ö"},
                {"Ã–", "Ö"},
                {"Ãµ", "õ"},
                {"Ã•", "Õ"},
                {"Ã¸", "ø"},
                {"Ã˜", "Ø"},
                {"Å“", "œ"},
                {"Å’", "Œ"},
                {"ÃŸ", "ß"},
                {"Ãº", "ú"},
                {"Ãš", "Ú"},
                {"Ã¹", "ù"},
                {"Ã™", "Ù"},
                {"Ã»", "û"},
                {"Ã›", "Û"},
                {"Ã¼", "ü"},
                {"Ãœ", "Ü"},                
                {"â‚¬", "€"},
                {"â€™", "’"},
                {"â€š", "‚"},
                {"Æ’", "ƒ"},
                {"â€ž", "„"},
                {"â€¦", "…"},
                {"â€¡", "‡"},                
                {"Ë†", "ˆ"},
                {"â€°", "‰"},
                {"Å ", "Š"},
                {"â€¹", "‹"},
                {"Å½", "Ž"},
                {"â€˜", "‘"},
                {"â€œ", "“"},
                {"â€¢", "•"},
                {"â€“", "–"},
                {"â€”", "—"},
                {"Ëœ", "˜"},                
                {"â„¢", "™"},
                {"Å¡", "š"},
                {"â€º", "›"},
                {"Å¾", "ž"},
                {"Å¸", "Ÿ"},
                {"Â¡", "¡"},
                {"Â¢", "¢"},
                {"Â£", "£"},
                {"Â¤", "¤"},
                {"Â¥", "¥"},
                {"Â¦", "¦"},                
                {"Â§", "§"},
                {"Â¨", "¨"},
                {"Â©", "©"},
                {"Âª", "ª"},
                {"Â«", "«"},
                {"Â¬", "¬"},
                {"Â®", "®"},
                {"Â¯", "¯"},
                {"Â°", "°"},
                {"Â±", "±"},
                {"Â²", "²"},                
                {"Â³", "³"},
                {"Â´", "´"},
                {"Âµ", "µ"},
                {"Â¶", "¶"},
                {"Â·", "·"},
                {"Â¸", "¸"},                
                {"Â¹", "¹"},
                {"Âº", "º"},
                {"Â»", "»"},
                {"Â¼", "¼"},
                {"Â½", "½"},
                {"Â¾", "¾"},                
                {"Â¿", "¿"},                
                {"â€", "†"},                                
                {"Ã¢", "â"},
                {"Ã‚", "Â"},
                {"Ãƒ", "Ã"},                
            };

            if (dictionary.ContainsKey(sy))
            {
                sy = dictionary[sy];
            }
            */

            char[] arr;
            arr = sy.ToCharArray();
            int cv;
            for (int i = 0; i < arr.Length; i++)
            {
                char c = arr[i];
                cv = Convert.ToInt32(c);
                if (cv > 217)
                {
                    switch (cv)
                    {
                        case 218:
                            arr[i] = 'é';
                            break;
                        case 219:
                            arr[i] = 'ê';
                            break;
                        case 250:
                            arr[i] = 'œ';
                            break;
                        case 352:
                            arr[i] = 'è';
                            break;
                        case 402:
                            arr[i] = 'â';
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

            return new string(arr);
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
                sequence.tracks[0].insertTempo(Tempo, 0);

            // Tags to sequence
            sequence.CloneTags();


        }

        #endregion


    }
}
