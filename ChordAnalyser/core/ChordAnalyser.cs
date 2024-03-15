#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;
using ChordAnalyser;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics.Eventing.Reader;

namespace ChordsAnalyser
{
    public class ChordAnalyser
    {
        // Fabrice Method
        Analyser Analyser = new Analyser();

        //cchords.chords ch = new cchords.chords();
        static List<MidiNote[]> lnMidiNote = new List<MidiNote[]>();
        static List<int[]> lnIntNote = new List<int[]>();
        static List<string[]> lnStringNote = new List<string[]>();

        #region properties

        public Dictionary<int, (string, string)> Gridchords { get; set; }
        
        // New search
        public Dictionary<int, List<string>> GridBeatChords { get; set; }
        private Dictionary<int, List<int>> dictnotes = new Dictionary<int, List<int>>();

        #endregion properties

        #region private
        private Sequence sequence1 = new Sequence();        
        
        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;

        private string NoChord = "<Chord not found>";
        private List<string> LstNoChords = new List<string>() { "<Chord not found>" };
        private string EmptyChord = "<Empty>";
        private List<string> LstEmptyChords = new List<string>() { "<Empty>" };

        #endregion private

        public ChordAnalyser() { }

        //public ChordAnalyser(SheetMusic sheetmus, Sequence seq)
        public ChordAnalyser(Sequence seq)
        {
            sequence1 = seq;
            //sheetmusic = sheetmus;

            UpdateMidiTimes();

            // Dictionary :
            // int = measure number
            // string : chord
            // 2 chords per measure
            Gridchords = new Dictionary<int, (string,string)>(NbMeasures);

            for (int i = 1; i <= NbMeasures; i++)
            {
                Gridchords[i] = (NoChord, NoChord);
            }

            SearchMethod2();

                        
        }

        
        private void SearchMethod()
        {
            // Search notes in measures
            SearchByHalfMeasure();

            // if search notes fails, take the bass line
            SearchByBass();

            // display results
            PublishResults(Gridchords);

        }


        private void SearchMethod2()
        {
            SearchByBeat();
        }

        #region variante

        private void SearchByBeat()
        {
            int nbBeatsPerMeasure = sequence1.Numerator;
            int measure;
            int beat;
            int timeinmeasure;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int beats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);

            // init dictionary
            GridBeatChords = new Dictionary<int, List<string>>();
            for (int i = 1; i <= beats; i++)
            {
                dictnotes[i] = new List<int>();
                GridBeatChords[i] = null;
            }

            //Search notes
            foreach (Track track in sequence1)
            {
                if (track.ContainsNotes && track.MidiChannel != 9)
                {
                    foreach (MidiNote note in track.Notes)
                    {
                        measure = DetermineMeasure(note.StartTime);
                        timeinmeasure = (int)GetTimeInMeasure(note.StartTime);
                        beat = 1 + (measure - 1) * nbBeatsPerMeasure + timeinmeasure;
                        dictnotes[beat].Add(note.Number);
                    }
                }
            }  
            
            for (int ibeat = 1; ibeat <= beats; ibeat++)
            {
                SearchBeat(ibeat);
            } 

        }

