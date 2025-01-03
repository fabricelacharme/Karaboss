using KMusicXml.MusicXml.Domain;
using Sanford.Multimedia.Midi.Score;
using System;
using System.Collections.Generic;
using System.Globalization;
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

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion

using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using static Sanford.Multimedia.Midi.Track;

namespace MusicXml.Domain
{
    public class Part
	{

        // <sound dynamics="THE_DYNAMIC_YOU_WANT"/>
        // <sound dynamics="88.89"/>
        public int SoundDynamics { get; set; }
        public List<Measure> Measures { get; set; } = new List<Measure>();

        public List<string> Notes
        {
            get
            {
                List<string> notes = new List<string>();
                Measures.ForEach(m => notes.AddRange(m.Notes));
                return notes;
            }
        }

        public List<int> Durations
        {
            get
            {
                List<int> durations = new List<int>();
                Measures.ForEach(m => durations.AddRange(m.Durations));
                return durations;
            }
        }


        public List<int> Times
        {
            get
            {
                List<int> times = new List<int>();

                Measures.ForEach(m => times.AddRange(m.Durations
                   .Select(d => (int)(60000 * (float)d / Division / m.Tempo))));

                return times;
            }
        }

        public int Staves { get; set; }  // included 2nd track
        public int Staff { get; set; }   // Staff number
        public int Voice { get; set; }  // voice
        
        private int _division;
        public int Division { get; set; }
        public Dictionary<int, int> Tempos { get; set; } // key = measure, value = tempo

        public string ID { get; set; }
        public string Name { get; set; }

        public string Raw { get; set; }

        public string Id { get; set; }
              
        // Fab
        public int MidiChannel { get; internal set; }
        public int MidiProgram { get; internal set; }
        public int Volume { get; internal set; }
        public int Pan { get; internal set; }

        public int Tempo { get; internal set; }

        public int Numerator { get; internal set; }
        public int Denominator { get; internal set; }

        public int _chromatictranspose {get; internal set; }
        public int _octavechange { get; internal set; }

        public int _measurelength { get; internal set; }

        public enum ScoreTypes 
        {
            None = 0,
            Notes = 1,
            Chords = 2,
            Both = 3,
        }

        public ScoreTypes ScoreType { get; set; }

        public Part()
		{
			Id = string.Empty;						
			// Fab
			MidiChannel = 1;
			MidiProgram = 1;
            Volume = 80; // 101;
            SoundDynamics = 80;
			Pan = 80;
            _chromatictranspose = 0;
            _octavechange = 0;

            ScoreType = ScoreTypes.Notes;
		}



        public int coeffmult { get; set; }

