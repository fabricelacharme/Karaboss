using Sanford.Multimedia.Midi;
using System;
using System.ComponentModel;
using System.IO;

namespace MusicTxt
{
    public class MusicTxtWriter
    {
        private BackgroundWorker writeTxtWorker = new BackgroundWorker();

        // txtmusic
        public event EventHandler<AsyncCompletedEventArgs> WriteTxtCompleted;
        public event ProgressChangedEventHandler WriteTxtProgressChanged;

        private bool disposed = false;

        // The Track to write to the Stream.
        private Track track = new Track();

        // The Stream to write to.
        private StreamWriter stream;

        // Running status.
        private int runningStatus = 0;

        private Sequence sequence;
        private string song;
        public string fileName {  get; private set; }

        public MusicTxtWriter(Sequence seq, string file)
        {
            InitializeBackgroundWorkers();
            sequence = seq;
            fileName = file;
            song = Path.GetFileNameWithoutExtension(file);
        }

        private void InitializeBackgroundWorkers()
        {
            // xmlmusic
            writeTxtWorker.DoWork += new DoWorkEventHandler(WriteTxtDoWork);
            writeTxtWorker.ProgressChanged += new ProgressChangedEventHandler(OnWriteTxtProgressChanged);
            writeTxtWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnWriteTxtCompleted);
            writeTxtWorker.WorkerReportsProgress = true;
        }

        bool bSilenceMode = false;

