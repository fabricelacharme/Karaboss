using KMusicXml.MusicXml.Domain;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using static MusicXml.Domain.Note;
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
        
        //private int _division;
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

        public float coeffmult { get; set; }

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


            // ======================================
            // Credits
            // ======================================
            MidiTags.ResetTags();
            MidiTags.KTag.Add("MIDI KARAOKE FILE");
            MidiTags.VTag.Add("0100");
            string s;

            foreach (var partIdent in doc.Descendants("identification"))
            {
                string creator = partIdent.Descendants("creator").FirstOrDefault()?.Value;
                if (creator != null)
                    MidiTags.ITag.Add(string.Format("Arranger: {0}", creator) );
                
                var encod = partIdent.Descendants("encoding").FirstOrDefault();
                if (encod != null)
                {
                    s = encod.Descendants("software").FirstOrDefault()?.Value;
                    if (s != null)
                        MidiTags.ITag.Add(string.Format("Encoding: {0}", s));
                }

            }

            foreach (var partCredit in doc.Descendants("credit"))
            {                
                var type = partCredit.Descendants("credit-type").FirstOrDefault()?.Value;
                if (type != null)
                {
                    switch (type)
                    {
                        case "title":
                            s = partCredit.Descendants("credit-words").FirstOrDefault()?.Value;
                            MidiTags.TTag.Add(s);
                            MidiTags.TagTitle = s;
                            break;
                        case "composer":
                            s = partCredit.Descendants("credit-words").FirstOrDefault()?.Value;
                            MidiTags.TTag.Add(s);
                            MidiTags.TagArtist = s; 
                            break;
                        default:
                            break;
                    }
                }

            }


            // ======================================
            // VOLUME
            // ******************************* The result is wrong => convert value to midi value *************************************************
            int vol = ConvertStringValue(partlistElement.Descendants("volume").FirstOrDefault()?.Value, 80);
            _part.Volume = vol * 127/100;


            // ======================================
            // PAN
            // 0 = -50%, 64 = 0%, 128 = 50%
            // ======================================            
            int pan = ConvertStringValue(partlistElement.Descendants("pan").FirstOrDefault()?.Value, 0);
            // pan = 0 in xml, means 64 in Midi (no pan)
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
                            _part.Numerator = ConvertStringValue(ptime.Descendants("beats").FirstOrDefault()?.Value, 4);
                            _part.Denominator = ConvertStringValue(ptime.Descendants("beat-type").FirstOrDefault()?.Value, 4);
                        }

                        // Divisions (forced to 480)
                        XElement quarterlength = attributes.Descendants("divisions").FirstOrDefault();
                        if (quarterlength != null)
                        {                            
                            // Real division
                            _part.Division = ConvertStringValue(attributes.Descendants("divisions").FirstOrDefault().Value, 1);                            

                            // pour avoir la longueur d'une mesure
                            int m = (int)(_part.Division * _part.Numerator * (4f / _part.Denominator));
                            _part._measurelength = m;


                            // 03/02/2025 
                            _part.coeffmult = 1;

                            // A gérer plus tard
                            // Force division to 480 with a coeff
                            //_part.coeffmult = 480f / _part.Division;
                            //_part.Division = 480; 
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


                        // if number = 0, this measure is different than other ones (shorter)
                        // ?? add additional rests ?
                        // <measure implicit="yes" number="0" width="167">
                        if (curMeasure.Number == 0)
                        {
                            var vLocalNotes  = measureNode.Descendants("note");

                            int d = 0;
                            int curstaff = -1;
                            int x = -1;
                            foreach (XElement e in vLocalNotes)
                            {
                                var ddur = e.Descendants("duration").FirstOrDefault();
                                var chord = e.Descendants("chord").FirstOrDefault();
                                var staf = e.Descendants("staff").FirstOrDefault();
                                if (staf != null)
                                {
                                    x = ConvertStringValue(staf.Value, -1);
                                    if (x != curstaff)
                                    {
                                        curstaff = x;
                                        d = 0;
                                    }
                                }                                

                                if (ddur != null && chord == null && (x == curstaff))
                                {
                                    d += ConvertStringValue(ddur.Value, 0);
                                }                                
                            }
                            if (d < _part._measurelength)
                            {                                
                                // Add a rest note to complete the measure
                                Note note = new Note();
                                note.IsRest = true;
                                note.Duration = (int)((_part._measurelength - d) * _part.coeffmult);
                                MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Note, Element = note };
                                curMeasure.MeasureElements.Add(trucmeasureElement);
                            }
                        }


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

                                    /*
                                    var newTime = new Time();
                                    newTime.Tempo = ttempo;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Time, Element = newTime };
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                                    */

                                    var newTempo = new TempoChange();
                                    newTempo.Tempo = ttempo;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.TempoChange, Element = newTempo };
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
                                //var t = measureNode.Attribute("number").Value;
                                //if (int.Parse(t) == 11 && _part.Id == "P2")
                                //    Console.Write("");

                                Note note = GetNote(childnode, _part.coeffmult, _part._chromatictranspose, _part._octavechange, _part.SoundDynamics, vNotes, _part._measurelength);
                                

                                #region note lyrics
                                if (note.Lyrics != null && note.Lyrics.Count > 0)
                                {
                                    // case not reserved and normally 3 verses, but only one lyric on this note
                                    // add a space as a lyric on missing ones
                                    if (!bReserved && (note.Lyrics.Count < curMeasure.lstVerseNumber.Count))
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
                                            lyric.Text = note.Lyrics[0].Text;
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

                                //_part.Notes.Add(note.Pitch.Step.ToString() + ", " + note.Duration.ToString());
                                curMeasure.Notes.Add(note.Pitch.Step.ToString() + ", " + note.Duration.ToString());

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
                                    int duration = (int)(_part.coeffmult * int.Parse(dur.Value));

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
                                    int duration = (int)(_part.coeffmult * int.Parse(dur.Value));
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
                                    s = nending.Attribute("number").Value;
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
                            else if (childnode.Name == "time")
                            {                                
                                var beats = childnode.Descendants("beats").FirstOrDefault();
                                if (beats != null) 
                                { 
                                    var beatType = childnode.Descendants("beat-type").FirstOrDefault();
                                    if (beatType != null)
                                    {
                                        var timesignature = new Time();
                                        timesignature.Beats = int.Parse(beats.Value);
                                        timesignature.BeatType = int.Parse(beatType.Value);
                                        MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Time, Element = timesignature };
                                        curMeasure.MeasureElements.Add(trucmeasureElement);


                                        int m = (int)(_part.Division * timesignature.Beats * (4f / timesignature.BeatType));
                                        _part._measurelength = m;
                                        
                                    }
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

        /// <summary>
        /// Return an int from any input, even with comma or point separator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int ConvertStringValue(string value, int defvalue)
        {            
            try
            {
                if (value == null) { return defvalue; }
                
                var comma = (NumberFormatInfo)CultureInfo.InstalledUICulture.NumberFormat.Clone();
                value = value.Replace(".", comma.NumberDecimalSeparator);

                return Convert.ToInt32(Convert.ToDouble(value));
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.ToString());
                return defvalue;
            }
           
        }

        private static Chord GetChord(XElement node, float mult, IEnumerable<XElement> c)
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
                    chord.Offset = (int)(int.Parse(offset.Value) * mult);
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

            chord.RemainDuration = (int)(Duration * mult);
            return chord;
        }

        private static Note GetNote(XElement node, float mult, int chromatictranspose, int octavechange, int SoundDynamics, IEnumerable<XElement> c, int mesurelength)
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
            var tied = node.Descendants("tied").FirstOrDefault();             // Linked notes

            var ties = node.Descendants("tie");
            var articulations = node.Descendants("articulations").FirstOrDefault(); // Staccato

            // Drums ?
            var displaystep = node.Descendants("display-step").FirstOrDefault();
            var displayoctave = node.Descendants("display-octave").FirstOrDefault();
            var instrument = node.Descendants("instrument").FirstOrDefault();

            //bool bgrace = false;

            Note note = new Note();

            if (staff != null)
                note.Staff = int.Parse(staff.Value);

            if (voice != null)
                note.Voice = ConvertStringValue(voice.Value, 1);

            int nDuration = 0;
            if (duration != null)           
                nDuration = ConvertStringValue(duration.Value, 0); //int.Parse(duration.Value);
            

            if (rest != null)
            {                
                // If rest and whole measure => duration = measure length
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
                #region drums
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
                        if (isNumeric(stp))
                            note.DrumInstrument = Convert.ToInt32(stp) - 1;
                        else
                        {
                            stp = stp.Substring(stp.Length - 1, 1);
                            if (isNumeric(stp))
                                note.DrumInstrument = Convert.ToInt32(stp) - 1;
                        }
                    }
                }
                else
                {
                    //note.DrumInstrument = note.Pitch.Step;
                }
                #endregion drums
            }

            // Velocity
            note.Velocity = SoundDynamics;

            // Warning: alter maybe -1.5
            string accidental = "";
            if (alter != null)
            {
                //switch (int.Parse(alter.Value))
                switch (ConvertStringValue(alter.Value, 0))
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
                note.Pitch.Alter = ConvertStringValue(alter.Value, 0);
            }

            note.Accidental = accidental;

            if (octave != null)
                note.Pitch.Octave = ConvertStringValue(octave.Value, 4); //int.Parse(octave.Value);
            else if (displayoctave != null)
                note.Pitch.Octave = ConvertStringValue(displayoctave.Value, 4); //int.Parse(displayoctave.Value);
            
            /*
            if (grace != null)
            {
                if (grace.FirstAttribute != null && grace.FirstAttribute.Value == "yes")
                    bgrace = true;                
            }
            */

            if (articulations != null)
            {                
                string a = articulations.Descendants().FirstOrDefault().Name.ToString();
                switch (a)
                {

                    case "accent":
                        note.Articulation = Note.Articulations.accent;
                        break;
                    case "strong-accent":
                        note.Articulation = Note.Articulations.strongaccent;
                        break;
                    case "staccato":
                        note.Articulation = Note.Articulations.staccato;
                        break;
                    case "tenuto":
                        note.Articulation = Note.Articulations.tenuto;
                        break;
                    case "detached-legato":
                        note.Articulation = Note.Articulations.detachedlegato;
                        break;
                    case "staccatissimo":
                        note.Articulation = Note.Articulations.staccatissimo;
                        break;
                    case "spiccato":
                        note.Articulation = Note.Articulations.spiccato;
                        break;
                    case "scoop":
                        note.Articulation = Note.Articulations.scoop;
                        break;
                    case "plop":
                        note.Articulation = Note.Articulations.plop;
                        break;
                    case "doit":
                        note.Articulation = Note.Articulations.doit;
                        break;
                    case "falloff":
                        note.Articulation = Note.Articulations.falloff;
                        break;
                    case "breath-mark":
                        note.Articulation = Note.Articulations.breathmark;
                        break;
                    case "caesura":
                        note.Articulation = Note.Articulations.caesura;
                        break;
                    case "stress":
                        note.Articulation = Note.Articulations.stress;
                        break;
                    case "unstress":
                        note.Articulation = Note.Articulations.unstress;
                        break;
                    case "soft-accent":
                        note.Articulation = Note.Articulations.softaccent;
                        break;
                    case "other-articulation":                                                
                    default:
                        note.Articulation = Note.Articulations.otherarticulation;
                        break;
                }                
            }

            #region duration

            // Linked notes
            string ti;
            if (ties != null && ties.Count() > 0)
            {
                if (ties.Count() == 1)
                {
                    ti = tie.Attribute("type").Value;
                    note.TieType = (ti == "start") ? Note.TieTypes.Start : Note.TieTypes.Stop;
                }
                else if (ties.Count() == 2)
                {
                    note.TieType = Note.TieTypes.Both;
                }

                // Duration inside the current measure
                note.TieDuration = (int)(nDuration * mult);
            }

            // Real duration
            note.Duration = (int)(nDuration * mult);

                    
            
            // Ajust calculation with notes having tie
            if (note.TieType == Note.TieTypes.Start)
            {                
                bool bStart = false;
                int curstaff = -1;
                int x = -1;
                if (staff != null)
                {
                    curstaff = ConvertStringValue(staff.Value, -1);
                }

                // Start of a linked note: add duration of Tie Stop note
                foreach (XElement e in c)
                {
                    x = -1;
                    if (e.Name == "note")
                    { 
                        if (e == node)
                        {
                            bStart = true;
                        }
                        else
                        {
                            var ttie = e.Descendants("tie").FirstOrDefault();
                            var tiesnext = e.Descendants("tie");

                            if (tiesnext != null && tiesnext.Count() > 0)
                            {
                                if (!note.IsDrums)
                                {
                                    var stepnext = e.Descendants("step").FirstOrDefault();
                                    var staffnext = e.Descendants("staff").FirstOrDefault();
                                    if (staffnext != null)
                                        x = ConvertStringValue(staffnext.Value, -1);

                                    if (bStart && stepnext.Value == step.Value && x == curstaff)
                                    {
                                        var ddur = e.Descendants("duration").FirstOrDefault();
                                        if (ddur != null)
                                        {
                                            int ddd = (int)(int.Parse(ddur.Value) * mult);
                                            note.Duration += ddd;

                                            // Only one linked note => break
                                            if (tiesnext.Count() == 1 && ttie.Attribute("type").Value == "stop")
                                                break;                                            
                                        }
                                    }
                                } 
                                else
                                {
                                    // Drums
                                    var displaystepnext = e.Descendants("display-step").FirstOrDefault();
                                    var staffnext = e.Descendants("staff").FirstOrDefault();
                                    if (staffnext != null)
                                        x = ConvertStringValue(staffnext.Value, -1);

                                    if (bStart && displaystepnext.Value == displaystep.Value && x == curstaff)
                                    {
                                        var ddur = e.Descendants("duration").FirstOrDefault();                                        
                                        if (ddur != null)
                                        {
                                            int ddd = (int)(int.Parse(ddur.Value) * mult);
                                            note.Duration += ddd;

                                            // Only one linked note => break
                                            if (tiesnext.Count() == 1 && ttie.Attribute("type").Value == "stop")
                                                break;
                                        }
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
            else if (note.TieType == Note.TieTypes.Both)
            {
                note.Duration = 0;
            }


           

            #endregion duration

            // Note is part of a chord
            if (chord != null)            
                note.IsChordTone = true;

            // Note has a stem to draw (help to determine if it is a real note to be played and not a placeholder) 
            if (stem != null)
                note.Stem = stem.Value;
            

            // Manage several lyrics per note (a note can be used by several verses)
            note.Lyrics = GetLyrics(node);

            // TODO number is wrong for repeat of 2 verses
            // verses 2, 3 and numbers are 1, 2

            return note;
        }

        private static bool isNumeric(string stp)
        {
            return int.TryParse(stp, out int test);
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
