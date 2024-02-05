#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;
using ChordsAnalyser.cchords;
using ChordAnalyser.UI;

namespace ChordsAnalyser
{
    public class ChordAnalyser
    {

        cchords.chords ch = new cchords.chords();
        static List<MidiNote[]> lnMidiNote = new List<MidiNote[]>();
        static List<int[]> lnIntNote = new List<int[]>();


        #region properties

        //private static Dictionary<int, (string, string)> Gridchords;
        public Dictionary<int, (string, string)> Gridchords { get; set; }

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
        private string EmptyChord = "<Empty>";

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
                Gridchords[i] = (NoChord, NoChord);

            //SearchByChord();

            
            // 1. Search by notes
            SearchByNotes();

            // if unsuccessfull above, seach by the bass line
            SeartchByBass();


            // display resluts
            PublishResults(Gridchords);      
                        
        }

        /*
        private void SearchByChord()
        {
            // For each track containing chords
            foreach (List<ChordSymbol> chords in sheetmusic.lstChords)
            {
                if (chords.Count > 0)
                {
                    int x = 0;
                    foreach (ChordSymbol chord in chords)
                    {
                        if (chord.Notes.Count >= 3)
                        {
                            x++;

                            int _measure = DetermineMeasure(chord.StartTime);

                            //Remove doubles
                            List<int> lstInt = new List<int>();
                            for (int i = 0; i < chord.Notes.Count; i++)
                            {
                                int n = chord.Notes[i].Number % 12;
                                lstInt.Add(n);
                            }
                            lstInt = lstInt.Distinct().ToList();


                            if (lstInt.Count > 2)
                            {
                                // Debug
                                List<string> notletters = TransposeToLetterChord(lstInt);

                                // Store into table
                                int[] intNotes = new int[lstInt.Count];
                                for (int i = 0; i < lstInt.Count; i++)
                                    intNotes[i] = lstInt[i];

                                lnIntNote = new List<int[]>();
                                // Build ln = list of all combinations of int
                                PermuteIntNote(intNotes, 0, lstInt.Count - 1);

                                // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                                List<List<int>> notes = new List<List<int>>();
                                foreach (int[] arry in lnIntNote)
                                {
                                    List<int> lll = new List<int>();
                                    for (int i = 0; i < arry.Length; i++)
                                    {
                                        lll.Add(arry[i]);
                                    }
                                    notes.Add(lll);
                                }

                                notes = ChangeListListNotesNumber(notes);

                                // Search root note                    
                                List<int> lroot = DetermineRoot(notes);

                                if (lroot != null)
                                {
                                    // Transpose to letters
                                    List<string> notesletters = TransposeToLetterChord(lroot);

                                    List<string> res = ch.determine(notesletters);

                                    if (res.Count > 0)
                                    {
                                        float st = GetTimeInMeasure(chord.StartTime);

                                        if (st < sequence1.Denominator / 2)
                                        {
                                            if (Gridchords[_measure].Item1 == NoChord)
                                                Gridchords[_measure] = (res[0], Gridchords[_measure].Item2);
                                        }
                                        else
                                        {
                                            if (Gridchords[_measure].Item2 == NoChord)
                                                Gridchords[_measure] = (Gridchords[_measure].Item1, res[0]);
                                        }
                                    }
                                    else
                                        Console.WriteLine(string.Format("{0} - [Search by chord] ERREUR : Determine n'a pas trouvé d'accord", _measure));
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("{0} - [Search by chord] no root note found", _measure));
                                }

                            }
                            else 
                            { 
                                Console.WriteLine(string.Format("{0} - [Search by chord] number of notes < 2",_measure)); 
                            }
                          

                        }

                    }

                }
            }
        }
        */