        private void SearchBeat(int beat)
        {
            if (dictnotes[beat].Count == 0)
            {
                if (GridBeatChords[beat] == null)
                {
                    GridBeatChords[beat] = new List<string> { EmptyChord };
                }
            }
            else
            {
                List<string> notletters = TransposeToLetterChord(dictnotes[beat]);
                List<int> lstInt = new List<int>();

                // Remove doubles
                notletters = notletters.Distinct().ToList();

                // Remove impossible chords
                notletters = CheckImpossibleChordString(notletters);

                // Translate strings to int
                for (int i = 0; i < notletters.Count; i++)
                {
                    string n = notletters[i];
                    string n0 = n.Substring(0, 1);
                    Dictionary<string, int> _note_dict = new Dictionary<string, int>() { { "C", 0 }, { "D", 2 }, { "E", 4 }, { "F", 5 }, { "G", 7 }, { "A", 9 }, { "B", 11 } };
                    int x = _note_dict[n0];
                    if (n.Length > 1)
                    {
                        if (n.Substring(1, 1) == "#")
                        {
                            x += 1;
                        }
                        else
                        {
                            x -= 1;
                            if (x < 0)  // Cb = B
                                x = 11;
                        }
                    }

                    lstInt.Add(x);
                }

                // Store into table
                int[] intNotes = new int[lstInt.Count];
                for (int i = 0; i < lstInt.Count; i++)
                    intNotes[i] = lstInt[i];

                lnIntNote = new List<int[]>();
                // Build ln = list of all combinations of int
                PermuteIntNote(intNotes, 0, lstInt.Count - 1);


                // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                List<List<int>> llnotes = new List<List<int>>();
                foreach (int[] arry in lnIntNote)
                {
                    List<int> lll = new List<int>();
                    for (int i = 0; i < arry.Length; i++)
                    {
                        lll.Add(arry[i]);
                    }
                    llnotes.Add(lll);
                }

                llnotes = ChangeListListNotesNumber(llnotes);


                // Search major or minor triads note                    
                List<int> lroot = DetermineRoot(llnotes);

                List<List<List<int>>> lroots = DetermineRoots(llnotes);

                if (lroot != null)
                {
                    string res = Analyser.determine(lroot);

                    if (GridBeatChords[beat] == null)
                    {
                        GridBeatChords[beat] = new List<string> { res };
                    }
                    else
                    {
                        GridBeatChords[beat].Add(res);
                    }
                }

            }
        }
        #endregion variante



        #region Search method

        /// <summary>
        /// HArvest notes
        /// </summary>
        private void SearchByHalfMeasure()
        {
            // Collect all notes of all tracks for each measure and try to fing a chord
            for (int _measure = 1; _measure <= NbMeasures; _measure++)
            {                
                // Create a list only for permutations                
                List<int> lstfirstmidiNotes = new List<int>();
                List<int> lstSecmidiNotes = new List<int>();

                #region harvest notes per measure
                // Harvest notes on each measure
                foreach (Track track in sequence1)
                {
                    if (track.ContainsNotes && track.MidiChannel != 9)
                    {
                        foreach (MidiNote note in track.Notes)
                        {
                            int Measure = DetermineMeasure(note.StartTime);

                            if (Measure > _measure)
                                break;

                            if (Measure == _measure)
                            {
                                float st = GetTimeInMeasure(note.StartTime);

                                if (st < sequence1.Denominator / 2)
                                {
                                    // add note to first part of the measure
                                    lstfirstmidiNotes.Add(note.Number);
                                }
                                else
                                {
                                    // Add note to second part of the measure
                                    lstSecmidiNotes.Add(note.Number);
                                }
                            }
                        }
                    }
                }
                #endregion harvest notes per measure


                #region search
 
                SearchMeasure(_measure, 1, lstfirstmidiNotes);

                SearchMeasure(_measure, 2, lstSecmidiNotes);

                #endregion search
            }
        }

      
        /// <summary>
        /// Search chords in each measure
        /// </summary>
        /// <param name="_measure"></param>
        /// <param name="section"></param>
        /// <param name="notes"></param>
        private void SearchMeasure(int _measure, int section,  List<int> notes)
        {
            if (notes.Count == 0)
            {
                if (section == 1)
                {
                    if (Gridchords[_measure].Item1 == NoChord)
                        Gridchords[_measure] = (EmptyChord, Gridchords[_measure].Item2);
                }
                else if (section == 2)
                {
                    if (Gridchords[_measure].Item2 == NoChord)
                        Gridchords[_measure] = (Gridchords[_measure].Item1, EmptyChord);
                }
            }
            else
            {
                List<string> notletters = TransposeToLetterChord(notes);
                List<int> lstInt = new List<int>();

                // Remove doubles
                notletters = notletters.Distinct().ToList();

                // Remove impossible chords
                notletters = CheckImpossibleChordString(notletters);

                // Translate strings to int
                for (int i = 0; i < notletters.Count; i++)
                {
                    string n = notletters[i];
                    string n0 = n.Substring(0,1);                    
                    Dictionary<string, int> _note_dict = new Dictionary<string, int>() { { "C", 0 }, { "D", 2 }, { "E", 4 }, { "F", 5 }, { "G", 7 }, { "A", 9 }, { "B", 11 } };
                    int x = _note_dict[n0];
                    if (n.Length > 1)
                    {
                        if (n.Substring(1, 1) == "#")
                        {
                            x += 1;
                        }
                        else
                        {
                            x -= 1;
                            if (x < 0)  // Cb = B
                                x = 11;
                        }
                    }

                    lstInt.Add(x);
                }

                // Store into table
                int[] intNotes = new int[lstInt.Count];
                for (int i = 0; i < lstInt.Count; i++)
                    intNotes[i] = lstInt[i];

                lnIntNote = new List<int[]>();
                // Build ln = list of all combinations of int
                PermuteIntNote(intNotes, 0, lstInt.Count - 1);
                

                // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                List<List<int>> llnotes = new List<List<int>>();
                foreach (int[] arry in lnIntNote)
                {
                    List<int> lll = new List<int>();
                    for (int i = 0; i < arry.Length; i++)
                    {
                        lll.Add(arry[i]);
                    }
                    llnotes.Add(lll);
                }

                llnotes = ChangeListListNotesNumber(llnotes);


                // Search major or minor triads note                    
                List<int> lroot = DetermineRoot(llnotes);

                if (lroot != null)
                {
                    string res = Analyser.determine(lroot);
                    if (section == 1)
                    {
                        if (Gridchords[_measure].Item1 == NoChord)
                            Gridchords[_measure] = (res, Gridchords[_measure].Item2);
                    }
                    else if (section == 2)
                    {
                        if (Gridchords[_measure].Item2 == NoChord)
                            Gridchords[_measure] = (Gridchords[_measure].Item1, res);

                    }
                }                
            }
        }

