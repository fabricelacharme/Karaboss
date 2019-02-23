#region License

/* Copyright (c) 2005 Leslie Sanford
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

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic; //List
using System.Text;
using System.Globalization;

namespace Sanford.Multimedia.Midi
{
	/// <summary>
	/// Reads a track from a stream.
	/// </summary>
	internal class TrackReader
	{
        private Track track = new Track();

        private Track newTrack = new Track();

        private ChannelMessageBuilder cmBuilder = new ChannelMessageBuilder();

        private SysCommonMessageBuilder scBuilder = new SysCommonMessageBuilder();

        private Stream stream;

        private byte[] trackData;

        private int trackIndex;

        private int previousTicks;

        private int ticks;

        private int status;

        private int runningStatus;


		public TrackReader()
		{
		}

        public void Read(Stream strm)
        { 
            stream = strm;
            
            // FAB : 27/07/2014
            if (FindTrack() != -1)
            {
                int trackLength = GetTrackLength();
                trackData = new byte[trackLength];

                int result = strm.Read(trackData, 0, trackLength);

                if (result < 0)
                {
                    throw new MidiFileException("End of MIDI file unexpectedly reached.");
                }

                newTrack = new Track();

                ParseTrackData();

                track = newTrack;
                track.checkNotes();
            }
            else
            {
                track = null;
            }
        }

        private int FindTrack()
        {
            bool found = false;
            int result;
            
            while(!found)
            {
                result = stream.ReadByte();

                if(result == 'M')
                {
                    result = stream.ReadByte();

                    if(result == 'T')
                    {
                        result = stream.ReadByte();

                        if(result == 'r')
                        {
                            result = stream.ReadByte();

                            if(result == 'k')
                            {
                                found = true;
                            }
                        }
                    }
                }

                if(result < 0)
                {
                    // FAB
                    //throw new MidiFileException("Unable to find track in MIDI file.");
                    Console.Write("\nERROR: Unable to find track in MIDI file (TrackReader.cs FindTrack)");
                    return -1;
                }                
            }            
            return 0;
        }

        private int GetTrackLength()
        {
            byte[] trackLength = new byte[4];

            int result = stream.Read(trackLength, 0, trackLength.Length);
            
            if(result < trackLength.Length)
            {
                throw new MidiFileException("\nERROR: End of MIDI file unexpectedly reached (TrackReader.cs)");
            }

            if(BitConverter.IsLittleEndian)         /// très intéressant, à méditer
            {
                Array.Reverse(trackLength);
            }

            return BitConverter.ToInt32(trackLength, 0);
        }

        private void ParseTrackData()
        {
            trackIndex = ticks = runningStatus = 0;

            while(trackIndex < trackData.Length)
            {
                previousTicks = ticks;

                ticks += ReadVariableLengthValue();

                // Fab trackindex increased too much
                if (trackIndex >= trackData.Length)
                    break;

                if ((trackData[trackIndex] & 0x80) == 0x80)
                {
                    status = trackData[trackIndex];
                    trackIndex++;
                }
                else
                {
                    status = runningStatus;
                }                
                ParseMessage();                
            }
        }

        private void ParseMessage()
        {
            // If this is a channel message.
            if(status >= (int)ChannelCommand.NoteOff && 
                status <= (int)ChannelCommand.PitchWheel + 
                ChannelMessage.MidiChannelMaxValue)
            {
                ParseChannelMessage();
            }           
            // Else if this is a meta message.
            else if (status == 0xFF)
            {
                ParseMetaMessage();
            }
            // Else if this is the start of a system exclusive message.
            else if (status == (int)SysExType.Start)
            {
                ParseSysExMessageStart();
            }
            // Else if this is a continuation of a system exclusive message.
            else if (status == (int)SysExType.Continuation)
            {
                ParseSysExMessageContinue();
            }
            // Else if this is a system common message.
            else if (status >= (int)SysCommonType.MidiTimeCode &&
                status <= (int)SysCommonType.TuneRequest)
            {
                ParseSysCommonMessage();
            }
            // Else if this is a system realtime message.
            else if (status >= (int)SysRealtimeType.Clock &&
                status <= (int)SysRealtimeType.Reset)
            {
                ParseSysRealtimeMessage();
            }
        }

        private void ParseChannelMessage()
        {
            if(trackIndex >= trackData.Length)
            {
                //throw new MidiFileException("End of track unexpectedly reached.");
                Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseChannelMessage)");
                return;
            }

            cmBuilder.Command = ChannelMessage.UnpackCommand(status);
            cmBuilder.MidiChannel = ChannelMessage.UnpackMidiChannel(status);
            cmBuilder.Data1 = trackData[trackIndex];

            // PROGRAM CHANGE
            if (cmBuilder.Command == ChannelCommand.ProgramChange)
            {
                newTrack.ProgramChange = cmBuilder.Data1;
                newTrack.MidiChannel = cmBuilder.MidiChannel;
            }
  
            trackIndex++;

            if(ChannelMessage.DataBytesPerType(cmBuilder.Command) == 2)
            {
                if(trackIndex >= trackData.Length)
                {
                    //throw new MidiFileException("End of track unexpectedly reached.");
                    Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseChannelMessage)");
                    return;
                }                                     
                
                // FAB : 07/08/2014
                if (trackData[trackIndex] <= 127)
                    cmBuilder.Data2 = trackData[trackIndex];
                else
                    cmBuilder.Data2 = 127;
                
                if (cmBuilder.Data1 == 0x07)
                {
                    // Volume de la piste
                    newTrack.Volume = cmBuilder.Data2;
                }
                else if (cmBuilder.Data1 == 0x5B)
                {
                    // Reverb 91 
                    // FAB 2017
                    newTrack.Reverb = cmBuilder.Data2;
                }
                else if (cmBuilder.Data1 == 0x0A)
                {
                    // pan 10
                    // FAB 2017
                    newTrack.Pan = cmBuilder.Data2;
                }
           
                // Collecte des notes
                if (cmBuilder.Command == ChannelCommand.NoteOn)
                {
                    newTrack.ContainsNotes = true;
                    newTrack.Visible = true;

                    // Data1 = Note number                    
                    // Data2 = Velocity
                    if (ticks >= 0 && cmBuilder.Data2 > 0)
                    {                       
                        // FAB : 
                        // Add a MidiNote to this track.  This is called for each NoteOn event */
                        newTrack.MidiChannel = cmBuilder.MidiChannel;

                        MidiNote note = new MidiNote(ticks, newTrack.MidiChannel, cmBuilder.Data1, 0, cmBuilder.Data2, false);
                        newTrack.Notes.Add(note);                                                
                    }
                    else
                    {
                        // FAB
                        if (newTrack.Notes.Count > 0)
                        {
                            newTrack.MidiChannel = cmBuilder.MidiChannel;
                            NoteOff(newTrack.MidiChannel, cmBuilder.Data1, ticks);                          
                        }
                    }
                }
                else if (ticks >= 0 && cmBuilder.Command == ChannelCommand.NoteOff)
                {                    
                    // FAB
                    newTrack.ContainsNotes = true;
                    newTrack.Visible = true;

                    newTrack.MidiChannel = cmBuilder.MidiChannel;
                    NoteOff(newTrack.MidiChannel, cmBuilder.Data1, ticks);                   
                }                
                trackIndex++;                        
            }  
            cmBuilder.Build();
            newTrack.Insert(ticks, cmBuilder.Result);

            runningStatus = status;
        }


        /** A NoteOff event occured.  Find the MidiNote of the corresponding
         * NoteOn event, and update the duration of the MidiNote.
         */
        public void NoteOff(int channel, int notenumber, int endtime)
        {
            for (int i = newTrack.Notes.Count - 1; i >= 0; i--)
            {
                MidiNote note = newTrack.Notes[i];
                if (note.Channel == channel && note.Number == notenumber && note.Duration == 0)
                {
                    note.NoteOff(endtime);
                    return;
                }
            }
        }        
        
        private void ParseMetaMessage()
        {
            if(trackIndex >= trackData.Length)
            {
                //throw new MidiFileException("End of track unexpectedly reached.");
                Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseMetaMessage)");
                return;
            }

            MetaType type = (MetaType)trackData[trackIndex];

            trackIndex++;

            if(trackIndex >= trackData.Length)
            {
                //throw new MidiFileException("End of track unexpectedly reached.");
                Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseMetaMessage)");
                return;
            }

            if (type == MetaType.EndOfTrack)
            {
                newTrack.EndOfTrackOffset = ticks - previousTicks;

                trackIndex++;
            }
            else if (type == MetaType.TrackName)
            {
                #region TRACK NAME
                byte[] data = new byte[ReadVariableLengthValue()];
                Array.Copy(trackData, trackIndex, data, 0, data.Length);
                newTrack.Insert(ticks, new MetaMessage(type, data));

                // TODO check if it is ASCCI
                newTrack.Name = System.Text.ASCIIEncoding.ASCII.GetString(data);
                                
                trackIndex += data.Length;
                #endregion
            }
            else if (type == MetaType.InstrumentName)
            {
                #region INSTRUMENT NAME 
                byte[] data = new byte[ReadVariableLengthValue()];
                Array.Copy(trackData, trackIndex, data, 0, data.Length);
                newTrack.Insert(ticks, new MetaMessage(type, data));

                newTrack.InstrumentName = System.Text.ASCIIEncoding.ASCII.GetString(data);
                trackIndex += data.Length;
                #endregion
            }
            else if (type == MetaType.Lyric)
            {
                #region karaoke pure lyrics

                // KAROKE LYRICS
                byte[] data = new byte[ReadVariableLengthValue()];

                try
                {
                    Array.Copy(trackData, trackIndex, data, 0, data.Length);
                    newTrack.Insert(ticks, new MetaMessage(type, data));
                 
                    manageMetaLyrics(data, ticks);                               
                }
                catch (Exception ex)
                {
                    Console.Write("ERROR: lyrics - " + ex.Message);
                }
                trackIndex += data.Length;

                #endregion karaoke pure lyrics

            }
            else if (type == MetaType.Tempo)
            {
                #region tempo

                // FAB recherche tempo
                byte[] data = new byte[ReadVariableLengthValue()];
                try
                {                    
                    Array.Copy(trackData, trackIndex, data, 0, data.Length);
                    newTrack.Insert(ticks, new MetaMessage(type, data));

                    // tempo = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    int Tempo = ((data[0] << 16) | (data[1] << 8) | data[2]);

                    newTrack.Tempo = Tempo;
                }
                catch (Exception ex)
                {
                    Console.Write("\nERROR: TrackReader.cs - Tempo Array.Copy - " + ex.Message);
                }
                trackIndex += data.Length;

                #endregion tempo

            }            
            else if (type == MetaType.Text)
            {                
                // KARAOKE TEXT
                byte[] data = new byte[ReadVariableLengthValue()];

                #region karaoke text lyrics
                try
                {
                    Array.Copy(trackData, trackIndex, data, 0, data.Length);
                    newTrack.Insert(ticks, new MetaMessage(type, data));                    

                    manageMetaText(data, ticks);                                       
                }
                catch (Exception ex)
                {
                    Console.Write("ERROR: lyrics - " + ex.Message);
                }
                #endregion karaoke text lyrics

                trackIndex += data.Length;
            }
            else if (type == MetaType.TimeSignature)
            {
                #region timesignature
                // TIME SIGNATURE FF 58 nn dd cc bb

                ///[nn] Numerator
                ///The numerator represents the numerator of the time signature that you would find on traditional sheet music. 
                ///The numerator counts the number of beats in a measure. For example a numerator of 4 means that each bar contains four beats.
                ///This is important to know because usually the first beat of each bar has extra emphasis.

                ///[dd] Denominator
                ///The denominator represents the denominator of the time signature that you would find on traditional sheet music.
                ///The denominator specifies the number of quarter notes in a beat. 
                ///A time signature of 4,4 means: 4 beats in the bar and each beat is a quarter note (i.e. a crotchet). 
                ///In MIDI the denominator value is stored in a special format. i.e. the real denominator = 2^[dd].

                ///[cc] MIDI ticks per metronome click
                ///The standard MIDI clock ticks every 24 times every quarter note (crotchet) so a [cc] value of 24 would mean that the metronome clicks once every quarter note. 
                ///A [cc] value of 6 would mean that the metronome clicks once every 1/8th of a note (quaver). 
                ///Be warned, this midi clock is different from the clock who's pulses determine the start time and duration of the notes (see PPQN below). 
                ///This MIDI clock ticks 24 times a second and seems to be used only to specify the rate of the metronome - 
                ///which I can only assume is a real metronome i.e. a device which makes a tick noise at a steady rate... tick, tick tick...

                ///[bb] 32nd notes per MIDI quarter note
                ///This value specifies the number of 1/32nds of a note happen every MIDI quarter note. 
                ///It is usually 8 which means that a quarter note happens every quarter note - 
                ///which is logical. By choosing different values it's possible to vary the rate of the music artificially. 
                ///By putting a value of 16 it means that the music plays two quarter notes for each quarter note metered out by the midi clock. 
                ///This means the music plays at double speed.

                byte[] data = new byte[ReadVariableLengthValue()];
                Array.Copy(trackData, trackIndex, data, 0, data.Length);
                
                // Fab - 15/05/16 - Correct some eroneous times ...
                if (data[0] == 6 && data[1] == 3)
                {
                    data[0] = 12;
                    data[1] = 4;
                }
                newTrack.Insert(ticks, new MetaMessage(type, data));

                string TimeSignature = data[0].ToString();
                


                newTrack.Numerator = data[0];
                newTrack.Denominator = data[1];


                trackIndex += data.Length;

                #endregion timesignature
            }
            else
            {
                #region default
                byte[] data = new byte[ReadVariableLengthValue()];

                try
                {                    
                    Array.Copy(trackData, trackIndex, data, 0, data.Length);
                    newTrack.Insert(ticks, new MetaMessage(type, data));
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                trackIndex += data.Length;
                #endregion
            }
        }

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

            // Clean special characters
            sy = CleanSpecialChars(sy);

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
                        newTrack.TotalLyricsL += "\r";
                        newTrack.Lyrics.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });
                    }
                    else if (sy.Substring(0, 1) == "\r" && sy.Length > 2)
                    {
                        reste = sy.Substring(1, sy.Length - 1);

                        newTrack.TotalLyricsL += "\r";
                        newTrack.Lyrics.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });

                        newTrack.TotalLyricsL += reste;
                        newTrack.Lyrics.Add(new Track.Lyric() { Type = "text", Element = reste, TicksOn = ticks });
                    }
                    else if (sy.Length > 2 && sy.Substring(sy.Length - 2, 2) == "\r")
                    {
                        // Fini par \r
                        reste = sy.Substring(0, sy.Length - 2);

                        newTrack.TotalLyricsL += reste;
                        newTrack.Lyrics.Add(new Track.Lyric() { Type = "text", Element = reste, TicksOn = ticks });

                        newTrack.TotalLyricsL += "\r";
                        newTrack.Lyrics.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });
                    }
                    else
                    {
                        if (s != "")
                        {
                            // Pas de retour chariot
                            newTrack.TotalLyricsL += sy;
                            newTrack.Lyrics.Add(new Track.Lyric() { Type = "text", Element = sy, TicksOn = ticks });
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

            // Clean special characters
            sy = CleanSpecialChars(sy);           

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
                            newTrack.LyricsText.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });
                            newTrack.TotalLyricsT += "\r";
                        }

                        else if (sy.Substring(0, 1) == "\r" && sy.Length > 2)
                        {
                            reste = sy.Substring(1, sy.Length - 1);

                            newTrack.LyricsText.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });
                            newTrack.TotalLyricsT += "\r";

                            newTrack.TotalLyricsT += reste;
                            newTrack.LyricsText.Add(new Track.Lyric() { Type = "text", Element = reste, TicksOn = ticks });
                        }
                        else if (sy.Length > 2 && sy.Substring(sy.Length - 2, 2) == "\r")
                        {
                            // Fini par \r
                            reste = sy.Substring(0, sy.Length - 2);

                            newTrack.TotalLyricsT += reste;
                            newTrack.LyricsText.Add(new Track.Lyric() { Type = "text", Element = reste, TicksOn = ticks });

                            newTrack.TotalLyricsT += "\r";
                            newTrack.LyricsText.Add(new Track.Lyric() { Type = "cr", Element = "\r", TicksOn = ticks });
                        }
                        else
                        {
                            if (s != "")
                            {
                                // Pas de retour chariot
                                newTrack.TotalLyricsT += sy;
                                newTrack.LyricsText.Add(new Track.Lyric() { Type = "text", Element = sy, TicksOn = ticks });
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

        /// <summary>
        /// Replace special chars
        /// </summary>
        /// <param name="sy"></param>
        /// <returns></returns>
        private string CleanSpecialChars(string sy)
        {
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


        private void extractOldMidiTags(string str) {
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

        private void ParseSysExMessageStart()
        {
            // System exclusive cancels running status.
            runningStatus = 0;

            byte[] data = new byte[ReadVariableLengthValue() + 1];
            data[0] = (byte)SysExType.Start;

            try
            {
               
                    Array.Copy(trackData, trackIndex, data, 1, data.Length - 1);
                    newTrack.Insert(ticks, new SysExMessage(data));
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }            

            trackIndex += data.Length - 1;
        }

        private void ParseSysExMessageContinue()
        {
            trackIndex++;

            if(trackIndex >= trackData.Length)
            {
                //throw new MidiFileException("End of track unexpectedly reached.");
                Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseSysExMessage)");
                return;

            }

            // System exclusive cancels running status.
            runningStatus = 0;
           
            // If this is an escaped message rather than a system exclusive 
            // continuation message.
            if((trackData[trackIndex] & 0x80) == 0x80)
            {
                status = trackData[trackIndex];
                trackIndex++;

                ParseMessage();
            }
            else
            {
                byte[] data = new byte[ReadVariableLengthValue() + 1];
                data[0] = (byte)SysExType.Continuation;

                Array.Copy(trackData, trackIndex, data, 1, data.Length - 1);
                newTrack.Insert(ticks, new SysExMessage(data));

                trackIndex += data.Length - 1;
            }
        }

        private void ParseSysCommonMessage()
        {
            if(trackIndex >= trackData.Length)
            {
                //throw new MidiFileException("End of track unexpectedly reached.");
                Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseSysCommonMessage)");
                return;

            }

            // System common cancels running status.
            runningStatus = 0;

            scBuilder.Type = (SysCommonType)status;

            switch((SysCommonType)status)
            {
                case SysCommonType.MidiTimeCode:
                    scBuilder.Data1 = trackData[trackIndex];
                    trackIndex++;
                    break;

                case SysCommonType.SongPositionPointer:
                    scBuilder.Data1 = trackData[trackIndex];
                    trackIndex++;

                    if(trackIndex >= trackData.Length)
                    {
                        //throw new MidiFileException("End of track unexpectedly reached.");
                        Console.Write("\nERROR: End of track unexpectedly reached (TrackReader.cs ParseSysCommonMessage)");
                        return;

                    }

                    scBuilder.Data2 = trackData[trackIndex];
                    trackIndex++;
                    break;

                case SysCommonType.SongSelect:
                    scBuilder.Data1 = trackData[trackIndex];
                    trackIndex++;
                    break;

                case SysCommonType.TuneRequest:
                    // Nothing to do here.
                    break;
            }

            scBuilder.Build();

            newTrack.Insert(ticks, scBuilder.Result);
        }

        private void ParseSysRealtimeMessage()
        {
            SysRealtimeMessage e = null;

            switch((SysRealtimeType)status)
            {
                case SysRealtimeType.ActiveSense:
                    e = SysRealtimeMessage.ActiveSenseMessage;
                    break;

                case SysRealtimeType.Clock:
                    e = SysRealtimeMessage.ClockMessage;
                    break;

                case SysRealtimeType.Continue:
                    e = SysRealtimeMessage.ContinueMessage;
                    break;

                case SysRealtimeType.Reset:
                    e = SysRealtimeMessage.ResetMessage;
                    break;

                case SysRealtimeType.Start:
                    e = SysRealtimeMessage.StartMessage;
                    break;

                case SysRealtimeType.Stop:
                    e = SysRealtimeMessage.StopMessage;
                    break;

                case SysRealtimeType.Tick:
                    e = SysRealtimeMessage.TickMessage;
                    break;
            }

            if (e != null)
                newTrack.Insert(ticks, e);
        }

        private int ReadVariableLengthValue()
        {
            if(trackIndex >= trackData.Length)
            {
                throw new MidiFileException("\nERROR: End of track unexpectedly reached.");

            }

            int result = 0;

            result = trackData[trackIndex];

            trackIndex++;

            if((result & 0x80) == 0x80)
            {
                result &= 0x7F;

                int temp;

                do
                {
                    if(trackIndex >= trackData.Length)
                    {
                        //throw new MidiFileException("End of track unexpectedly reached.");
                        break;
                    }

                    temp = trackData[trackIndex];
                    trackIndex++;
                    result <<= 7;
                    result |= temp & 0x7F;
                }while((temp & 0x80) == 0x80);
            }

            return result;            
        }

        public Track Track
        {
            get
            {
                return track;
            }
        }
	}
}
