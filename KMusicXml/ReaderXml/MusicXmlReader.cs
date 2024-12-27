using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MusicXml.Domain;
using Sanford.Multimedia.Midi;
using System.Runtime.CompilerServices;
using System.Data.Sql;

namespace MusicXml
{
    public class MusicXmlReader
    {
        private BackgroundWorker loadXmlWorker = new BackgroundWorker();

        // xmlmusic
        public event EventHandler<AsyncCompletedEventArgs> LoadXmlCompleted;
        public event ProgressChangedEventHandler LoadXmlProgressChanged;

        private bool disposed = false;

        
        // 2 tracks can be create for the same part
        public Sequence seq = new Sequence();

        private Track track1 = new Track();
        private Track track2 = new Track();
        
        private List<Track> newTracks;
        private List<MidiNote> newNotes = new List<MidiNote>();

        private StreamReader stream;

        private Sequence sequence;
        private int Format = 1;
        private int Numerator = 4;
        private int Denominator = 4;        
        private int Division = 480; 
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
        private int lyricLengh = 0; // Length of lyrics

        // Constructor
        public MusicXmlReader() 
        {
            InitializeBackgroundWorkers();
            MidiTags.ResetTags();
        }

        private void InitializeBackgroundWorkers()
        {
            // xmlmusic
            loadXmlWorker.DoWork += new DoWorkEventHandler(LoadXmlDoWork);
            loadXmlWorker.ProgressChanged += new ProgressChangedEventHandler(OnLoadXmlProgressChanged);
            loadXmlWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnLoadXmlCompleted);
            loadXmlWorker.WorkerReportsProgress = true;
        }

        bool bSilenceMode = false;