        /// <summary>
        /// Create each part
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="partlistElement"></param>
        /// <returns></returns>
        public static Part Create(XDocument doc, XElement partlistElement)
        {
            Part _part = new Part();

            string id = partlistElement.Attributes("id").FirstOrDefault()?.Value;
            _part.Id = id;

            _part.MidiChannel = (int?)partlistElement.Descendants("midi-channel").FirstOrDefault() ?? 1;
            _part.MidiProgram = (int?)partlistElement.Descendants("midi-program").FirstOrDefault() ?? 1;


            _part.coeffmult = 6;

            // VOLUME
            // ******************************* The result is wrong => convert value to midi value *************************************************
            //_part.Volume = (int?)partlistElement.Descendants("volume").FirstOrDefault() ?? 80;
            int vol = 80;
            string volu;
            if (partlistElement.Descendants("volume").FirstOrDefault() != null) 
            { 
                volu = partlistElement.Descendants("volume").FirstOrDefault().Value;
                if (volu.IndexOf(".") > 0 || volu.IndexOf(",") > 0)
                {
                    vol = ConvertStringValue(volu);
                }
                else
                    vol = Convert.ToInt32(volu);
            }
            _part.Volume = vol * 127/100;


            // ======================================
            // PAN
            // 0 = -50%, 64 = 0%, 128 = 50%
            // ======================================
            int pan = 64;
            if (partlistElement.Descendants("pan").FirstOrDefault() != null)
            {
                string pa = partlistElement.Descendants("pan").FirstOrDefault()?.Value;
                if (pa.IndexOf(".") > 0 || pa.IndexOf(",") > 0)
                {
                    pan = ConvertStringValue(pa);
                }
                else
                    pan = Convert.ToInt32(pa);
            }
            _part.Pan = 64 * (1 + pan / 90);


            // ====================================================
            // For each track
            // ====================================================
            foreach (var partElement in doc.Descendants("part"))
            {
                // Check if goof Id for this part
                string idd = partElement.Attributes("id").FirstOrDefault()?.Value;
                if (idd == _part.Id)
                {                    
                    // Is there a 2nd track included in this part?
                    _part.Staves = (int?)partElement.Descendants("staves").FirstOrDefault() ?? 1;
                   
                    // =======================================================================
                    // ATTRIBUTES ******************
                    // =======================================================================
                    XElement attributes = partElement.Descendants("attributes").FirstOrDefault();
                    if (attributes != null) 
                    { 
                        // Transpositions
                        XElement transpose = attributes.Descendants("transpose").FirstOrDefault();
                        if (transpose != null)
                        {
                            // Chromatic transpose
                            //string diatonic = transpose.Descendants("diatonic").FirstOrDefault()?.Value;
                            string chromatic = transpose.Descendants("chromatic").FirstOrDefault()?.Value;
                            try
                            {
                                _part._chromatictranspose = Convert.ToInt32(chromatic);
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }

                            // Octave transpose
                            string octave = transpose.Descendants("octave-change").FirstOrDefault()?.Value;
                            try
                            {
                                _part._octavechange = Convert.ToInt32(octave);
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                        }

                        // Time
                        XElement ptime = attributes.Descendants("time").FirstOrDefault();
                        if (ptime != null)
                        {
                            _part.Numerator = (int?)ptime.Descendants("beats").FirstOrDefault() ?? 4;
                            _part.Denominator = (int?)ptime.Descendants("beat-type").FirstOrDefault() ?? 4;
                        }

                        // Divisions
                        XElement quarterlength = attributes.Descendants("divisions").FirstOrDefault();
                        if (quarterlength != null)
                        {
                            _part.Division = (int)attributes.Descendants("divisions").FirstOrDefault();
                            _part._measurelength = _part.Division * _part.Numerator; // FAB pour avoir la longueur d'une mesure
                            _part.coeffmult = 480 / _part.Division;
                            _part.Division = 480; 
                        }
                    }

                    // Search for VOICE
                    XElement pnote = partElement.Descendants("note").FirstOrDefault();
                    int v = (int?)pnote.Descendants("voice").FirstOrDefault() ?? -1;
                    if (v != -1 & _part.Voice == 0)
                        _part.Voice = v;
                    v = (int?)pnote.Descendants("staff").FirstOrDefault() ?? -1;
                    if (v != -1 && _part.Staff == 0)
                        _part.Staff = v;


                    // Search for CHORDS OR NOTES mode
                    XElement pchord = partElement.Descendants("harmony").FirstOrDefault();
                    if (pchord != null)
                    {
                        _part.ScoreType = ScoreTypes.Chords;
                    }

                    bool bReserved = false;
                    List<int> lstVerseNumber = new List<int>();

                    var measuresXpath = string.Format("//part[@id='{0}']/measure", _part.Id);
                    var measureNodes = doc.XPathSelectElements(measuresXpath);

                    IEnumerable<XElement> vHarmony; // both chords & notes
                    IEnumerable<XElement> vNotes = measureNodes.Descendants("note");  // only notes


                    // ====================================================
                    // For each measure
                    // ====================================================
                    foreach ( XElement measureNode in measureNodes )
                    {
                        // Reset harmony (chords) elements of this measure
                        // hamony tags are to be managed inside a measure
                        vHarmony = new List<XElement>();                        

                        Measure curMeasure = new Measure();
                                                
                        // Why ?: the measures between ending.start and ending.stop are reserved for one or several verses
                        // So we must set the list of verses to these measures
                        if (bReserved) 
                        {                           
                            curMeasure.lstVerseNumber = lstVerseNumber;
                        }

                        if (_part.ScoreType == ScoreTypes.Chords)
                        {
                            // Search how many harmony tags                            
                            vHarmony = measureNode.Descendants().Where(x => x.Name.LocalName == "harmony" || x.Name.LocalName == "note");
                        }

                        // Attributes containing everything
                        curMeasure.Attributes = new MeasureAttributes();

                        // ATTIBUTES / KEY **********************************
                        XElement pkey = partElement.Descendants("key").FirstOrDefault();
                        if (pkey != null)
                        {
                            curMeasure.Attributes.Key = new Key();

                            XElement fif = pkey.Descendants("fifths").FirstOrDefault();
                            if (fif != null)                            
                                curMeasure.Attributes.Key.Fifths = Convert.ToInt32(fif.Value);
                            
                            XElement mod = pkey.Descendants("mode").FirstOrDefault();
                            if (mod != null)
                                curMeasure.Attributes.Key.Mode = mod.Value.ToString();
                        }

                        /*
                        XElement attrb = partElement.Descendants("attributes").FirstOrDefault();
                        if (attrb != null)
                        {
                            _part.Division = (int)attributes.Descendants("divisions").FirstOrDefault();
                            _part._measurelength = _part.Division * _part.Numerator; // FAB pour avoir la longueur d'une mesure
                            _part.coeffmult = 480 / _part.Division;
                        }
                        */
                        
                        #region measure number
                        /* 
                        * TODO
                        * Sometimes number = X1
                        * case of repeat 
                        * 
                        */
                        try
                        {
                            curMeasure.Number = int.Parse(measureNode.Attribute("number").Value);                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        #endregion measure number


                        foreach (XElement childnode in measureNode.Descendants())
                        {                            
                            // Speed
                            if (childnode.Name == "metronome")
                            {
                                var pm = childnode.Descendants("per-minute").FirstOrDefault();
                                if (pm != null)                             
                                {
                                    string strtmpo = pm.Value;
                                    strtmpo = strtmpo.Replace(",", ".");                                    
                                    double PerMinute = Convert.ToDouble(strtmpo, (CultureInfo.InvariantCulture));                                    
                                    const float kOneMinuteInMicroseconds = 60000000;
                                    float ttempo = kOneMinuteInMicroseconds / (float)PerMinute;

                                    var newTime = new Time();
                                    newTime.Tempo = ttempo;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Time, Element = newTime };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);


                                    if (_part.Tempo == 0)
                                        _part.Tempo = (int)ttempo;
                                }

                            }                            
                            
                            else if (childnode.Name == "sound")
                            {
                                // Velocity of notes
                                if (childnode.Attribute("dynamics") != null)
                                {                                    
                                    double curSoundDynamics = Convert.ToDouble(childnode.Attribute("dynamics").Value, (CultureInfo.InvariantCulture));                                    

                                    curSoundDynamics = (int)(curSoundDynamics * 80 / 88);
                                    if (curSoundDynamics > 127)
                                    {
                                        curSoundDynamics = 127;                                        
                                    }                                    
                                    _part.SoundDynamics = (int)curSoundDynamics;
                                }
                            }
                            
                            else if (childnode.Name == "note")
                            {                                
                                // Get notes information                                
                                Note note = GetNote(childnode, _part.coeffmult, _part._chromatictranspose, _part._octavechange, _part.SoundDynamics, vNotes, _part._measurelength);

                                #region note lyrics
                                if (note.Lyrics != null && note.Lyrics.Count > 0)
                                {                                                               
                                    // case not reserved and normally 3 verses, but only one lyric on this note
                                    // add a space as a lyric on missing ones
                                    if (!bReserved &&  (note.Lyrics.Count < curMeasure.lstVerseNumber.Count))
                                    {
                                        int versenumber;
                                        List<Lyric> lstlyrics = new List<Lyric>();  
                                        for (int i = 0; i < curMeasure.lstVerseNumber.Count; i++)
                                        {
                                            Lyric lyric = new Lyric();
                                            lyric.VerseNumber = curMeasure.lstVerseNumber[i];
                                            lyric.Text = "";
                                            lstlyrics.Add(lyric);
                                        }
                                        for (int i = 0; i < lstlyrics.Count; i++)
                                        {
                                            versenumber = lstlyrics[i].VerseNumber;
                                            for (int j = 0; j < note.Lyrics.Count; j++)
                                            {
                                                if (note.Lyrics[j].VerseNumber == versenumber)
                                                {
                                                    lstlyrics[i] = note.Lyrics[j];
                                                    break;
                                                }
                                            }                                            
                                        }
                                        note.Lyrics = lstlyrics;                                       
                                    }


                                    // Wrong curMeasure.lstVerseNumber
                                    // Adjust list of verses of the measure to the list of lyrics of the note
                                    if (curMeasure.lstVerseNumber.Count < note.Lyrics.Count)
                                    {
                                        List<int> tmpVerseNumber = new List<int>();
                                        foreach (Lyric lyric in note.Lyrics)
                                        {
                                            if (bReserved || note.Lyrics.Count > 1)
                                            {                                                                                                
                                                // real number for reserved                                                 
                                                tmpVerseNumber.Add(lyric.VerseNumber);
                                                
                                            }
                                            else
                                            {
                                                // 0 for non reserved
                                                tmpVerseNumber.Add(0);
                                            }
                                        }
                                        curMeasure.lstVerseNumber = tmpVerseNumber;
                                    }

                                    // case reserved and verse numbers wrong
                                    // Ajust for the notes (1,2) => (2,3)                                    
                                    if (bReserved && lstVerseNumber.Count > 1 && lstVerseNumber.Count == note.Lyrics.Count)
                                    {                                        
                                        for (int i = 0; i < note.Lyrics.Count; i++)
                                        {
                                            note.Lyrics[i].VerseNumber = lstVerseNumber[i];

                                        }
                                    }

                                    // Case of reserved for several verses, 
                                    if (bReserved && note.Lyrics.Count == 1 && curMeasure.lstVerseNumber.Count > note.Lyrics.Count)
                                    {
                                        
                                        // only one lyric common to all
                                        if (note.Lyrics[0].VerseNumber == 1 && curMeasure.lstVerseNumber[0] > 1) 
                                        {
                                            // One lyric with versnumber = 1
                                            note.Lyrics[0].VerseNumber = curMeasure.lstVerseNumber[0];
                                            Lyric lyric = new Lyric();
                                            lyric.Text =  note.Lyrics[0].Text;
                                            lyric.Syllabic = note.Lyrics[0].Syllabic;
                                            lyric.VerseNumber = note.Lyrics[0].VerseNumber;
                                            
                                            for (int i = 1; i < curMeasure.lstVerseNumber.Count; i++)
                                            {                                                
                                                lyric.VerseNumber = curMeasure.lstVerseNumber[i];
                                                note.Lyrics.Add(lyric);
                                            }
                                        }
                                        else if (note.Lyrics[0].VerseNumber == 2)
                                        {
                                            // One lyric, dedicated to a verse
                                            // verses 2,3
                                            // versenumber > 1 means "not the first verse"
                                            //Console.WriteLine("ici");

                                            note.Lyrics[0].VerseNumber = 3;

                                            Lyric lyric = new Lyric();
                                            lyric.Text = "";
                                            lyric.VerseNumber = 2;
                                            note.Lyrics.Insert(0, lyric);

                                        }                                                                                                                             
                                    }
                                    
                                }
                                #endregion note lyrics

                                // Create new element
                                MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Note, Element = note };
                                curMeasure.MeasureElements.Add(trucmeasureElement);
                                
                            }
                            
                            else if (childnode.Name == "harmony")
                            {
                                // Chords
                                // <root-step>B</root-step>
                                // <root-alter>B</root-step>
                                // <kind>B</root-step>                                
                                
                                
                                Chord chord = GetChord(childnode, _part.coeffmult, vHarmony);
                                
                                if (chord.Kind != "none")
                                {
                                    // Create new element
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Chord, Element = chord };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                }                                
                            }
                            
                            else if (childnode.Name == "backup")
                            {
                                // voir https://www.w3.org/2021/06/musicxml40/tutorial/midi-compatible-part/

                                var dur = childnode.Descendants("duration").FirstOrDefault();
                                if (dur != null)
                                {
                                    var backup = new Backup();
                                    //int duration = 480 * int.Parse(dur.Value);
                                    int duration = _part.coeffmult * int.Parse(dur.Value);

                                    backup.Duration = duration;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Backup, Element = backup };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                }                               
                            }
                            else if (childnode.Name == "forward")
                            {
                                var dur = childnode.Descendants("duration").FirstOrDefault();
                                if (dur != null)
                                {
                                    var forward = new Forward();
                                    //int duration = 480 * int.Parse(dur.Value);
                                    int duration = _part.coeffmult * int.Parse(dur.Value);
                                    forward.Duration = duration;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Forward, Element = forward };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                }

                            }                            
                            else if (childnode.Name == "barline")
                            {
                                var barline = new Barline();
                                barline.Measure = curMeasure.Number;

                                // There is an "ending" start or stop
                                // it means mesures dedicated to a verse
                                // Start reservation: <ending number="1" type="start"/>
                                // Stop reservation: <ending number="1" type="stop"/>
                                var nending = childnode.Descendants("ending").FirstOrDefault();
                                if (nending != null)
                                {
                                    var ending = new Ending();
                                    // New case: list of numbers
                                    // <ending number="2, 3" type="start" default-y="44.97"/>
                                    string s = nending.Attribute("number").Value;
                                    List<string> lststrnumbers = s.Split(',').Select(p => p.Trim()).ToList(); 
                                    List<int> lstnumbers = new List<int>();
                                    for (int i = 0; i < lststrnumbers.Count; i++)
                                    {
                                        lstnumbers.Add(Convert.ToInt32(lststrnumbers[i]));
                                    }                                    
                                    ending.VerseNumber = lstnumbers;

                                    /*
                                    if (curMeasure.Number == 4)
                                    {
                                        Console.Write("ici");
                                    }
                                    */

                                    string type = nending.Attribute("type").Value;
                                    switch (type)
                                    {
                                        case "start":
                                            ending.Type = EndingTypes.start;
                                            bReserved = true;
                                            lstVerseNumber = ending.VerseNumber;
                                            curMeasure.lstVerseNumber = ending.VerseNumber;
                                            break;
                                        case "stop":
                                            ending.Type = EndingTypes.stop;                                            
                                            curMeasure.lstVerseNumber = ending.VerseNumber;
                                            lstVerseNumber = new List<int>();
                                            bReserved = false;
                                            break;                                        
                                        case "discontinue":
                                            // This is located at the end of a measure
                                            ending.Type = EndingTypes.discontinue;
                                            curMeasure.lstVerseNumber = ending.VerseNumber;
                                            //lstVerseNumber = new List<int>();
                                            //bReserved = false;
                                            lstVerseNumber = ending.VerseNumber;
                                            bReserved = true;
                                            break;
                                    }
                                    // Add element
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Ending, Element = ending };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                }
                                
                                // There is a repeat forward or backward
                                // It means repeat a sequence for a new verse with the same notes
                                var repeat = childnode.Descendants("repeat").FirstOrDefault();
                                if (repeat != null)
                                {                                    
                                    if (repeat.Attribute("direction").Value == "forward")
                                    {                                        
                                        barline.Direction = RepeatDirections.forward;                                        
                                    }
                                    else if (repeat.Attribute("direction").Value == "backward")
                                    {
                                        // Remove bReserved for the next measures
                                        lstVerseNumber = new List<int>();
                                        bReserved = false;
                                        barline.Direction = RepeatDirections.backward;
                                    }
                                    // Add element
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Barline, Element = barline };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                }
                            }
                        }
                        _part.Measures.Add(curMeasure);
                    }
                                
                    string iddd = partElement.Attributes("id").FirstOrDefault()?.Value;
                    if (iddd == null)
                    {
                        iddd = "NO ID";
                    }
                    _part.ID = iddd;

                    string name = doc.Descendants("score-part").Where(p => String.Equals(p.Attribute("id").Value.ToString(), id)).FirstOrDefault()?.Descendants("part-name")?.FirstOrDefault().Value;
                    if (name == null)
                    {
                        name = "NO NAME";
                    }
                    _part.Name = name;
                }
            }
            
            _part.Raw = partlistElement.ToString();
            if (_part.Tempo == 0)
                _part.Tempo = 500000;
            return _part;
        }

        private static int ConvertStringValue(string value)
        {
            var comma = (NumberFormatInfo)CultureInfo.InstalledUICulture.NumberFormat.Clone();
            value = value.Replace(".", comma.NumberDecimalSeparator);

            return Convert.ToInt32(Convert.ToDouble(value));
           
        }

        private static Chord GetChord(XElement node, int mult, IEnumerable<XElement> c)
        {
            string stp = "";

            var step = node.Descendants("root-step").FirstOrDefault();
            var alter = node.Descendants("root-alter").FirstOrDefault();
            var kind = node.Descendants("kind").FirstOrDefault();
            var offset = node.Descendants("offset").FirstOrDefault();

            var bassstep = node.Descendants("bass-step").FirstOrDefault();
            var bassalter = node.Descendants("bass-alter").FirstOrDefault();

            Chord chord = new Chord();

            // Letter            
            if (step != null)
            {
                stp = step.Value;
                chord.Pitch.Step = stp[0];

                // # or b
                if (alter != null)
                {
                    chord.Pitch.Alter = int.Parse(alter.Value);
                }            

                // Major, Dominant, ...
                if (kind != null)
                {
                    chord.Kind = kind.Value;
                }

                if (offset != null)
                {
                    chord.Offset = int.Parse(offset.Value) * mult;
                }
            }

            // Bass of the chord
            #region bass
            if (bassstep != null)
            {
                // Bass is different than chord root. ex Ab/C
                stp = bassstep.Value;
                chord.BassPitch.Step = stp[0];

                if (bassalter != null)
                {
                    chord.BassPitch.Alter = int.Parse(bassalter.Value);
                }
            }
            else
            {
                // Bass is identical as chord root
                chord.BassPitch.Step = chord.Pitch.Step;
                chord.BassPitch.Alter = chord.Pitch.Alter;
            }
            #endregion bass

            int Duration = 0;
            bool bStart = false;

            if (chord.Offset > 0)
            {
                Duration = int.Parse(offset.Value);
            }
            else
            {

                // loop into harmony and notes in ordrer to calculate the time elapse between the current chord
                // and the next chord, or end of measure 
                foreach (XElement e in c)
                {
                    if (e.Name == "note")
                    {
                        if (bStart)
                        {
                            // Calculate duration starting form current harmony
                            var duration = e.Descendants("duration").FirstOrDefault();
                            if (duration != null)
                                Duration += int.Parse(duration.Value);
                        }
                    }
                    else if (e.Name == "harmony")
                    {
                        if (e == node)
                        {
                            // Start counting duration from this point until end of measure or next harmony
                            bStart = true;
                        }
                        else
                        {
                            // A new harmony is encountered => stop calculation
                            if (e.Descendants("offset").FirstOrDefault() == null)
                            {
                                bStart = false;
                            }
                            else
                            {
                                // !!!! case of harmony juts after but with an offset
                                int d = int.Parse(e.Descendants("offset").FirstOrDefault().Value);
                                Duration += d;
                                break;
                            }

                        }

                    }
                }
            }

            chord.RemainDuration = Duration * mult;
            return chord;
        }

        private static Note GetNote(XElement node, int mult, int chromatictranspose, int octavechange, int SoundDynamics, IEnumerable<XElement> c, int mesurelength)
        {
            var rest = node.Descendants("rest").FirstOrDefault();
            var step = node.Descendants("step").FirstOrDefault();            
            var alter = node.Descendants("alter").FirstOrDefault();
            var octave = node.Descendants("octave").FirstOrDefault();            
            var duration = node.Descendants("duration").FirstOrDefault();
            var chord = node.Descendants("chord").FirstOrDefault();
            var voice = node.Descendants("voice").FirstOrDefault();
            var staff = node.Descendants("staff").FirstOrDefault();
            var type = node.Descendants("type").FirstOrDefault();
            var grace = node.Descendants("grace").FirstOrDefault();
            var stem = node.Descendants("stem").FirstOrDefault();           // If stem, notes is drawed so it must be played (otherwise, this is just a timeline)
            var tie = node.Descendants("tie").FirstOrDefault();             // Linked notes

            // Drums ?
            var displaystep = node.Descendants("display-step").FirstOrDefault();
            var displayoctave = node.Descendants("display-octave").FirstOrDefault();
            var instrument = node.Descendants("instrument").FirstOrDefault();

            bool bgrace = false;

            Note note = new Note();

            if (staff != null)
                note.Staff = int.Parse(staff.Value);

            if (voice != null)
                note.Voice = int.Parse(voice.Value);

            int nDuration = 0;
            if (duration != null)
            {
                nDuration = int.Parse(duration.Value);
            }

            if (rest != null)
            {                
                note.IsRest = true;
                if (rest.Attribute("measure") != null && rest.Attribute("measure").Value == "yes")
                {
                    nDuration = mesurelength;
                }
            }
            
            if (type != null)          
                note.Type = type.Value;
            
            string stp = "";
            if (step != null)
            {
                stp = step.Value;
                note.Pitch.Step = stp[0];
                note.ChromaticTranspose = chromatictranspose;
                note.OctaveChange = octavechange;
            }
            // Drums
            else if (displaystep != null)
            {
                stp = displaystep.Value;
                note.Pitch.Step = stp[0];
                note.IsDrums = true;


                if (instrument != null)
                {
                    // <instrument id="P7-I37"/>
                    // Extract 2 last digits => 37 (36 in Karaboss)
                    stp = instrument.LastAttribute.Value;
                    if (stp.Length > 2)
                    {
                        stp = stp.Substring(stp.Length - 2, 2);
                        note.DrumPitch = Convert.ToInt32(stp) - 1;
                    }
                }
            }

            // Velocity
            note.Velocity = SoundDynamics;

            string accidental = "";
            if (alter != null)
            {
                switch (int.Parse(alter.Value))
                {
                    case 1:
                        accidental = "S";
                        break;
                    case -1:
                        accidental = "F";
                        break;
                    default:
                        break;
                }
                note.Pitch.Alter = int.Parse(alter.Value);
            }

            note.Accidental = accidental;

            if (octave != null)
                note.Pitch.Octave = int.Parse(octave.Value);
            else if (displayoctave != null)
                note.Pitch.Octave = int.Parse(displayoctave.Value);
            
            
            if (grace != null)
            {
                if (grace.FirstAttribute != null && grace.FirstAttribute.Value == "yes")
                    bgrace = true;                
            }

            #region duration

            // Linked notes
            string ti;
            if (tie != null)
            {
                ti = tie.Attribute("type").Value;
                note.TieType = (ti == "start") ? Note.TieTypes.Start : Note.TieTypes.Stop;
            }


            note.Duration = nDuration * mult;
            /*
            if (duration != null)
            {
                note.Duration = int.Parse(duration.Value);
                note.Duration = note.Duration * mult;
            }
            else
            {
                note.Duration = 0;
            }
            */

            // Ajust calculation with notes having tie
            if (note.TieType == Note.TieTypes.Start)
            {
                bool bStart = false;
                // Start of a linked note: add duration of Tie Stop note
                foreach (XElement e in c)
                {
                    if (e.Name == "note")
                    { 
                        if (e == node)
                        {
                            bStart = true;
                        }
                        else
                        {
                            var ttie = e.Descendants("tie").FirstOrDefault();
                            if (ttie != null)
                            {
                                if (bStart && ttie.Attribute("type").Value == "stop")
                                {
                                    var ddur = e.Descendants("duration").FirstOrDefault();
                                    if (ddur != null)
                                    {
                                        int ddd = int.Parse(ddur.Value) * mult;
                                        note.Duration += ddd;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (note.TieType == Note.TieTypes.Stop)
            {
                note.Duration = 0;
                
            }
            #endregion duration


            if (chord != null)            
                note.IsChordTone = true;

            if (stem != null)
                note.Stem = stem.Value;

            

            // Manage several lyrics per note (a note can be used by several verses)
            note.Lyrics = GetLyrics(node);

            // TODO number is wrong for repeat of 2 verses
            // verses 2, 3 and numbers are 1, 2

            return note;
        }
              

        /// <summary>
        /// Extract the list of lyrics for a single note 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static List<Lyric> GetLyrics(XElement node)
        {
            var lyric = new Lyric();
            List<Lyric> lstLyrics = new List<Lyric>();
            var allLyrics = node.Descendants("lyric");            

            if (allLyrics != null)
            {
                foreach (var myLyric in allLyrics)
                {
                    if (myLyric != null)
                    {
                        lyric = new Lyric();

                        if (myLyric.Attribute("number") != null)
                        {
                            try
                            {
                                lyric.VerseNumber = Convert.ToInt32(myLyric.Attribute("number").Value);                                                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return lstLyrics;
                            }                            
                        }

                        #region syllabic
                        var syllabicNode = myLyric.Descendants("syllabic").FirstOrDefault();
                        var syllabicText = string.Empty;

                        if (syllabicNode != null)
                            syllabicText = syllabicNode.Value;

                        switch (syllabicText)
                        {
                            case "":
                                lyric.Syllabic = Syllabic.None;
                                break;
                            case "begin":
                                lyric.Syllabic = Syllabic.Begin;
                                break;
                            case "single":
                                lyric.Syllabic = Syllabic.Single;
                                break;
                            case "end":
                                lyric.Syllabic = Syllabic.End;
                                break;
                            case "middle":
                                lyric.Syllabic = Syllabic.Middle;
                                break;
                        }
                        #endregion syllabic

                        var textNode = myLyric.Descendants("text").FirstOrDefault();
                        if (textNode != null)
                            lyric.Text = textNode.Value;                        

                        lstLyrics.Add(lyric);
                    }
                }                                                
            }

            return lstLyrics;
        }

      

    }
}
