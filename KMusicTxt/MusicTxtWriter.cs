#region License

/* Copyright (c) 2024 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion
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


        #region backgroundworker

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

        #endregion backgroundworker

        /// <summary>
        /// Dump sequence to text file
        /// </summary>
        /// <param name="strm"></param>
        public void Write(StreamWriter strm)
        {
            this.stream = strm;
            int trackid = 0;

            //0, 0, Header, format, nTracks, division            
            stream.WriteLine(string.Format("0, 0, Header, {0}, {1}, {2}", sequence.Format, sequence.tracks.Count, sequence.Division));

            for (int i = 0; i < sequence.tracks.Count; i++)
            {
                track = sequence.tracks[i];
                trackid = i;

                // Start new track
                stream.WriteLine(string.Format("{0}, 0, Start_track", trackid));                

                // Add sequence infos in first track
                if (i == 0)
                    AddSeqInfos();

                // Track, Time, Title_t, Text
                stream.WriteLine(string.Format("{0}, 0, Title_t, \"{1}\"", trackid, track.Name));

                // Track, Time, Instrument_name_t, Text                
                stream.WriteLine(string.Format("{0}, 0, Instrument_name_t, \"{1}\"", trackid, MidiFile.PCtoInstrument(track.ProgramChange)));
                

                #region events
                foreach (MidiEvent e in track.Iterator())
                {
                    switch (e.MidiMessage.MessageType)
                    {
                        case MessageType.Channel:                            
                            Write((ChannelMessage)e.MidiMessage, trackid, e.AbsoluteTicks);
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
                #endregion events

                
            }

            // Last record
            // 0, 0, End_of_file
            stream.WriteLine("0, 0, End_of_file");

            // Close the stream
            stream.Close();
        }
        
        /// <summary>
        /// Insert sequence information
        /// </summary>
        private void AddSeqInfos()
        {
            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FREN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */
            if (sequence.KTag == null || sequence.KTag.Count == 0)
                stream.WriteLine("0, 0, Text_t, \"@KMIDI KARAOKE FILE\"");
            if (sequence.VTag == null || sequence.VTag.Count == 0)
                stream.WriteLine("0, 0, Text_t, \"@V0100\"");
            if (sequence.TTag == null || sequence.TTag.Count == 0)
                stream.WriteLine(string.Format("0, 0, Text_t, \"@T{0}\"", song));

            if (sequence.ITag == null || sequence.ITag.Count == 0)
                stream.WriteLine("0, 0, Text_t, \"@IMidi file dump made with Karaboss\"");

            stream.WriteLine("0, 0, Copyright_t, \"No copyright\"");


            // TimeSignature can change during execution                        
            //stream.WriteLine(string.Format("0, 0, Time_signature, {0}, {1}, {2}, {3}", sequence.Time.Numerator, sequence.Time.Denominator, 24, 8));            
            
            // Tempo can change during execution
            //stream.WriteLine(string.Format("1, 0, Tempo, {0}", sequence.Tempo));
        }

    

        #region write events

        /// <summary>
        /// Write notes
        /// </summary>
        /// <param name="message"></param>
        /// <param name="trackid"></param>
        /// <param name="ticks"></param>
        /// <param name="channel"></param>
        private void Write(ChannelMessage message, int trackid, int ticks)
        {
            if (runningStatus != message.Status)
            {
                runningStatus = message.Status;
            }

            // 14/01/2025 :
            // Midi message can have another channel than the track's channel: track.channel replaces par message.MidiChannel
            

            if (ChannelMessage.DataBytesPerType(message.Command) == 2)
            {
                if (message.Command == ChannelCommand.NoteOn)
                {
                    // Track, Time, Note_on_c, Channel, Note, Velocity                    
                    stream.WriteLine(string.Format("{0}, {1}, Note_on_c, {2}, {3}, {4}", trackid, ticks, message.MidiChannel, message.Data1, message.Data2));
                }
                else if (message.Command == ChannelCommand.NoteOff)
                {
                    // Track, Time, Note_off_c, Channel, Note, Velocity = 0
                    stream.WriteLine(string.Format("{0}, {1}, Note_off_c, {2}, {3}, 0", trackid, ticks, message.MidiChannel, message.Data1));
                }
                else if (message.Command == ChannelCommand.PitchWheel)
                {
                    // Track                    
                    stream.WriteLine(string.Format("{0}, {1}, Pitch_bend_c, {2}, {3}", trackid, ticks, message.MidiChannel, message.Data2));

                }
                else if (message.Command == ChannelCommand.Controller)
                {
                    stream.WriteLine(string.Format("{0}, {1}, Control_c, {2}, {3}, {4}", trackid, ticks, message.MidiChannel, message.Data1, message.Data2));
                    // 7 Volume
                    // 10 Pan
                    // 11 Fader
                    // 91 Reverb
                    // 93 chorus                                                          
                }
                
            }
            else
            {
                if (message.Command == ChannelCommand.ProgramChange)
                {
                    // Change instrument
                    // Track, Time, Program_c, Channel, Program_num
                    stream.WriteLine(string.Format("{0}, {1}, Program_c, {2}, {3}", trackid, ticks, message.MidiChannel, message.Data1));


                }
            }
        }


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
            byte[] data;

            switch (message.MetaType)
            {
                case MetaType.Copyright:
                    break;
                case MetaType.CuePoint:
                    break;
                case MetaType.DeviceName:
                    break;
                
                case MetaType.EndOfTrack:                                       
                    stream.WriteLine(string.Format("{0}, {1}, End_track", trackid, ticks));
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
                    data = message.GetBytes();
                    int _tempo = ((data[0] << 16) | (data[1] << 8) | data[2]);
                    stream.WriteLine(string.Format("{0}, {1}, Tempo, {2}", trackid, ticks, _tempo));
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
                    // Track, Time, Time_signature, Num, Denom, Click, NotesQ
                    data = message.GetBytes();
                    if (data.Length > 3)
                    {
                        var Numerator = data[0];
                        var Denominator = (int)Math.Pow(2, data[1]); // denominator is a negative power of 2
                        var cc = data[2];
                        var dd = data[3];
                        stream.WriteLine(string.Format("{0}, {1}, Time_signature, {2}, {3}, {4}, {5}", trackid, ticks, Numerator, Denominator, cc, dd));
                    }
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

        #endregion write events



    }

}