        /// <summary>
        /// load async xmlmusic
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void LoadXmlAsync(string fileName, bool silenceMode = false)
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Sequence");
            }
            else if (IsXmlBusy)
            {
                throw new InvalidOperationException();
            }
            else if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion
            bSilenceMode |= silenceMode;
            loadXmlWorker.RunWorkerAsync(fileName);
        }

        #region xmlmusic
        private void OnLoadXmlCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadXmlCompleted?.Invoke(this, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
        }

        private void OnLoadXmlProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadXmlProgressChanged?.Invoke(this, e);
        }

        #endregion xmlmusic

        /// <summary>
        /// Load xmlmusic track by LoadXmlWorker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadXmlDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = (string)e.Argument;

            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open,
                    FileAccess.Read, FileShare.Read);

                using (stream)
                {
                    // Your stuff
                    seq = Read(fileName, true);
                }
            }
            catch (Exception ee)
            {
                Console.Write(ee.ToString());
                e.Cancel = true;
                
            }
        }


        /// <summary>
        /// xmlmusic
        /// </summary>
        public bool IsXmlBusy
        {
            get
            {
                return loadXmlWorker.IsBusy;
            }
        }


        /// <summary>
        /// Create a score objetc
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Sequence Read(string fileName, bool silencemode)
        {
            bSilenceMode = silencemode;
            System.Xml.Linq.XDocument doc;

            // Create Score
            try
            {
                doc = XDocument.Load(fileName);
            }
            catch (Exception ex)
            {                
                if (!bSilenceMode) 
                    MessageBox.Show("Invalid MusicXml file\n" + ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            Score myscore = Score.Create(doc);

            return Load(myscore);
        }


        /// <summary>
        /// Read MusicXml score and convert to Midi
        /// </summary>
        /// <param name="SC"></param>
        /// <returns></returns>
        public Sequence Load(MusicXml.Domain.Score SC) 
        {                         
            string Id = null;
                        
            Identification Identification = SC.Identification;
            String MovementTitle = SC.MovementTitle;
            

            // List of tracks
            List<Part> Parts = SC.PartList;

            #region check
            if (Parts.Count == 0)
                return null;
            #endregion


            // Create MAP of measures according to repeat backward/forward found
            List<List<int>> mapmeasures = CreateVerses(Parts[0].Measures);


            // Init sequence
            newTracks = new List<Track>(Parts.Count);
            Tempo = Parts[0].Tempo;
            Format = 1;
            
            if (Parts[0].Numerator > 0)
                Numerator = Parts[0].Numerator;
            
            if (Parts[0].Denominator > 0)
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
            int commondivision = ((Parts[0].Division > 0) ? Parts[0].Division  :  24); 

            
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
                Channel = part.MidiChannel - 1;
                if (Channel > 15)
                    break;

                ProgramChange = part.MidiProgram - 1;
                Volume = part.Volume;
                Pan = part.Pan;

                /*
                 *  Attention, certaines partitions ont une division différente pour chaque piste !!!
                 *  Exemple BeetAnGeSample.xml
                 *  Part 1 : division 24
                 *  Part 2 : division 96
                 *  
                 *  Conclusion, il faut gérer chaque piste séparément
                 */
                double multcoeff = 1;       // Mutiply everything in order to have common Division
                
                if (part.Division == 0)
                {
                    Console.WriteLine("ERROR: Division = 0");
                    part.Division = 24;
                }
                
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

                int versenumber = 0;
                

                // =========================================                
                // mapmesure is the list of verses.
                // Each element of mapmesure is a list of measure indices that belong to a verse. 
                // (The same measure may belong to several verses)
                // =========================================
                foreach (List<int> lmap in mapmeasures)
                {

                    bool bFound = false;
                    int indice;
                    
                    // Search for verse number
                    for (int i = 0; i < lmap.Count; i++)
                    {
                        indice = lmap[i];
                        if (indice < Measures.Count)
                        {
                            Measure measure = Measures[indice];

                            if (measure.lstVerseNumber.Count > 1)
                            {
                                versenumber++;
                                bFound = true;
                                break;
                            }
                        }
                    }

                    if (!bFound)
                        versenumber = 0;

                    
                    for (int i = 0; i < lmap.Count; i++)
                    {
                        indice = lmap[i];
                        if (indice < Measures.Count) 
                        { 

                            Measure measure = Measures[indice];
                            
                            // BEGIN RECUP
                            decimal W = measure.Width;
                            int notenumber = 0;

                            Key k = measure.Attributes.Key;
                            int fif = k.Fifths;
                            string mod = k.Mode;


                            // *************** TAKE INTO ACCOUNT THOSE THINGS !!!!! ***************
                            //
                            // if (fif != 0)
                            //{
                            //    Console.WriteLine(fif.ToString());
                            //}


                            #region Extract all
                            List<MeasureElement> lstME = measure.MeasureElements;

                            // For each measureElement in current measure
                            foreach (MeasureElement measureElement in lstME)
                            {

                                object obj = measureElement.Element;
                                MeasureElementType metype = measureElement.Type;

                                switch (metype)
                                {

                                    case MeasureElementType.Time:
                                        Time tm = (Time)obj;
                                        float ttempo = tm.Tempo;
                                        CreateTempoEvent(ttempo, timeline);
                                        break;
                                    
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
                                        

                                        // keep only the good number of the verse             
                                        List<Lyric> lyrics = note.Lyrics;
                                        
                                        string ntype = note.Type;


                                        // Note duration
                                        //int t = part.Division;
                                        note.Duration = (int)(note.Duration * multcoeff);
                                        
                                        /*
                                        switch (note.Type)
                                        {
                                            case "whole":
                                                t = t/8;                                                
                                                break;
                                            case "half":
                                                t = t/8;
                                                break;
                                            case "quarter":
                                                t = t/8;                                                
                                                break;
                                            case "eighth":
                                                t = t/8;                                               
                                                break;
                                            case "16th":
                                                t = t/8;                                                
                                                break;
                                            case "32nd":
                                                t = t/8;                                                
                                                break;
                                            case "":
                                                t = t/8; // part.Division;
                                                break;
                                            default:
                                                break;
                                        }

                                        int test1 = (int)(note.DurationOld * multcoeff); ;
                                        int test2 = t * note.Duration;

                                        if (test1 != test2)
                                            Console.WriteLine("");

                                        note.Duration = test2;
                                        */

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

                                        if (note.IsDrums)
                                        {
                                            notenumber = note.DrumPitch;
                                        }
                                        else
                                        {
                                            notenumber = 12 + Notes.IndexOf(letter) + 12 * octave;
                                            notenumber += note.ChromaticTranspose;
                                            notenumber += 12 * note.OctaveChange;

                                            if (note.Pitch.Alter != 0)
                                            {
                                                int alter = note.Pitch.Alter;
                                                notenumber += alter;
                                            }

                                        }

                                        // Create note
                                        if (note.Staff <= 1)
                                            CreateMidiNote1(note, notenumber, starttime);
                                        else
                                            CreateMidiNote2(note, notenumber, starttime);
                                        
                                        
                                        if (note.Lyrics.Count > 0 && note.Lyrics[0].Text != null)
                                        {
                                            CreateLyric(note, starttime, versenumber);
                                        }
                                        break;


                                    case MeasureElementType.Forward:
                                        Forward fwd = (Forward)obj;
                                        timeline += (int)(fwd.Duration * multcoeff);
                                        break;

                                }
                            }

                            #endregion Extract all

                        // END RECUP
                        }
                    }
                }


            }

            CreateSequence();

            return sequence;
        }

        // =================================================================================================


        #region Create verses

        /// <summary>
        /// Create the list of verses
        /// </summary>
        /// <param name="partmes"></param>
        /// <returns></returns>
        private List<List<int>> CreateVerses(List<Measure> partmes)
        {
            bool bcondition = true;
            int y;
            int pivot = 0;
            int firstfwd = 0;
            int firstbackward = 0;
            List<int> bloc = new List<int>();
            List<List<int>> mapmeasures = new List<List<int>>();

            int versenumber = 0;
            //bool bReserved = false;
            bool bIncreasePivot = false;
            
            // SIMPLE CASE  ===========================================================
            // no backward/forward => no changes => mapmeasure is the list of measures
            // ========================================================================
            y = GetFirstBackward(pivot, partmes);
            if (y == -1)
            {
                bloc = new List<int>();
                for (int i = 0; i <= partmes.Count - 1; i++)
                {
                    bloc.Add(i);
                }
                mapmeasures.Add(bloc);
                return mapmeasures;
            }


            // COMPLEX CASE ===========================================================
            // blocs backward/forward exist
            // => extract verses
            //    remove measures attached to a single verse
            // ========================================================================
            y = 0;
            Measure mes = new Measure();
                        
            int numloop = 0;            
            int nbLoopMax = 3;
            int firstfwdminimum = 0;

            // 1. Consider first bloc
            // Can be empty or not
            // Can be some measure or repeated measures

            // Get first forward/forward
            firstfwd = GetFirstForwardUp(partmes.Count - 1, partmes);
            firstbackward = GetFirstBackward(0, partmes);
            // Blox exists if firstfwd is at 0

            // A backward exists greater than the forward 
            // this is a reapeat  => take twice (ex imagine)
            if (firstbackward > 0 && firstbackward < firstfwd)
            {
                for (int j = 0; j < 2; j++)
                {
                    bloc = new List<int>();
                    for (int i = 0; i <= firstbackward; i++)
                    {
                        bloc.Add(i);
                    }
                    mapmeasures.Add(bloc);
                }
                pivot = firstbackward + 1;
            }
            else if (firstfwd > 0 && firstfwd < firstbackward)
            {
                // This is a simple bloc => take one (ex cigarette)
                bloc = new List<int>();
                for (int i = 0; i < firstfwd; i++)
                {
                    bloc.Add(i);
                }
                mapmeasures.Add(bloc);
                
                pivot = firstfwd + 1;
            }


            
            // 2. consider a loop with several forward/backward
            while (bcondition)
            {
                // Calculate limits of bloc if the repeats are done
                // 1 Search descending forward from start to less
                firstfwd = GetFirstForward(pivot, partmes);
                if (firstfwd < firstfwdminimum)
                    firstfwd = firstfwdminimum;
                    

                // 2. Search ascending backward from start to more
                y = GetFirstBackward(pivot, partmes);


                // If no more backward starting from "start"
                // Create a verse with all trailing measures and leave
                if (y == -1)
                {
                    #region leave if no more backward
                    versenumber++;
                    bloc = new List<int>();
                    for (int i = firstfwd; i <= partmes.Count - 1; i++)
                    {
                        mes = partmes[i];
                        if (mes.lstVerseNumber.Count == 0 || mes.lstVerseNumber.Contains(versenumber) || mes.lstVerseNumber[0] == 0)
                        //if (mes.lstVerseNumber.Count == 0 || mes.lstVerseNumber.Contains(versenumber))
                        {
                            bloc.Add(i);
                        }
                    }
                    mapmeasures.Add(bloc);
                    break;
                    #endregion leave if no more backward
                }
                

                // Add bloc including measures between FirstForward and FirstBackWard
                versenumber++;                
                bloc = new List<int>();

                for (int i = firstfwd; i <= y; i++)
                {
                    mes = partmes[i];

                    // TODO lyriques réservés et pas réservés

                    // keep only blocs 
                    // - Without lyrics (Count = 0)
                    // - With only one lyric number (all the verses use these single lyrics) (Count = 1)
                    // - Which verse number is contained in the list of verses
                    //if (mes.lstVerseNumber.Count <= 1 || mes.lstVerseNumber.Contains(versenumber))
                    if (mes.lstVerseNumber.Count == 0 || mes.lstVerseNumber.Contains(versenumber)  || mes.lstVerseNumber[0] == 0 )
                    {                        
                        // If last measure of a verse
                        // if the current verse is the last one, it means that we have looped all the verses
                        // put bReserved to true in order to evolve the pivot value
                        if (i == y)
                        {
                            if (mes.lstVerseNumber.Count > 0)
                            {
                                if (versenumber == mes.lstVerseNumber[mes.lstVerseNumber.Count - 1])
                                    bIncreasePivot = true; // bReserved = true;
                            }                           
                        }

                        bloc.Add(i);
                    }
                }                
                mapmeasures.Add(bloc);

                numloop++;

                // Increase pivot value               /
                if (numloop >= nbLoopMax)
                {
                    pivot = y + 1;  
                    numloop = 0;
                    nbLoopMax = 3;

                    // All loops have been done: we do not have to consider previous measures
                    // how can we prevent to calculate again firstfwd ?
                    // this is the forward minimum value to take into account
                    firstfwdminimum = pivot;
                    bIncreasePivot = false; //bReserved = false;
                    
                    
                    // ********************* FAB 31/10/ A CONFIRMER *******************************
                    versenumber = 0;
                } 
                else if (bIncreasePivot) // bReserved)
                {                    
                    pivot = y + 1;
                    bIncreasePivot = false; // bReserved = false;
                    
                }


                #region leave if end of file
                // get last bloc before leaving
                if (pivot > partmes.Count - 1)
                {
                    pivot = partmes.Count - 1;
                    firstfwd = GetFirstForward(pivot, partmes);
                    y = GetFirstBackward(pivot, partmes);
                    
                    // Add bloc
                    bloc = new List<int>();
                    for (int i = firstfwd; i <= y; i++)
                    {
                        mes = partmes[i];
                        //if (mes.lstVerseNumber.Count <= 1 || mes.lstVerseNumber.Contains(versenumber))
                        if (mes.lstVerseNumber.Count == 0 || mes.lstVerseNumber.Contains(versenumber) || mes.lstVerseNumber[0] == 0)
                        {
                            bloc.Add(i);
                        }
                    }
                    mapmeasures.Add(bloc);
                    break;
                }

                #endregion leave if end of file
            }

            return mapmeasures;
        }
       

        /// <summary>
        /// Return first element of type barline/forward - Search descending
        /// </summary>
        /// <param name="start"></param>
        /// <param name="Measures"></param>
        /// <returns></returns>
        private int GetFirstForward(int start, List<Measure> Measures)
        {            
            for (int j = start; j >= 0; j--)
            {
                Measure measure = Measures[j];
                List<MeasureElement> lstME = measure.MeasureElements;
                for (int i = 0; i < lstME.Count; i++)
                {
                    MeasureElement measureElement = lstME[i];
                    if (measureElement.Type == MeasureElementType.Barline)
                    {
                        Barline bl = (Barline)measureElement.Element;
                        if (bl.Direction == RepeatDirections.forward)
                        {
                            return j;
                        }
                    }
                }
            }

            // Return first element if no forward
            return 0;
        }

        private int GetFirstForwardUp(int end, List<Measure> Measures)
        {
            for (int j = 0; j <= end; j++)
            {
                Measure measure = Measures[j];
                List<MeasureElement> lstME = measure.MeasureElements;
                for (int i = 0; i < lstME.Count; i++)
                {
                    MeasureElement measureElement = lstME[i];
                    if (measureElement.Type == MeasureElementType.Barline)
                    {
                        Barline bl = (Barline)measureElement.Element;
                        if (bl.Direction == RepeatDirections.forward)
                        {
                            return j;
                        }
                    }
                }
            }

            // Return first element if no forward
            return -1;
        }


        /// <summary>
        /// Return first element of type barline/backward - Search ascending
        /// </summary>
        /// <param name="start"></param>
        /// <param name="Measures"></param>
        /// <returns></returns>
        private int GetFirstBackward(int start, List<Measure> Measures)
        {                                                            
            for (int j = start; j < Measures.Count; j++)
            {
                Measure measure = Measures[j];
                List<MeasureElement> lstME = measure.MeasureElements;
                for (int i = 0; i < lstME.Count; i++)
                {
                    MeasureElement measureElement = lstME[i];
                    if (measureElement.Type == MeasureElementType.Barline)
                    {
                        Barline bl = (Barline)measureElement.Element;
                        if (bl.Direction == RepeatDirections.backward)
                        {
                            return  j;                            
                        }
                    }
                }
            }

            // Return last element if no backward
            return -1;

        }

        #endregion Create verses


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
           
            if (Numerator > 0 && Denominator > 0) 
                track1.insertTimesignature(Numerator, Denominator);            
            track1.insertTrackname(TrackName);

            if (Volume >  0)
                track1.insertVolume(Channel, Volume);
            
            if (Pan > 0)
                track1.insertPan(Channel, Pan);            

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
            track2.insertTrackname(TrackName);
            track2.insertVolume(Channel, Volume);
            track2.insertPan(Channel, Pan);

            newTracks.Add(track2);
        }

        #endregion tracks


        #region Tempo
        private void CreateTempoEvent(float tmp, int ticks)
        {
            track1.insertTempo((int)tmp, ticks);
        }
        #endregion Tempo


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
        private void CreateLyric(Note n, int t, int versenumber)
        {
            try
            {
                //bool bCutPossible = false;
                bool blineFeed = false;                
                byte[] newdata;

             
                // Search the Lyric coresponding to the right verse
                // We add a special character at the begining of the lyric when we use it
                // next time, the next lyric will be used
                Lyric lyric = new Lyric();

                
                bool bfound = false;
                if (n.Lyrics.Count > 1)
                {
                    // Search with current versenumber
                    foreach (Lyric ll in n.Lyrics)
                    {
                        if (ll.VerseNumber == versenumber && ll.Text != "")
                        {
                            lyric = ll;
                            bfound = true;
                            break;
                        }
                    }
                    
                    if (!bfound)
                    {
                        return;
                    }
                }
                else if (n.Lyrics.Count == 1)
                {
                    // If only one line of lyrics => take it
                    lyric = n.Lyrics[0];
                }
                else
                {
                    return;
                }

                string currentElement = lyric.Text;
                
                if (lyric.Text == null || lyric.Text.Trim().Length == 0)
                    return;

                // check if linefeed present before trimming it (This will remove the '\n' character !!!!)
                if (currentElement.IndexOf("\n") > -1)                
                {                    
                    blineFeed = true;
                }
                // Trim first, and after add spaces occording to "Single" and "End" syllabic 
                currentElement = currentElement.Trim();  
                

                switch (lyric.Syllabic)
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

               
                // Text encoding
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
                
                // Message
                MetaMessage mtMsg;
                // si lyrics de type lyrics
                mtMsg = new MetaMessage(MetaType.Lyric, newdata);
                           
                // Update Track.Lyrics List
                Track.Lyric L = new Track.Lyric()
                {
                    Element = currentElement,
                    TicksOn = t,
                    Type = Track.Lyric.Types.Text,
                };
                                
                // Insert new message in track
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

                // ======================
                // Manage linefeeds
                // ======================
                if (blineFeed)
                {
                    currentElement = "¼";

                    // Text encoding
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

                    mtMsg = new MetaMessage(MetaType.Lyric, newdata);

                    L = new Track.Lyric()
                    {
                        Element = currentElement,
                        TicksOn = t,
                        Type = Track.Lyric.Types.LineFeed,
                    };

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
                sequence.tracks[0].insertTempo(Tempo, 0);

            // Tags to sequence
            //sequence.CloneTags();
        }

        #endregion sequence

    }
}