        private void SearchByBass()
        {
            // Collect all notes of all tracks for each measure and try to fing a chord
            for (int _measure = 1; _measure <= NbMeasures; _measure++)
            {
                // Create a list only for permutations                
                List<int> lstfirstmidiNotes = new List<int>();
                List<int> lstSecmidiNotes = new List<int>();

                #region harvest notes per measure
                // Search bass track
                foreach (Track track in sequence1)
                {
                    // Consider only bass tracks
                    if (32 <= track.ProgramChange && track.ProgramChange <= 39 && track.ContainsNotes)
                    {
                        foreach (MidiNote note in track.Notes)
                        {
                            int Measure = DetermineMeasure(note.StartTime);

                            if (Measure > _measure)
                                break;

                            if (Measure == _measure)
                            {
                                float st = GetTimeInMeasure(note.StartTime);

                                if (st < sequence1.Denominator / 2)
                                {
                                    // add note to first part of the measure
                                    lstfirstmidiNotes.Add(note.Number);
                                }
                                else
                                {
                                    // Add note to second part of the measure
                                    lstSecmidiNotes.Add(note.Number);
                                }
                            }
                        }
                    }

                }

                #endregion harvest notes per measure

                #region search

                SearchMeasureBass(_measure, 1, lstfirstmidiNotes);

                SearchMeasureBass(_measure, 2, lstSecmidiNotes);

                #endregion search

            }
        }