        public void WriteTxtAsync(string fileName, bool silenceMode = false)
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
            writeTxtWorker.RunWorkerAsync(fileName);
        }

        public bool IsTxtBusy
        {
            get
            {
                return writeTxtWorker.IsBusy;
            }
        }


        #region txtmusic
        private void OnWriteTxtCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WriteTxtCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnWriteTxtProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WriteTxtProgressChanged?.Invoke(this, e);
        }


        /// <summary>
        /// Dump by WriteTxtWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteTxtDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {
                FileStream fstream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter stream = new StreamWriter(fstream);

                using (stream)
                {
                    // Your stuff
                    Write(stream);
                }
            }
            catch (Exception ee)
            {
                Console.Write(ee.ToString());
                e.Cancel = true;

            }
        }

        #endregion txtmusic

        /// <summary>
        /// Dump sequence to text file
        /// </summary>
        /// <param name="strm"></param>
        public void Write(StreamWriter strm)
        {
            this.stream = strm;
            int trackid = 0;
            bool bHeaderCreated = false;

            if (sequence.tracks[0].ContainsNotes == false)
            {
                // first track has no notes
                // Create header
                //0, 0, Header, format, nTracks, division            
                stream.WriteLine(string.Format("0, 0, Header, {0}, {1}, {2}", sequence.Format, sequence.tracks.Count, sequence.Division));
            }
            else
            {
                // If first track contains notes, create an empty track with header information (so tracks.count + 1)

                //0, 0, Header, format, nTracks, division            
                stream.WriteLine(string.Format("0, 0, Header, {0}, {1}, {2}", sequence.Format, sequence.tracks.Count + 1, sequence.Division));

                trackid++;

                // First track is for informations            
                stream.WriteLine("1, 0, Start_track");

                // informations
                stream.WriteLine(string.Format("1, 0, Title_t, \"{0}\"", song));
                stream.WriteLine("1, 0, Text_t, \"Midi file dump made with Karaboss\"");
                stream.WriteLine("1, 0, Copyright_t, \"No copyright\"");

                // Track, Time, Time_signature, Num, Denom, Click, NotesQ
                // FAB: TO BE CHECKED
                stream.WriteLine(string.Format("1, 0, Time_signature, {0}, {1}, {2}, {3}", sequence.Time.Numerator, sequence.Time.Denominator, 24, 8));
                stream.WriteLine(string.Format("1, 0, Tempo, {0}", sequence.Tempo));
                stream.WriteLine("1, 0, End_track");

                bHeaderCreated = true;
            }



            for (int i = 0; i < sequence.tracks.Count; i++)
            {
                runningStatus = 0;
                track = sequence.tracks[i];

                trackid++;

                #region track informations
                // Start new track
                stream.WriteLine(string.Format("{0}, 0, Start_track", trackid));

                if (!bHeaderCreated)
                {
                    // first track has no notes
                    // header was not created
                    // informations
                    stream.WriteLine(string.Format("1, 0, Title_t, \"{0}\"", song));
                    stream.WriteLine("1, 0, Text_t, \"Midi file dump made with Karaboss\"");
                    stream.WriteLine("1, 0, Copyright_t, \"No copyright\"");

                    // Track, Time, Time_signature, Num, Denom, Click, NotesQ
                    // FAB: TO BE CHECKED
                    stream.WriteLine(string.Format("1, 0, Time_signature, {0}, {1}, {2}, {3}", sequence.Time.Numerator, sequence.Time.Denominator, 24, 8));
                    stream.WriteLine(string.Format("1, 0, Tempo, {0}", sequence.Tempo));
                    bHeaderCreated = true;
                }
                else
                {
                    // Track, Time, Title_t, Text
                    stream.WriteLine(string.Format("{0}, 0, Title_t, \"{1}\"", trackid, track.Name));

                    // Track, Time, Instrument_name_t, Text                
                    stream.WriteLine(string.Format("{0}, 0, Instrument_name_t, \"{1}\"", trackid, MidiFile.PCtoInstrument(track.ProgramChange)));

                    // Track, Time, Program_c, Channel, Program_num
                    stream.WriteLine(string.Format("{0}, 0, Program_c, {1}, {2}", trackid, track.MidiChannel, track.ProgramChange));

                }

                #endregion

                #region events
                foreach (MidiEvent e in track.Iterator())
                {
                    switch (e.MidiMessage.MessageType)
                    {
                        case MessageType.Channel:
                            Write((ChannelMessage)e.MidiMessage, trackid, e.AbsoluteTicks, track.MidiChannel);
                            break;

                        case MessageType.SystemExclusive:
                            Write((SysExMessage)e.MidiMessage);
                            break;

                        case MessageType.Meta:
                            Write((MetaMessage)e.MidiMessage, trackid, e.AbsoluteTicks);
                            break;

                        case MessageType.SystemCommon:
                            Write((SysCommonMessage)e.MidiMessage);
                            break;

                        case MessageType.SystemRealtime:
                            Write((SysRealtimeMessage)e.MidiMessage);
                            break;


                    }
                }
                #endregion

                //Track, Time, End_track
                stream.WriteLine(string.Format("{0}, 0, End_track", trackid));
            }

            // Last record
            // 0, 0, End_of_file
            stream.WriteLine("0, 0, End_of_file");

            // Close the stream
            stream.Close();
        }

        /// <summary>
        /// Write notes
        /// </summary>
        /// <param name="message"></param>
        /// <param name="trackid"></param>
        /// <param name="ticks"></param>
        /// <param name="channel"></param>
        private void Write(ChannelMessage message, int trackid, int ticks, int channel)
        {
            if (runningStatus != message.Status)
            {
                runningStatus = message.Status;
            }

            if (ChannelMessage.DataBytesPerType(message.Command) == 2)
            {
                if (message.Command == ChannelCommand.NoteOn)
                {
                    // Track, Time, Note_on_c, Channel, Note, Velocity                    
                    stream.WriteLine(string.Format("{0}, {1}, Note_on_c, {2}, {3}, {4}", trackid, ticks, channel, message.Data1, message.Data2));
                }
                else if (message.Command == ChannelCommand.NoteOff)
                {
                    // Track, Time, Note_off_c, Channel, Note, Velocity = 0
                    stream.WriteLine(string.Format("{0}, {1}, Note_off_c, {2}, {3}, 0", trackid, ticks, channel, message.Data1));
                }
                else if (message.Command == ChannelCommand.PitchWheel)
                {
                    // Track
                    //stream.WriteLine(string.Format("{0}, {1}, Pitch_bend_c, {2}, {3}", trackid, ticks, channel, message.Data1));
                    stream.WriteLine(string.Format("{0}, {1}, Pitch_bend_c, {2}, {3}", trackid, ticks, channel, message.Data2));

                }
                else if (message.Command == ChannelCommand.Controller)
                {
                    stream.WriteLine(string.Format("{0}, {1}, Control_c, {2}, {3}, {4}", trackid, ticks, channel, message.Data1, message.Data2));
                    // 7 Volume
                    // 10 Pan
                    // 11 Fader
                    // 91 Reverb
                    // 93 chorus                                                          
                }
            }
        }


        #region other events
        private void Write(SysExMessage message)
        {
            // System exclusive message cancel running status.
            runningStatus = 0;

            //trackData.Add((byte)message.Status);

            //WriteVariableLengthValue(message.Length - 1);

            for (int i = 1; i < message.Length; i++)
            {
                //trackData.Add(message[i]);
            }
        }

        private void Write(MetaMessage message, int trackid, int ticks)
        {
            string sy = string.Empty;
            //trackData.Add((byte)message.Status);
            //trackData.Add((byte)message.MetaType);

            //WriteVariableLengthValue(message.Length);

            //trackData.AddRange(message.GetBytes());

            switch (message.MetaType)
            {
                case MetaType.Copyright:
                    break;
                case MetaType.CuePoint:
                    break;
                case MetaType.DeviceName:
                    break;
                case MetaType.EndOfTrack:
                    break;
                case MetaType.InstrumentName:
                    break;
                case MetaType.KeySignature:
                    break;
                case MetaType.Lyric:
                    // Track, Time, Lyric_t, Text
                    byte[] b = message.GetBytes();
                    // Replace byte value 13 (cr) by antislash
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (b[i] == 13) b[i] = 92;
                    }

                    sy = System.Text.Encoding.Default.GetString(b);
                    // comma is the separator : remove all comma ?
                    //sy = sy.Replace(',', '-');
                    stream.WriteLine(string.Format("{0}, {1}, Lyric_t, \"{2}\"", trackid, ticks, sy));
                    break;
                case MetaType.Marker:
                    break;
                case MetaType.ProgramName:
                    break;
                case MetaType.ProprietaryEvent:
                    break;
                case MetaType.SequenceNumber:
                    break;
                case MetaType.SmpteOffset:
                    break;
                case MetaType.Tempo:
                    break;
                case MetaType.Text:
                    // Track, Time, Text_t, Text
                    //sy = System.Text.Encoding.Default.GetString(message.GetBytes()).Trim();
                    sy = System.Text.Encoding.Default.GetString(message.GetBytes());
                    sy = sy.Replace("\r\n", "\r");
                    sy = sy.Replace("\r", "\\");
                    sy = sy.Replace("\n", "\\");
                    // comma is the separator : remove all comma ?
                    //sy = sy.Replace(',', '-');
                    stream.WriteLine(string.Format("{0}, {1}, Text_t, \"{2}\"", trackid, ticks, sy));
                    break;
                case MetaType.TimeSignature:
                    break;
                case MetaType.TrackName:
                    break;
            }



        }

        private void Write(SysCommonMessage message)
        {
            // Escaped messages cancel running status.
            runningStatus = 0;

            // Escaped message.
            //trackData.Add((byte)0xF7);

            //trackData.Add((byte)message.Status);

            switch (message.SysCommonType)
            {
                case SysCommonType.MidiTimeCode:
                    //trackData.Add((byte)message.Data1);
                    break;

                case SysCommonType.SongPositionPointer:
                    //trackData.Add((byte)message.Data1);
                    //trackData.Add((byte)message.Data2);
                    break;

                case SysCommonType.SongSelect:
                    //trackData.Add((byte)message.Data1);
                    break;
            }
        }

        private void Write(SysRealtimeMessage message)
        {
            // Escaped messages cancel running status.
            runningStatus = 0;

            // Escaped message.
            //trackData.Add((byte)0xF7);
            //trackData.Add((byte)message.Status);
        }

        #endregion



    }

}

