using System;
using System.Collections.Generic;
using System.Globalization;
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

        public Part()
		{
			Id = string.Empty;						
			// Fab
			MidiChannel = 1;
			MidiProgram = 1;
            Volume = 80; // 101;
			Pan = 80;
            _chromatictranspose = 0;
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


            // PAN
            _part.Pan = (int?)partlistElement.Descendants("pan").FirstOrDefault() ?? 0;
            _part.Pan += 65;
          
            // Default
            // Division 480 + Tempo 500000 => BPM 120
            //_part.Tempo = 500000;            
            
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
                            string diatonic = transpose.Descendants("diatonic").FirstOrDefault()?.Value;
                            string chromatic = transpose.Descendants("chromatic").FirstOrDefault()?.Value;
                            try
                            {
                                _part._chromatictranspose = Convert.ToInt32(chromatic);
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
                            _part.coeffmult = 480 / _part.Division;
                            _part.Division = 480; // = _part.coeffmult * _part.Division;
                        }
                    }


                    XElement pnote = partElement.Descendants("note").FirstOrDefault();
                    int v = (int?)pnote.Descendants("voice").FirstOrDefault() ?? -1;
                    if (v != -1 & _part.Voice == 0)
                        _part.Voice = v;
                    v = (int?)pnote.Descendants("staff").FirstOrDefault() ?? -1;
                    if (v != -1 && _part.Staff == 0)
                        _part.Staff = v;


                    bool bReserved = false;
                    List<int> lstVerseNumber = new List<int>();

                    var measuresXpath = string.Format("//part[@id='{0}']/measure", _part.Id);
                    var measureNodes = doc.XPathSelectElements(measuresXpath);                    
                    
                    
                    
                    foreach ( XElement measureNode in measureNodes )
                    {                        
                        Measure curMeasure = new Measure();
                                                
                        // Why ?: the measures between ending.start and ending.stop are reserved for one or several verses
                        // So we must set the list of verses to these measures
                        if (bReserved) 
                        {
                            /*
                            if (curMeasure.Number == 5)
                            {
                                Console.Write("ici");
                            }
                            */
                            curMeasure.lstVerseNumber = lstVerseNumber;
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

                        /*
                        if (curMeasure.Number == 14)
                        {
                            Console.WriteLine("ici");
                        }
                        */

                        #endregion measure number


                        foreach (XElement childnode in measureNode.Descendants())
                        {                            
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
                            /*
                            else if (childnode.Name == "sound")
                            {
                                if (childnode.Attribute("tempo") != null)
                                {
                                    
                                    double curTempo = Convert.ToDouble(childnode.Attribute("tempo").Value, (CultureInfo.InvariantCulture));
                                    const float kOneMinuteInMicroseconds = 60000000;
                                    float ttempo = kOneMinuteInMicroseconds / (float)curTempo;

                                    _part.Tempo = (int)ttempo;                                    

                                }
                            }
                            */
                            else if (childnode.Name == "note")
                            {
                                
                                // Get notes information
                                Note note = GetNote(childnode, _part.coeffmult, _part._chromatictranspose);

                                if (note.Lyrics != null && note.Lyrics.Count > 0)
                                {
                                    
                                    /*
                                    if (curMeasure.Number == 4)
                                    {
                                        Console.Write("ici");
                                    }
                                     */                                   

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

                                // Create new element
                                MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Note, Element = note };
                                curMeasure.MeasureElements.Add(trucmeasureElement);

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

        private static Note GetNote(XElement node, int mult, int transpose)
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

            bool bgrace = false;

            Note note = new Note();

            if (staff != null)
                note.Staff = int.Parse(staff.Value);

            if (voice != null)
                note.Voice = int.Parse(voice.Value);

            if (rest != null)
                note.IsRest = true;
            
            if (type != null)          
                note.Type = type.Value;
            

            string stp = "";
            if (step != null)
            {
                stp = step.Value;
                note.Pitch.Step = stp[0];
                note.Transpose = transpose;                                
            }

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

            if (grace != null)
            {
                if (grace.FirstAttribute != null && grace.FirstAttribute.Value == "yes")
                    bgrace = true;                
            }


            if (duration != null)
            {
                note.Duration = int.Parse(duration.Value);                
                note.Duration = note.Duration * mult;
            }
            else
            {
                note.Duration = 0;                               
                
            }

            if (chord != null)            
                note.IsChordTone = true;

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
