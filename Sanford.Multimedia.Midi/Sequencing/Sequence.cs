#region License

/* Copyright (c) 2006 Leslie Sanford
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    /// Represents a collection of Tracks.
    /// </summary>
    public sealed class Sequence : IComponent, ICollection<Track>
    {
        #region Sequence Members

        #region Fields

        // The collection of Tracks for the Sequence.
        //private List<Track> tracks = new List<Track>();
        // FAB : Modified in public List
        public List<Track> tracks = new List<Track>();
               
        // The Sequence's MIDI file properties.
        private MidiFileProperties properties = new MidiFileProperties();

        private BackgroundWorker loadWorker = new BackgroundWorker();

        private BackgroundWorker saveWorker = new BackgroundWorker();

        private ISite site = null;

        private bool disposed = false;

        #endregion

        #region Events

        public event EventHandler<AsyncCompletedEventArgs> LoadCompleted;

        public event ProgressChangedEventHandler LoadProgressChanged;

        public event EventHandler<AsyncCompletedEventArgs> SaveCompleted;

        public event ProgressChangedEventHandler SaveProgressChanged;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Sequence class.
        /// </summary>
        public Sequence()
        {
            InitializeBackgroundWorkers();
        }        

        /// <summary>
        /// Initializes a new instance of the Sequence class with the specified division.
        /// </summary>
        /// <param name="division">
        /// The Sequence's division value.
        /// </param>
        public Sequence(int division)
        {
            properties.Division = division;
            properties.Format = 1;
            
            //FAB
            properties.Tempo = 0;
            properties.Orig_Tempo = 0;            
            properties.Numerator = 0;
            properties.Denominator = 0;
            properties.Quarternote = division;

            properties.ResetLog();            
            InitializeBackgroundWorkers();
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with the specified
        /// file name of the MIDI file to load.
        /// </summary>
        /// <param name="fileName">
        /// The name of the MIDI file to load.
        /// </param>
        public Sequence(string fileName)
        {
            InitializeBackgroundWorkers();
            Load(fileName);
        }


        public Sequence(Stream fileStream)
        {
            InitializeBackgroundWorkers();
            Load(fileStream);
        }


        private void InitializeBackgroundWorkers()
        {
            loadWorker.DoWork += new DoWorkEventHandler(LoadDoWork);
            loadWorker.ProgressChanged += new ProgressChangedEventHandler(OnLoadProgressChanged);
            loadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnLoadCompleted);
            loadWorker.WorkerReportsProgress = true;

            saveWorker.DoWork += new DoWorkEventHandler(SaveDoWork);
            saveWorker.ProgressChanged += new ProgressChangedEventHandler(OnSaveProgressChanged);
            saveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnSaveCompleted);
            saveWorker.WorkerReportsProgress = true;
        }        

        #endregion

        #region Methods

 

        /// <summary>
        /// Loads a MIDI file into the Sequence.
        /// </summary>
        /// <param name="fileName">
        /// The MIDI file's name.
        /// </param>
        public void Load(string fileName)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if(IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion                        

            FileStream stream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read, FileShare.Read);

            using(stream)
            {
                MidiFileProperties newProperties = new MidiFileProperties();
                TrackReader reader = new TrackReader();
                List<Track> newTracks = new List<Track>();

                newProperties.Read(stream);

                for(int i = 0; i < newProperties.TrackCount; i++)
                {
                    reader.Read(stream);
                    newTracks.Add(reader.Track);

                    #region tempo
                    // ----------------------------------------------------
                    // FAB                                                
                    if (this.Tempo == 0)
                    {
                        this.Tempo = reader.Track.Tempo;
                    }
                    if (this.Numerator == 0)
                    {
                        this.Numerator = reader.Track.Numerator;
                        this.Denominator = reader.Track.Denominator;
                    }

                    #endregion tempo
                }

                properties = newProperties;
                tracks = newTracks;

                // FAB : determine time signature                    
                int tempo = this.Tempo;
                int numer = this.Numerator;
                int denom = this.Denominator;
                int quarternote = this.Division;

                if (tempo == 0)
                {
                    tempo = 500000; // 500,000 microseconds = 0.05 sec 
                }
                if (numer == 0)
                {
                    numer = 4; denom = 4;
                }

                timesig = new TimeSignature(numer, denom, quarternote, tempo);                
            }

            #region Ensure

            Debug.Assert(Count == properties.TrackCount);

            #endregion
        }

        /// <summary>
        /// Loads a MIDI stream into the Sequence.
        /// </summary>
        /// <param name="fileStream">
        /// The MIDI file's stream.
        /// </param>
        public void Load(Stream fileStream)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            #endregion

            using (fileStream)
            {
                MidiFileProperties newProperties = new MidiFileProperties();
                TrackReader reader = new TrackReader();
                List<Track> newTracks = new List<Track>();

                newProperties.Read(fileStream);

                for (int i = 0; i < newProperties.TrackCount; i++)
                {
                    reader.Read(fileStream);
                    newTracks.Add(reader.Track);
                }

                properties = newProperties;
                tracks = newTracks;
            }

            #region Ensure

            Debug.Assert(Count == properties.TrackCount);

            #endregion
        }


        public void LoadAsync(string fileName)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if(IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            loadWorker.RunWorkerAsync(fileName);

        }

        public void LoadAsyncCancel()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            loadWorker.CancelAsync();
        }

        /// <summary>
        /// Saves the Sequence as a MIDI file.
        /// </summary>
        /// <param name="fileName">
        /// The name to use for saving the MIDI file.
        /// </param>
        public void Save(string fileName)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            FileStream stream = new FileStream(fileName, FileMode.Create,
                FileAccess.Write, FileShare.None);

            using(stream)
            {
                properties.Write(stream);

                TrackWriter writer = new TrackWriter();

                foreach(Track trk in tracks)
                {
                    writer.Track = trk;
                    writer.Write(stream);
                }
            }
        }

        public void SaveAsync(string fileName)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if(IsBusy)
            {
                throw new InvalidOperationException();
            }
            else if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            saveWorker.RunWorkerAsync(fileName);
        }

        public void SaveAsyncCancel()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            saveWorker.CancelAsync();
        }


        /// <summary>
        /// FAB: Dump a sequence to text file
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteDump(string fileName, string dumpFileName)
        {
            #region Require
            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (dumpFileName == null)
            {
                throw new ArgumentNullException("dumpFileName");
            }
            else if (this.tracks.Count == 0)
                throw new Exception("No tracks");
            #endregion


            try
            {
                FileStream fstream = new FileStream(dumpFileName, FileMode.Create,
                    FileAccess.Write, FileShare.None);

                StreamWriter stream = new StreamWriter(fstream);

                using (stream)
                {
                    DumpWriter dumpwriter = new DumpWriter(this, fileName);
                    dumpwriter.Write(stream);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /*
        /// <summary>
        /// FAB: Create a sequence from a text file
        /// </summary>
        /// <param name="dumpFileName"></param>
        public Sequence ReadDump(string dumpFileName)
        {
            #region Require
            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (dumpFileName == null)
            {
                throw new ArgumentNullException("dumpFileName");
            }
            #endregion

            FileStream fstream = new FileStream(dumpFileName, FileMode.Open,
                FileAccess.Read, FileShare.None);

            StreamReader stream = new StreamReader(fstream);

            using (stream)
            {
                DumpReader dumpreader = new DumpReader();
                return dumpreader.Read(stream);
            }
        }
        */

        /// <summary>
        /// Gets the length in ticks of the Sequence.
        /// </summary>
        /// <returns>
        /// The length in ticks of the Sequence.
        /// </returns>
        /// <remarks>
        /// The length in ticks of the Sequence is represented by the Track 
        /// with the longest length.
        /// </remarks>
        public int GetLength()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            int length = 0;

            foreach(Track t in this)
            {
                if(t.Length > length)
                {
                    length = t.Length;
                }
            }

            return length;
        }

        /// <summary>
        /// FAB: get ticks of last note played
        /// </summary>
        /// <returns></returns>
        public int GetLastNote()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            int maxticks = 0;

            foreach (Track t in this)
            {
                int tck = 0;
                if (t.Notes.Count > 0)
                    tck = t.Notes[t.Notes.Count - 1].StartTime;

                if (tck > maxticks)
                {
                    maxticks = tck;
                }
            }

            return maxticks;
        }

        /// <summary>
        /// Get end time of last note
        /// </summary>
        /// <returns></returns>
        public int GetLastNoteEndTime()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            int maxticks = 0;
            int tck = 0;
            foreach (Track t in this)
            {                
                if (t.Notes.Count > 0)
                    tck = t.Notes[t.Notes.Count - 1].EndTime;

                if (tck > maxticks)
                {
                    maxticks = tck;
                }
            }

            return maxticks;
        }



        /// <summary>
        /// Clean sequence: Clean all MIDI events after the last note
        /// </summary>
        public void Clean()
        {
            // Cleaning: Clean all MIDI events after the last note
            int lastNoteTicks = GetLastNote();
            if (Time != null)
            {
                int measurelen = Time.Measure;
                int nbMeasures = 1 + lastNoteTicks / measurelen;
                int after = (nbMeasures + 1) * measurelen;
                CleanMidiEvents(after);
            }
        }

        /// <summary>
        /// FAB: delete all events after "after"
        /// </summary>
        /// <param name="after"></param>
        public void CleanMidiEvents(int after)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion         

            foreach (Track t in this)
            {
                t.deleteMidiEventsAfter(after);
            }
        }

        /// <summary>
        /// FAB: delete a track
        /// </summary>
        /// <param name="tracknum"></param>
        public void TrackDelete(Track track)
        {
            track.Clear();
            this.tracks.Remove(track);
            properties.TrackCount = tracks.Count;
        }

        /// <summary>
        /// Extract notes for each track of the sequence
        /// </summary>
        public void ExtractNotes()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion         

            foreach (Track t in this)
            {
                t.ExtractNotes();
            }
        }


        /// <summary>
        /// Transpose notes of all tracks
        /// </summary>
        /// <param name="amount"></param>
        public void Transpose(int amount)
        {
            #region Require
            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            #endregion

            List<Track> result = new List<Track>();

            foreach (Track t in this)
            {
                Track track = new Track();

                if (t.MidiChannel != 9)
                {                    
                    if (t.Notes.Count > 0)
                    {
                        // Copy all avents and change note value
                        foreach (MidiEvent e in t.Iterator())
                        {
                            IMidiMessage m = e.MidiMessage;
                            if (m.MessageType == MessageType.Channel)
                            {
                                // FAB: Amelioration 23/01/18
                                ChannelMessage msg = (ChannelMessage)e.MidiMessage;
                                ChannelCommand cmd = msg.Command;

                                int channel = msg.MidiChannel;
                                int number = msg.Data1;
                                int velocity = msg.Data2;
                                int ticks = e.AbsoluteTicks;

                                //ChannelCommand cmd = ChannelMessage.UnpackCommand(m.Status);
                                //int number = m.Data1; // note number                                
                                number += amount;
                                //int channel = t.MidiChannel;
                                //int velocity = m.Data2;

                                if (cmd == ChannelCommand.NoteOn || cmd == ChannelCommand.NoteOff)
                                {                                                                        
                                    ChannelMessage message = new ChannelMessage(cmd, channel, number, velocity);
                                    track.Insert(ticks, message);
                                }
                                else
                                {
                                    track.Insert(ticks, m);
                                }
                            }
                            else
                            {
                                track.Insert(e.AbsoluteTicks, m);
                            }
                        }

                        foreach (MidiNote n in t.Notes)
                        {
                            n.Number += amount;
                            track.Notes.Add(n);
                        }

                        // Add modified
                        track.MidiChannel = t.MidiChannel;
                        result.Add(track);
                    }
                    else
                    {
                        // Add as it is
                        result.Add(t);
                    }
                }
                else
                {
                    // Add as it is
                    result.Add(t);
                }

            }

            // Replace by new tracks
            this.tracks.Clear();
            foreach (Track track in result)
            {
                this.tracks.Add(track);
            }
        }


        private void OnLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadProgressChanged?.Invoke(this, e);
        }


        /// <summary>
        /// Load track by loadworker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open,
                    FileAccess.Read, FileShare.Read);

                using (stream)
                {
                    MidiFileProperties newProperties = new MidiFileProperties();
                    TrackReader reader = new TrackReader();
                    List<Track> newTracks = new List<Track>();

                    newProperties.Read(stream);

                    float percentage;
                    
                    // Initialize tags
                    MidiTags.ResetTags();

                    for (int i = 0; i < newProperties.TrackCount && !loadWorker.CancellationPending; i++)
                    {                                                                        
                        reader.Read(stream);

                        // FAB : MTRK not found in TrackReader.cs FindTrack()
                        if (reader.Track != null)
                            newTracks.Add(reader.Track);

                        #region tempo
                        // ----------------------------------------------------

                        if (newProperties.Tempo == 0)
                        {
                            newProperties.Tempo = reader.Track.Tempo;
                        }   
                        if (newProperties.Numerator == 0)
                        {
                            newProperties.Numerator = reader.Track.Numerator;
                            newProperties.Denominator = reader.Track.Denominator;
                        }
                        // ----------------------------------------------------
                        #endregion tempo

                        percentage = (i + 1f) / newProperties.TrackCount;
                        loadWorker.ReportProgress((int)(100 * percentage));
                    }
                    
                    // All tracks have been read
                    if (loadWorker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        properties = newProperties;

                        Tempo = properties.Tempo;
                        Numerator = properties.Numerator;
                        Denominator = properties.Denominator;
                        Orig_Tempo = Tempo;

                        // FAB : determine time signature                    
                        int quarternote = properties.Division;

                        if (Tempo == 0)
                        {
                            Tempo = 500000; // 500,000 microseconds = 0.05 sec 
                            properties.AddLog("No tempo found. Set tempo to default value 500000.");
                        }
                        if (Numerator == 0)
                        {
                            Numerator = 4; Denominator = 4;
                            properties.AddLog("No Numerator found. Set Numerator & Denominator to default value 4/4.");
                        }

                        // If we only have one track with multiple channels, then treat
                        // each channel as a separate track.
                        //if (this.Format == 0)
                        if (properties.Format == 0)
                        {
                            
                            List<Track> thetracks = SplitChannels(newTracks[0], this.Tempo, this.Numerator, this.Denominator);
                            tracks = thetracks;
                            //this.Format = 1;        // FAB : peut pas forcer format to 1 ici ; Isbusy = true hu hu hu...

                            OrigFormat = 0;

                            //newProperties.OrigFormat = 0;
                            properties.AddLog("Switched format from 0 to 1.");


                            // ? mieux ?
                            //newProperties.Format = 1;
                            //newProperties.TrackCount = tracks.Count;
                            properties.Format = 1;
                            properties.TrackCount = tracks.Count;
                            
                        }
                        else
                        {
                            tracks = newTracks;
                            OrigFormat = 1;
                        }

                                             
                        
                        /*
                         * Here you can force Numerator & Denominator to another Value 
                         * 
                         * 
                         */
                        // Uncomment
                        /*    
                        if (Numerator == 12 && Denominator == 3)
                        {
                            Numerator = 3;
                            Denominator = 4;
                        }
                        */

                        timesig = new TimeSignature(Numerator, Denominator, quarternote, tempo);

                        // A corriger !!! 2 pistes tout le temps !!!
                        // If we have 1 track, we can split it int 2 tacks
                        // to have left hand and rignt hand for piano

                        if (OpenMidiFileOptions.SplitHands == true)
                        {
                            if (tracks.Count == 1)
                            {
                                if (this.Format == 1)
                                {
                                    int measurelen = timesig.Measure;
                                    tracks = CombineToTwoTracks(tracks[0], measurelen);

                                    // mieux ?
                                    //newProperties.TrackCount = tracks.Count;
                                    properties.TrackCount = tracks.Count;
                                }
                            } else
                            {
                                string msg = "You cannot split a file having more than one track.";
                                properties.AddLog(msg);
                            }
                        }
                        Log = properties.Log;
                        
                        // Tags to sequence
                        CloneTags();
                    }
                }
            }
            catch (Exception ee)
            {
                // FAB TODO : how to cancel loading?

                Console.Write(ee.ToString());
                e.Cancel = true;
                

            }
            
        }

        
        /// <summary>
        /// FAB: Split the given track into multiple tracks, separating each
        /// channel into a separate track.
        /// </summary>
        /// <param name="origtrack"></param>
        /// <returns></returns>
        private static List<Track> SplitChannels(Track origtrack, int Tempo, int Numerator, int Denominator)
        {
            int i = 0;

            /* Find the instrument used for each channel */
            int[] channelInstruments = new int[16]; // Tableau de 16 int

            foreach (MidiEvent e in origtrack.Iterator())
            {
                if (e.MidiMessage.MessageType == MessageType.Channel)
                {                    
                    IMidiMessage a = e.MidiMessage;

                    // on cherche le changement d'instrument : ChannelCommand.ProgramChange
                    ChannelCommand b = ChannelMessage.UnpackCommand(a.Status);
                    if (b == ChannelCommand.ProgramChange)
                    {
                        
                        // On cherche le channel ?
                        int channel = ChannelMessage.UnpackMidiChannel(a.Status);
                                                
                        // On cherche l'instrument ?
                        //int instrument = ChannelMessage.UnpackData1(a.Status);
                        int instrument = a.Data1;

                        channelInstruments[channel] = instrument;
                    }       
                }
            }

            // Drums: channel 9
            channelInstruments[9] = 127; /* Channel 9 = Percussion */
            
            // Répartition des notes dans chaque nouvelle track
            List<Track> result = new List<Track>();
            bool foundchannel = false;

            foreach (MidiNote note in origtrack.Notes)
            {
                foundchannel = false;
                
                foreach (Track track in result)
                {
                    if (note.Channel == track.Notes[0].Channel)
                    {
                        foundchannel = true;
                        track.Notes.Add(note);                                          
                    }
                }
                
                if (!foundchannel)
                {                   
                    // Create a new track
                    Track track = new Track();           
         
                    track.Notes.Add(note);                   
                    track.ContainsNotes = true;
                    track.Visible = true;
                    
                    track.ProgramChange = channelInstruments[note.Channel];                                                          
                    track.MidiChannel = note.Channel;
                    track.insertPatch(note.Channel, track.ProgramChange);

                    i++;
                    track.Name = "track" + i.ToString();
                    track.insertTrackname(track.Name);

                    // FAB 21/03/2015
                    track.Tempo = Tempo;
                    track.insertTempo(Tempo);
                    track.insertKeysignature(Numerator, Denominator);
                    track.insertTimesignature(Numerator, Denominator);

                    result.Add(track);
                    
                }
            }

            // Répartition des events dans chaque nouvelle track
            foreach (MidiEvent e in origtrack.Iterator())
            {
                IMidiMessage a = e.MidiMessage;
                int channel = ChannelMessage.UnpackMidiChannel(a.Status);

                foreach (Track track in result)
                {
                    if (track.MidiChannel == channel)
                    {
                        int position = e.AbsoluteTicks;
                        track.Insert(position, a);
                    }
                }               
            }

            // A revoir : insertion des Lyrics dans une track
            if (origtrack.Lyrics != null)
            {
                // Decision totalement arbitraire : lyrics de type L dans piste 0
                if (result.Count > 0)
                {
                    Track track = result[0];

                    foreach (Track.Lyric lyricEvent in origtrack.Lyrics)
                    {
                        track.TotalLyricsL += lyricEvent.Element;
                        track.Lyrics.Add(lyricEvent);
                    }
                }
            }
            
            return result;
        }

     

        public void CloneTags()
        {
            #region oldtags
            /*
            * Midi file tags
            * @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            * @L	(single) Language	FRAN, ENGL        
            * @W	(multiple) Copyright (of Karaoke file, not song)        
            * @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            * @I	Information  ex Date(of Karaoke file, not song)
            * @V	(single) Version ex 0100 ?        
            */

            Copyright = MidiTags.Copyright != null ? MidiTags.Copyright : string.Empty;
            KTag = MidiTags.KTag; // != null ? MidiTags.KTag : new List<string>();
            LTag = MidiTags.LTag; // != null ? MidiTags.LTag : new List<string>();
            ITag = MidiTags.ITag; // != null ? MidiTags.ITag : new List<string>();
            VTag = MidiTags.VTag; // != null ? MidiTags.VTag : new List<string>();
            TTag = MidiTags.TTag; // != null ? MidiTags.TTag : new List<string>();
            WTag = MidiTags.WTag; // != null ? MidiTags.WTag : new List<string>();
            #endregion

            #region newtags
            TagTitle = MidiTags.TagTitle; // != null ? MidiTags.tagTitle : string.Empty;
            TagArtist = MidiTags.TagArtist; // != null ? MidiTags.tagArtist : string.Empty;
            TagAlbum = MidiTags.TagAlbum; // != null ? MidiTags.tagAlbum : string.Empty;
            TagCopyright = MidiTags.TagCopyright; // != null ? MidiTags.tagCopyright : string.Empty;
            TagDate = MidiTags.TagDate; // != null ? MidiTags.tagDate : string.Empty;
            TagEditor = MidiTags.TagEditor; // != null ? MidiTags.tagEditor : string.Empty;
            TagGenre = MidiTags.TagGenre; // != null ? MidiTags.tagGenre : string.Empty;
            TagEvaluation = MidiTags.TagEvaluation; // != null ? MidiTags.tagEvaluation : string.Empty;
            TagComment = MidiTags.TagComment; // != null ? MidiTags.tagComment : string.Empty;
            #endregion        
        }


        #region split track

        /// <summary>
        /// Combine the notes in all the tracks given into two MidiTracks,
        /// and return them.
        /// This function is intended for piano songs, when we want to display
        /// a left-hand track and a right-hand track.  The lower notes go into
        /// the left-hand track, and the higher notes go into the right hand
        /// track.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="measurelen"></param>
        /// <returns></returns>
        public static List<Track> CombineToTwoTracks(Track track, int measurelen)
        {            
            List<Track> result = SplitTrack(track, measurelen);           
            return result;
        }

        /// <summary>
        /// Split the given MidiTrack into two tracks, top and bottom.
        /// The highest notes will go into top, the lowest into bottom.
        /// This function is used to split piano songs into left-hand (bottom)
        /// and right-hand (top) tracks.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="measurelen"></param>
        /// <returns></returns>
        public static List<Track> SplitTrack(Track track, int measurelen)
        {
            List<MidiNote> notes = track.Notes;
            int count = notes.Count;

            Track top = new Track();
            Track bottom = new Track();
            List<Track> result = new List<Track>(2);
            result.Add(top); result.Add(bottom);

            if (count == 0)
                return result;

            int prevhigh = 76; /* E5, top of treble staff */
            int prevlow = 45; /* A3, bottom of bass staff */
            int startindex = 0;

            foreach (MidiNote note in notes)
            {
                int high, low, highExact, lowExact;

                int number = note.Number;
                high = low = highExact = lowExact = number;

                while (notes[startindex].EndTime < note.StartTime)
                {
                    startindex++;
                }

                /* I've tried several algorithms for splitting a track in two,
                 * and the one below seems to work the best:
                 * - If this note is more than an octave from the high/low notes
                 *   (that start exactly at this start time), choose the closest one.
                 * - If this note is more than an octave from the high/low notes
                 *   (in this note's time duration), choose the closest one.
                 * - If the high and low notes (that start exactly at this starttime)
                 *   are more than an octave apart, choose the closest note.
                 * - If the high and low notes (that overlap this starttime)
                 *   are more than an octave apart, choose the closest note.
                 * - Else, look at the previous high/low notes that were more than an 
                 *   octave apart.  Choose the closeset note.
                 */
                FindHighLowNotes(notes, measurelen, startindex, note.StartTime, note.EndTime,
                                 ref high, ref low);
                FindExactHighLowNotes(notes, startindex, note.StartTime,
                                      ref highExact, ref lowExact);

                if (highExact - number > 12 || number - lowExact > 12)
                {
                    if (highExact - number <= number - lowExact)
                    {
                        top.addNote(note);
                    }
                    else
                    {
                        bottom.addNote(note);
                    }
                }
                else if (high - number > 12 || number - low > 12)
                {
                    if (high - number <= number - low)
                    {
                        top.addNote(note);
                    }
                    else
                    {
                        bottom.addNote(note);
                    }
                }
                else if (highExact - lowExact > 12)
                {
                    if (highExact - number <= number - lowExact)
                    {
                        top.addNote(note);
                    }
                    else
                    {
                        bottom.addNote(note);
                    }
                }
                else if (high - low > 12)
                {
                    if (high - number <= number - low)
                    {
                        top.addNote(note);
                    }
                    else
                    {
                        bottom.addNote(note);
                    }
                }
                else
                {
                    if (prevhigh - number <= number - prevlow)
                    {
                        top.addNote(note);
                    }
                    else
                    {
                        bottom.addNote(note);
                    }
                }

                /* The prevhigh/prevlow are set to the last high/low
                 * that are more than an octave apart.
                 */
                if (high - low > 12)
                {
                    prevhigh = high;
                    prevlow = low;
                }
            }

            top.Notes.Sort(track.Notes[0]);
            bottom.Notes.Sort(track.Notes[0]);

            return result;
        }


        /* Find the highest and lowest notes that overlap this interval (starttime to endtime).
         * This method is used by SplitTrack to determine which staff (top or bottom) a note
         * should go to.
         *
         * For more accurate SplitTrack() results, we limit the interval/duration of this note 
         * (and other notes) to one measure. We care only about high/low notes that are
         * reasonably close to this note.
         */
        private static void FindHighLowNotes(List<MidiNote> notes, int measurelen, int startindex, int starttime, int endtime, ref int high, ref int low)
        {

            int i = startindex;
            if (starttime + measurelen < endtime)
            {
                endtime = starttime + measurelen;
            }

            while (i < notes.Count && notes[i].StartTime < endtime)
            {
                if (notes[i].EndTime < starttime)
                {
                    i++;
                    continue;
                }
                if (notes[i].StartTime + measurelen < starttime)
                {
                    i++;
                    continue;
                }
                if (high < notes[i].Number)
                {
                    high = notes[i].Number;
                }
                if (low > notes[i].Number)
                {
                    low = notes[i].Number;
                }
                i++;
            }
        }

        /* Find the highest and lowest notes that start at this exact start time */
        private static void FindExactHighLowNotes(List<MidiNote> notes, int startindex, int starttime, ref int high, ref int low)
        {

            int i = startindex;

            while (notes[i].StartTime < starttime)
            {
                i++;
            }

            while (i < notes.Count && notes[i].StartTime == starttime)
            {
                if (high < notes[i].Number)
                {
                    high = notes[i].Number;
                }
                if (low > notes[i].Number)
                {
                    low = notes[i].Number;
                }
                i++;
            }
        }


        #endregion split track


        private void OnSaveCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnSaveProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SaveProgressChanged?.Invoke(this, e);
        }

        private void SaveDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Create,
                    FileAccess.Write, FileShare.None);


                using (stream)
                {
                    properties.Write(stream);

                    TrackWriter writer = new TrackWriter();

                    float percentage;

                    for (int i = 0; i < tracks.Count && !saveWorker.CancellationPending; i++)
                    {
                        writer.Track = tracks[i];
                        writer.Write(stream);

                        percentage = (i + 1f) / properties.TrackCount;

                        saveWorker.ReportProgress((int)(100 * percentage));
                    }

                    if (saveWorker.CancellationPending)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        #region Properties

        #region oldtags
        /*
        Midi file tags
        @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
        @L	(single) Language	FRAN, ENGL        
        @W	(multiple) Copyright (of Karaoke file, not song)        
        @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
        @I	Information  ex Date(of Karaoke file, not song)
        @V	(single) Version ex 0100 ?        
        */
        // copyright : all  begining with @ ?
        public string Copyright { get; set; }
        // @K        
        public  List<string> KTag { get; set; }
        // @L
        public  List<string> LTag { get; set; }
        // @I
        public  List<string> ITag { get; set; }
        // @V
        public  List<string> VTag { get; set; }
        // @T
        public  List<string> TTag { get; set; }
        // @W
        public  List<string> WTag { get; set; }
        #endregion

        #region newtags
        // New tags similar to mp3
        // @id01   Title
        // @id02   Artist
        // @id03   Album
        // @id04   Copyright
        // @id05   Date
        // @id06   Editor
        // @id07   Genre        
        // @id08   Evaluation
        // @id09   Comment


        /// <summary>
        /// Song Title: @id01
        /// </summary>
        // Song title
        public  string TagTitle { get; set; }

        /// <summary>
        /// Artist: @id02
        /// </summary>
        // Artist name
        public  string TagArtist { get; set; }

        /// <summary>
        /// Album: @id03
        /// </summary>
        // Album name
        public  string TagAlbum { get; set; }

        /// <summary>
        /// Copyright: @id04
        /// </summary>
        // Copyright of song
        public  string TagCopyright { get; set; }

        /// <summary>
        /// Date: @id05
        /// </summary>
        // Date of album or song
        public  string TagDate { get; set; }

        /// <summary>
        /// Editor: @id06
        /// </summary>
        // Editor
        public  string TagEditor { get; set; }

        /// <summary>
        /// Genre: @id07
        /// </summary>
        // Genre: pop, folk r&b
        public  string TagGenre { get; set; }

        /// <summary>
        /// Evaluation: @id08
        /// </summary>
        // Evaluation (1 to 5)
        public  string TagEvaluation { get; set; }

        /// <summary>
        /// Comment: @id09
        /// </summary>
        // Comment;
        public  string TagComment { get; set; }


        #endregion
        
        public bool HasLyrics
        {
            get
            {
                foreach (Track T in tracks)
                {
                    if (T.HasLyrics)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the Track to get.
        /// </param>
        /// <returns>
        /// The Track at the specified index.
        /// </returns>
        public Track this[int index]
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }
                else if(index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index", index,
                        "Sequence index out of range.");
                }

                #endregion

                return tracks[index];
            }
        }

        /// <summary>
        /// Gets the Sequence's division value.
        /// </summary>
        public int Division
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }

                #endregion

                return properties.Division;
            }
            set
            {
                #region Require
                if (disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }
                #endregion
                
                properties.Division = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sequence's format value.
        /// </summary>
        public int Format
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }

                #endregion

                return properties.Format;
            }
            set
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }
                else if(IsBusy)
                {
                    throw new InvalidOperationException();
                }

                #endregion

                properties.Format = value;
            }
        }

        /// <summary>
        /// Gets the Sequence's type.
        /// </summary>
        public SequenceType SequenceType
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }

                #endregion

                return properties.SequenceType;
            }
        }

        public bool IsBusy
        {
            get
            {
                return loadWorker.IsBusy || saveWorker.IsBusy;
            }
        }

        /// <summary>
        /// FAB - Tempo
        /// </summary>
        /// 
        private int origformat;
        public int OrigFormat {
            get { return origformat; }
            set { origformat = value; }
        }


        private int tempo;
        public int Tempo
        {
            get
            {
                return tempo;
            }
            set
            {
                tempo = value;
            }
        }

        private int orig_tempo;
        public int Orig_Tempo
        {
            get { return orig_tempo; }
            set { orig_tempo = value; }
        }

        private int numerator;
        public int Numerator
        {
            get
            {
                return numerator;
            }
            set
            {
                numerator = value;
            }
        }

        private int denominator;
        public int Denominator
        {
            get
            {
                return denominator;
            }
            set
            {
                denominator = value;
            }
        }

        private int quarternote;
        public int Quarternote
        {
            get
            {
                return quarternote;
            }
            set
            {
                quarternote = value;
            }
        }

        /// <summary>
        /// FAB time signature
        /// </summary>
        private TimeSignature timesig;
        /** Get the time signature */
        public TimeSignature Time
        {
            get { return timesig; }
            set { timesig = value; }
        }


        public bool SplitHands { get; set; }

        public string TextEncoding { get; set; }

        private string log;
        public string Log
        {
            get { return log; }
            set { log = value; }
        }

        #endregion

        #endregion

        #region ICollection<Track> Members

        /// <summary>
        /// FAB : insert function
        /// </summary>
        /// <param name="trackindex"></param>
        /// <param name="item"></param>
        public void Insert(int trackindex, Track item)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            #endregion

            tracks.Insert(trackindex, item);
            properties.TrackCount = tracks.Count;
        }
        
        public void Add(Track item)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            } 
            else if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            #endregion

            tracks.Add(item);

            properties.TrackCount = tracks.Count;
        }

        public void Clear()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            tracks.Clear();

            properties.TrackCount = tracks.Count;
        }

        public bool Contains(Track item)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            return tracks.Contains(item);
        }

        public void CopyTo(Track[] array, int arrayIndex)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            tracks.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }

                #endregion

                return tracks.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("Sequence");
                }

                #endregion

                return false;
            }
        }

        public bool Remove(Track item)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            bool result = tracks.Remove(item);

            if(result)
            {
                properties.TrackCount = tracks.Count;
            }

            return result;
        }

        #endregion

        #region IEnumerable<Track> Members

        public IEnumerator<Track> GetEnumerator()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            return tracks.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }

            #endregion

            return tracks.GetEnumerator();
        }

        #endregion

        #region IComponent Members

        public event EventHandler Disposed;

        public ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion

            loadWorker.Dispose();
            saveWorker.Dispose();

            disposed = true;

            EventHandler handler = Disposed;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

    
    }
}