        private void SearchMeasureBass(int _measure, int section, List<int> notes)
        {
            if (notes.Count == 0)
                return;

            List<string> notletters = TransposeToLetterChord(notes);
            List<int> lstInt = new List<int>();

            // Remove doubles
            notletters = notletters.Distinct().ToList();

            // Remove impossible chords
            notletters = CheckImpossibleChordString(notletters);

            // Translate strings to int
            for (int i = 0; i < notletters.Count; i++)
            {
                string n = notletters[i];
                string n0 = n.Substring(0, 1);
                Dictionary<string, int> _note_dict = new Dictionary<string, int>() { { "C", 0 }, { "D", 2 }, { "E", 4 }, { "F", 5 }, { "G", 7 }, { "A", 9 }, { "B", 11 } };
                int x = _note_dict[n0];
                if (n.Length > 1)
                {
                    if (n.Substring(1, 1) == "#")
                    {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                        if (x < 0)  // Cb = B
                            x = 11;
                    }
                }

                lstInt.Add(x);
            }

            // Store into table
            int[] intNotes = new int[lstInt.Count];
            for (int i = 0; i < lstInt.Count; i++)
                intNotes[i] = lstInt[i];

            lnIntNote = new List<int[]>();
            // Build ln = list of all combinations of int
            PermuteIntNote(intNotes, 0, lstInt.Count - 1);

            // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
            List<List<int>> llnotes = new List<List<int>>();
            foreach (int[] arry in lnIntNote)
            {
                List<int> lll = new List<int>();
                for (int i = 0; i < arry.Length; i++)
                {
                    lll.Add(arry[i]);
                }
                llnotes.Add(lll);
            }

            llnotes = ChangeListListNotesNumber(llnotes);

            // Search major or minor triads note                    
            List<int> lroot = DetermineRoot(llnotes);

            if (lroot != null)
            {
                string res = Analyser.determine(lroot);
                if (section == 1)
                {
                    if (Gridchords[_measure].Item1 == NoChord)
                        Gridchords[_measure] = (res, Gridchords[_measure].Item2);
                }
                else if (section == 2)
                {
                    if (Gridchords[_measure].Item2 == NoChord)
                        Gridchords[_measure] = (Gridchords[_measure].Item1, res);

                }
            }
            else
            {
                string res = Analyser.determine(llnotes[0]);
                if (res != null)
                {
                    if (section == 1)
                    {
                        if (Gridchords[_measure].Item1 == NoChord)
                            Gridchords[_measure] = (res, Gridchords[_measure].Item2);
                    }
                    else if (section == 2)
                    {
                        if (Gridchords[_measure].Item2 == NoChord)
                            Gridchords[_measure] = (Gridchords[_measure].Item1, res);

                    }
                }
            }


        }
       
        #endregion Search method


        private List<int> CheckImpossibleChordInt(List<int> lstInt)
        {
            List<string> notletters = TransposeToLetterChord(lstInt);
            List<string> ress = new List<string>();
            foreach (string s in notletters)
                ress.Add(s);


            List<int> res = new List<int>();
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            foreach (string s in notletters)
            {                
                if (s.Length == 1)                
                    if (notletters.Contains(s + "#"))                     
                        ress.Remove(s + "#");                                     
            }

            for (int i = 0; i < ress.Count; i++)
            {
                res.Add(letters.IndexOf(ress[i]));
            }

            return res;
        }

        private List<string> CheckImpossibleChordString(List<string> lstString)
        {
            List<string> res = new List<string>();
            foreach (string s in lstString)
                res.Add(s);

            foreach (string s in lstString) 
            {
                if (s.Length == 1)
                {
                    if (lstString.Contains(s + "#"))
                    {
                        while (res.Contains(s + "#"))
                            res.Remove(s + "#");
                    }
                }
            }
            return res;

        }