        private void SeartchByBass()
        {
            // Search bass track
            foreach (Track track in sequence1)
            {
                if (track.ContainsNotes)
                {
                    // Consider only bass tracks
                    if (32 <= track.ProgramChange && track.ProgramChange <= 39)
                    {
                        //Console.WriteLine("Name: " + track.Name + " - Instrument: " + track.InstrumentName);

                        List<string> lsfirstnotes = new List<string>();
                        List<string> lsSecnotes = new List<string>();

                        int prevmeasure = 1;

                        foreach (MidiNote note in track.Notes)
                        {
                            float st = GetTimeInMeasure(note.StartTime);

                            // For the moment maximum is 2 chords per measure
                            // one chord in first half
                            // Second chord in second half

                            int Measure = DetermineMeasure(note.StartTime);
                            if (Measure != prevmeasure)
                            {
                                // Store results from previous measure

                                // Remove doubles
                                lsfirstnotes = lsfirstnotes.Distinct().ToList();
                                lsSecnotes = lsSecnotes.Distinct().ToList();

                                if (st < sequence1.Denominator / 2)
                                {
                                    #region first part of measure
                                    if (0 < lsfirstnotes.Count && lsfirstnotes.Count <= 2)
                                    {
                                        if (Gridchords[prevmeasure].Item1 == NoChord)
                                            Gridchords[prevmeasure] = (lsfirstnotes[0], Gridchords[prevmeasure].Item2);
                                    }
                                    else if (lsfirstnotes.Count > 2)
                                    {
                                        List<string> res = ch.determine(lsfirstnotes);
                                        if (res.Count > 0)
                                        {                                            
                                            if (Gridchords[prevmeasure].Item1 == NoChord)
                                                Gridchords[prevmeasure] = (res[0], Gridchords[prevmeasure].Item2);                                            
                                        }
                                        else
                                        {
                                            Console.WriteLine(string.Format("[Search by bass] ERREUR : Determine n'a pas trouvé d'accord mesure {0}", prevmeasure));
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 2nd part of measure
                                    if (0 < lsSecnotes.Count && lsSecnotes.Count <= 2)
                                    {
                                        if (Gridchords[prevmeasure].Item2 == NoChord)
                                            Gridchords[prevmeasure] = (Gridchords[prevmeasure].Item1, lsSecnotes[0]);

                                    }
                                    else if (lsSecnotes.Count > 2)
                                    {
                                        List<string> res = ch.determine(lsSecnotes);
                                        if (res.Count > 0)
                                        {
                                            if (Gridchords[prevmeasure].Item2 == NoChord)
                                                Gridchords[prevmeasure] = (Gridchords[prevmeasure].Item1, res[0]);
                                        }
                                        else
                                        {
                                            Console.WriteLine(string.Format("[Search by bass] ERREUR : Determine n'a pas trouvé d'accord mesure {0}", prevmeasure));
                                        }
                                    }
                                    #endregion
                                }

                                prevmeasure = Measure;
                                lsfirstnotes = new List<string>();
                                lsSecnotes = new List<string>();
                            }

                            // first half of measure
                            if (st < sequence1.Denominator / 2)
                                lsfirstnotes.Add(TransposeToLetterNote(note.Number));
                            else
                            {
                                // Second half of measure
                                lsSecnotes.Add(TransposeToLetterNote(note.Number));
                            }
                        }

                        // Do not forget Last measure
                        #region last measure

                        // First part of last measure
                        lsfirstnotes = lsfirstnotes.Distinct().ToList();
                        if (0 < lsfirstnotes.Count && lsfirstnotes.Count <= 2)
                        {
                            if (Gridchords[NbMeasures].Item1 == NoChord)
                                Gridchords[NbMeasures] = (lsfirstnotes[0], Gridchords[NbMeasures].Item2);
                        }
                        else if (lsfirstnotes.Count > 2)
                        {
                            List<string> res = ch.determine(lsfirstnotes);
                            if (res.Count > 0)
                            {
                                if (Gridchords[NbMeasures].Item1 == NoChord)
                                    Gridchords[NbMeasures] = (res[0], Gridchords[NbMeasures].Item2);
                            }
                            else
                            {
                                Console.WriteLine(string.Format("[Search by bass] ERREUR : Determine n'a pas trouvé d'accord mesure {0}", NbMeasures));                             
                            }
                        }

                        // 2nd part of last measure
                        lsSecnotes = lsSecnotes.Distinct().ToList();
                        if (0 < lsSecnotes.Count && lsSecnotes.Count <= 2)
                        {
                            if (Gridchords[NbMeasures].Item2 == NoChord)
                                Gridchords[NbMeasures] = (Gridchords[NbMeasures].Item1, lsSecnotes[0]);
                        }
                        else if (lsSecnotes.Count > 2)
                        {
                            List<string> res = ch.determine(lsSecnotes);
                            if (res.Count > 0)
                            {
                                if (Gridchords[NbMeasures].Item2 == NoChord)
                                    Gridchords[NbMeasures] = (Gridchords[NbMeasures].Item1, res[0]);
                            }
                            else
                            {
                                Console.WriteLine(string.Format("[Search by bass] ERREUR : Determine n'a pas trouvé d'accord mesure {0}", NbMeasures));
                            }
                        }

                        #endregion last measure

                    } // Check bass track
                } // TRack contains notes
            }
        }

        private void SearchByNotes()
        {
            // Collect all notes of all tracks for each measure and try to fing a chord
            for (int _measure = 1; _measure <= NbMeasures; _measure++)
            {
                //List<string> lsnotes = new List<string>();

                // Create a list only for permutations
                List<MidiNote> lstfirstmidiNotes = new List<MidiNote> ();
                List<MidiNote> lstSecmidiNotes = new List<MidiNote>();

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
                                    //lsnotes.Add(TransposeToLetterNote(note.Number));
                                    lstfirstmidiNotes.Add(note);
                                }
                                else
                                {
                                    lstSecmidiNotes.Add(note);
                                }
                            }
                        }
                    }
                }

                #region first part of measure
                if (lstfirstmidiNotes.Count == 0)
                {
                    if (Gridchords[_measure].Item1 == NoChord)
                        Gridchords[_measure] = (EmptyChord, Gridchords[_measure].Item2);
                }
                else
                {
                    //Remove doubles
                    List<int> lstInt = new List<int>();
                    for (int i = 0; i < lstfirstmidiNotes.Count; i++)
                    {
                        int n = lstfirstmidiNotes[i].Number % 12;
                        lstInt.Add(n);
                    }
                    lstInt = lstInt.Distinct().ToList();

                    // Check impossible chords
                    // C, E, D, D#, A
                    lstInt = CheckImpossibleChord(lstInt);

                    if (lstInt.Count > 2)
                    {
                        // Debug
                        List<string> notletters = TransposeToLetterChord(lstInt);

                        // Store into table
                        int[] intNotes = new int[lstInt.Count];
                        for (int i = 0; i < lstInt.Count; i++)
                            intNotes[i] = lstInt[i];

                        lnIntNote = new List<int[]>();
                        // Build ln = list of all combinations of int
                        PermuteIntNote(intNotes, 0, lstInt.Count - 1);

                        // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                        List<List<int>> notes = new List<List<int>>();
                        foreach (int[] arry in lnIntNote)
                        {
                            List<int> lll = new List<int>();
                            for (int i = 0; i < arry.Length; i++)
                            {
                                lll.Add(arry[i]);
                            }
                            notes.Add(lll);
                        }

                        notes = ChangeListListNotesNumber(notes);

                        // Search root note                    
                        List<int> lroot = DetermineRoot(notes);

                        if (lroot != null)
                        {
                            // Transpose to letters
                            List<string> notesletters = TransposeToLetterChordSpecial(lroot);
                            //List<string> res = ch.determine(notesletters,true);
                            List<string> res = ch.determine(notesletters);

                            if (res.Count > 0)
                            {
                                if (Gridchords[_measure].Item1 == NoChord)
                                    Gridchords[_measure] = (res[0], Gridchords[_measure].Item2);
                            }
                            else
                            {
                                lroot = new List<int> { lroot[0], lroot[1], lroot[2] };
                                // Transpose to letters
                                notesletters = TransposeToLetterChord(lroot);
                                res = ch.determine(notesletters);
                                if (res.Count > 0)
                                {
                                    if (Gridchords[_measure].Item1 == NoChord)
                                        Gridchords[_measure] = (res[0], Gridchords[_measure].Item2);
                                }
                                else
                                    Console.WriteLine(string.Format("{0} - [Search by chord] ERREUR : Determine n'a pas trouvé d'accord", _measure));
                            }
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} - [Search by chord] no root note found", _measure));
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} - [Search by chord] number of notes < 2", _measure));
                    }

                }
                #endregion first part of measure


                #region 2nd part of measure
                if (lstSecmidiNotes.Count == 0)
                {
                    if (Gridchords[_measure].Item2 == NoChord)
                        Gridchords[_measure] = (Gridchords[_measure].Item1, EmptyChord);
                }
                else
                {
                    List<int> lstInt = new List<int>();
                    //Remove doubles
                    lstInt = new List<int>();
                    for (int i = 0; i < lstSecmidiNotes.Count; i++)
                    {
                        int n = lstSecmidiNotes[i].Number % 12;
                        lstInt.Add(n);
                    }
                    lstInt = lstInt.Distinct().ToList();

                    // Check impossible chords
                    // C, E, D, D#, A
                    lstInt = CheckImpossibleChord(lstInt);

                    if (lstInt.Count > 2)
                    {
                        // Debug
                        List<string> notletters = TransposeToLetterChord(lstInt);

                        // Store into table
                        int[] intNotes = new int[lstInt.Count];
                        for (int i = 0; i < lstInt.Count; i++)
                            intNotes[i] = lstInt[i];

                        lnIntNote = new List<int[]>();
                        // Build ln = list of all combinations of int
                        PermuteIntNote(intNotes, 0, lstInt.Count - 1);

                        // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                        List<List<int>> notes = new List<List<int>>();
                        foreach (int[] arry in lnIntNote)
                        {
                            List<int> lll = new List<int>();
                            for (int i = 0; i < arry.Length; i++)
                            {
                                lll.Add(arry[i]);
                            }
                            notes.Add(lll);
                        }

                        notes = ChangeListListNotesNumber(notes);

                        // Search root note                    
                        List<int> lroot = DetermineRoot(notes);

                        if (lroot != null)
                        {
                            // Transpose to letters
                            List<string> notesletters = TransposeToLetterChordSpecial(lroot);
                            List<string> res = ch.determine(notesletters);

                            if (res.Count > 0)
                            {
                                if (Gridchords[_measure].Item2 == NoChord)
                                    Gridchords[_measure] = (Gridchords[_measure].Item1, res[0]);
                            }
                            else
                            {
                                lroot = new List<int> { lroot[0], lroot[1], lroot[2] };
                                // Transpose to letters
                                notesletters = TransposeToLetterChord(lroot);
                                res = ch.determine(notesletters);
                                if (res.Count > 0)
                                {
                                    if (Gridchords[_measure].Item2 == NoChord)
                                        Gridchords[_measure] = (Gridchords[_measure].Item1, res[0]);
                                }
                                else
                                    Console.WriteLine(string.Format("{0} - [Search by chord] ERREUR : Determine n'a pas trouvé d'accord", _measure));
                            }
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} - [Search by chord] no root note found", _measure));
                        }

                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} - [Search by chord] number of notes < 2", _measure));
                    }

                }
                #endregion               

            }

        }

        private List<int> CheckImpossibleChord(List<int> lstInt)
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
            float timeinmeasure = sequence1.Numerator - (int)((curmeasure * _measurelen - ticks) / (_measurelen / sequence1.Numerator));
            
            return timeinmeasure;

            //int rest = ticks % _measurelen;
            //return (float)rest / sequence1.Time.Quarter;            
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


        /// <summary>
        /// Fix error F => E#
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<string> TransposeToLetterChordSpecial(List<int> notes)
        {
            //List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
            //List<string> letters = new List<string>() { "B#", "C#", "D", "D#", "E", "E#", "F#", "G", "G#", "A", "A#", "B" };
            //List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "E#", "F#", "G", "G#", "A", "A#", "B" };

            List<string> letters = new List<string>() { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

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



        /// <summary>
        /// Retrieve notes letter for a chord
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private string TransposeToLetterNote(int n)
        {
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            
            string l = string.Empty;
            int x = 0;
                        
            x = n % 12;
            l = letters[x];
                        
            return l;
        }


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
                // this test needs that chord have only 3 notes
                if (IsMajorChord(chord) || IsMinorChord(chord))                    
                    return chord;                    
            }
            return null;  
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


    }
}