        private void PublishResults(Dictionary<int, (string, string)> dict)
        {
            
            foreach (KeyValuePair<int, (string, string)> pair in dict)
            {
                Console.WriteLine(string.Format("{0} - {1}", pair.Key, pair.Value));
            }            
        }


        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;            
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;                
                NbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer 
            }
        }


        /// <summary>
        /// In which measure is the chord
        /// </summary>
        /// <param name="chord"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private int DetermineMeasure(int ticks)
        {
            return 1 + ticks / _measurelen;            
        }


        /// <summary>
        /// Get time inside measure
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public float GetTimeInMeasure(int ticks)
        {
            // Num measure
            int curmeasure = 1 + ticks / _measurelen;
            // Temps dans la mesure
            float timeinmeasure = sequence1.Numerator - ((curmeasure * _measurelen - ticks) / (float)(_measurelen / sequence1.Numerator));
            
            return timeinmeasure;                       
        }

        /// <summary>
        /// Remove a note if in double inside a chord
        /// If two C exists, remove one
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<List<int>> RemoveDoubles(List<List<int>> lschords)
        {
            List<List<int>> res = new List<List<int>>();
            int n = 0;
            for (int j = 0; j < lschords.Count; j++)
            {
                List<int> lsnotes = lschords[j];
                for (int i = 0; i < lsnotes.Count; i++)                
                    lsnotes[i] = lsnotes[i] % 12;
                
                lsnotes = lsnotes.Distinct().ToList();
                res.Add(lsnotes);
            }

            return res;
        }

        /// <summary>
        /// Retrieve notes letter for a chord
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<string> TransposeToLetterChord(List<int> notes)
        {
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};            
            List<string> letternotes = new List<string>();            
            string l = string.Empty;
            int x = 0;
            foreach (int n in notes)
            {                
                x = n % 12;
                l = letters[x];
                letternotes.Add(l);
            }            
            return letternotes;
        }



        #region permutations

        /// <summary>
        /// Get all combinations of a set of values
        /// </summary>
        /// <param name="arry"></param>
        /// <param name="i"></param>
        /// <param name="n"></param>
        static void PermuteMidiNote(MidiNote[] arry, int i, int n)
        {            
            int j;
            if (i == n)
            {                
                MidiNote[] m = new MidiNote[arry.Count()];
                for (int x = 0; x < arry.Count();x++)
                    m[x] = arry[x];
                lnMidiNote.Add(m);
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    SwapMidiNote(ref arry[i], ref arry[j]);
                    PermuteMidiNote(arry, i + 1, n);
                    SwapMidiNote(ref arry[i], ref arry[j]); //backtrack
                }
            }            
        }

        static void SwapMidiNote(ref MidiNote a, ref MidiNote b)
        {
            MidiNote tmp;
            tmp = a;
            a = b;
            b = tmp;
        }


        static void PermuteIntNote(int[] arry, int i, int n)
        {
            int j;
            if (i == n)
            {
                int[] m = new int[arry.Count()];
                for (int x = 0; x < arry.Count(); x++)
                    m[x] = arry[x];
                lnIntNote.Add(m);
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    SwapIntNote(ref arry[i], ref arry[j]);
                    PermuteIntNote(arry, i + 1, n);
                    SwapIntNote(ref arry[i], ref arry[j]); //backtrack
                }
            }
        }

        static void SwapIntNote(ref int a, ref int b)
        {
            int tmp;
            tmp = a;
            a = b;
            b = tmp;
        }


        static void PermuteStringNote(string[] arry, int i, int n)
        {
            int j;
            if (i == n)
            {
                string[] m = new string[arry.Count()];
                for (int x = 0; x < arry.Count(); x++)
                    m[x] = arry[x];
                lnStringNote.Add(m);
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    SwapStringNote(ref arry[i], ref arry[j]);
                    PermuteStringNote(arry, i + 1, n);
                    SwapStringNote(ref arry[i], ref arry[j]); //backtrack
                }
            }
        }
        static void SwapStringNote(ref string a, ref string b)
        {
            string tmp;
            tmp = a;
            a = b;
            b = tmp;
        }

        #endregion permutations


        /// <summary>
        /// Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
        /// </summary>
        /// <param name="ll"></param>
        /// <returns></returns>
        private List<List<int>> ChangeListListNotesNumber(List<List<int>> ll)
        {            
            int n;
            int prevnumber = 0;
            List<List<int>> res = new List<List<int>>();

            for (int j = 0; j < ll.Count; j++)          
            {               
                List<int> lsnotes = ll[j];                
                int t = lsnotes[0];
                t = t % 12;
                lsnotes[0] = t;
                prevnumber = t;

                for (int i = 1; i < lsnotes.Count; i++)
                {
                    n = lsnotes[i] % 12;
                    if (n < prevnumber)
                    {
                        while (n < prevnumber)
                            n += 12;
                    }
                    lsnotes[i] = n;
                    prevnumber = n;
                }
                res.Add(lsnotes);
            }
            return res;
        }


        /// <summary>
        /// Select the chord existing in the proposed list
        /// </summary>
        /// <param name="lsnotes"></param>
        /// <returns></returns>
        private List<int> DetermineRoot(List<List<int>> lsnotes)
        {
            /* {0,1,2} => 1 - 0, 2 - 0 ET {0,2,1} ????
             * {1,2,0} => 2 - 1, 0 - 1 ET {1,0,2}
             * {2,0,1} => 0 - 2, 1 - 2
             * 
             */
            
            foreach (List<int> chord in lsnotes)
            {
                if (chord.Count > 3)
                {
                    if (IsMajorChord7(chord) || IsMinorChord7(chord))
                        return chord;
                }
            }
            

            foreach (List<int> chord in lsnotes)
            {                
                if (chord.Count > 2)
                {
                    if (IsMajorChord(chord) || IsMinorChord(chord))
                        return chord;
                }
            }
            

            return null;  
        }

        private List<List<List<int>>> DetermineRoots(List<List<int>> lsnotes)
        {
            List<List<int>> lstMajorChords = new List<List<int>>();
            List<List<int>> lstMinorChords = new List<List<int>>();
            List<List<int>> lstMajorChords7 = new List<List<int>>();
            List<List<int>> lstMinorChords7 = new List<List<int>>();

            List<List<List<int>>> res = new List<List<List<int>>>();

            foreach (List<int> chord in lsnotes)
            {
                if (chord.Count > 3)
                {
                    if (IsMajorChord7(chord)) 
                    {
                        lstMajorChords7.Add(chord);
                    }
                    else if (IsMinorChord7(chord)) 
                    {
                        lstMinorChords7.Add(chord);
                    }                        
                }

                if (chord.Count > 2)
                {
                    if (IsMajorChord(chord))
                    {
                        lstMajorChords.Add(chord);
                    }
                    else if (IsMinorChord(chord))
                    {
                        lstMinorChords.Add(chord);
                    }
                }
            }

            res.Add(lstMajorChords7);
            res.Add(lstMinorChords7);
            res.Add(lstMajorChords);
            res.Add(lstMinorChords);

            return res;
        }

        List<MidiNote> Rotate(List<MidiNote> notes)
        {
            MidiNote first = notes[0];
            notes.RemoveAt(0);
            notes.Add(first);

            return notes;

        }

        /// <summary>
        /// Sort Midi notes by number
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<MidiNote> SortNotes(List<MidiNote> notes)
        {
            List<MidiNote> l = new List<MidiNote>();
            var res = from n in notes
                        orderby n.Number
                        ascending
                        select n;
            foreach (var x in res)
            {
                l.Add(x);
            }
            return l;
        }

        static bool IsMajorChord(List<int> notes)
        {
            // Un coup ça marche sans % 12, un coup avec
            // Si les number des notes sont dans le bon ordre, c'est bon 
            if (notes.Count < 3) return false;
            
            // A major chord consists of the root, major third, and perfect fifth
            return (notes[1] - notes[0] == 4) && (notes[2] - notes[0] == 7);
        }

        static bool IsMinorChord(List<int> notes)
        {
            if (notes.Count <3) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1] - notes[0] == 3) && (notes[2] - notes[0] == 7);
        }

        static bool IsMajorChord7(List<int> notes)
        {
            // Un coup ça marche sans % 12, un coup avec
            // Si les number des notes sont dans le bon ordre, c'est bon 
            if (notes.Count < 4) return false;

            // A major chord consists of the root, major third, and perfect fifth
            return (notes[1] - notes[0] == 4) && (notes[2] - notes[0] == 7) && (notes[3] - notes[0] == 10);
        }

        static bool IsMinorChord7(List<int> notes)
        {
            if (notes.Count < 4) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1] - notes[0] == 3) && (notes[2] - notes[0] == 7) && (notes[3] - notes[0] == 10);
        }

    }
}
