using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

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

        public Part()
		{
			Id = string.Empty;						
			// Fab
			MidiChannel = 1;
			MidiProgram = 1;
			Volume = 80;
			Pan = 80;            
		}



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
            _part.Volume = vol;


            _part.Pan = (int?)partlistElement.Descendants("pan").FirstOrDefault() ?? 0;
            _part.Pan += 65;
          
            // Default
            _part.Tempo = 100000;

            foreach (var partElement in doc.Descendants("part"))
            {
                // Check if goof Id for this part
                string idd = partElement.Attributes("id").FirstOrDefault()?.Value;
                if (idd == _part.Id)
                {
                    
                    // Is there a 2nd track included in this part?
                    _part.Staves = (int?)partElement.Descendants("staves").FirstOrDefault() ?? 1;

                    _part.Division = (int?)partElement.Descendants("divisions").FirstOrDefault() ?? 24;

                    XElement ptime = partElement.Descendants("time").FirstOrDefault();
                    _part.Numerator = (int?)ptime.Descendants("beats").FirstOrDefault() ?? 4;
                    _part.Denominator = (int?)ptime.Descendants("beat-type").FirstOrDefault() ?? 4;

                    XElement pnote = partElement.Descendants("note").FirstOrDefault();
                    int v = (int?)pnote.Descendants("voice").FirstOrDefault() ?? -1;
                    if (v != -1 & _part.Voice == 0)
                        _part.Voice = v;
                    v = (int?)pnote.Descendants("staff").FirstOrDefault() ?? -1;
                    if (v != -1 && _part.Staff == 0)
                        _part.Staff = v;


                    var measuresXpath = string.Format("//part[@id='{0}']/measure", _part.Id);
                    var measureNodes = doc.XPathSelectElements(measuresXpath);                    
                    foreach ( XElement measureNode in measureNodes )
                    {                        
                        Measure curMeasure = new Measure();

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
                            // Attributes containing everything
                            curMeasure.Attributes = new MeasureAttributes();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        #endregion measure number


                        foreach (XElement childnode in measureNode.Descendants())
                        {                            
                            if (childnode.Name == "sound")
                            {
                                if (childnode.Attribute("tempo") != null)
                                {
                                    int curTempo = int.Parse(childnode.Attribute("tempo").Value);
                                    if (curTempo > 0)
                                    {
                                        curTempo *= 10000;
                                        curMeasure.Tempo = curTempo;                                        
                                        _part.Tempo = curTempo;
                                    }
                                }
                            }                            
                            else if (childnode.Name == "note")
                            {
                                // Get notes information
                                Note note = GetNote(childnode);       

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
                                    int duration = int.Parse(dur.Value);
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
                                    int duration = int.Parse(dur.Value);
                                    forward.Duration = duration;
                                    MeasureElement trucmeasureElement = new MeasureElement { Type = MeasureElementType.Forward, Element = forward };
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
            return _part;
        }


        private static int ConvertStringValue(string value)
        {
            var comma = (NumberFormatInfo)CultureInfo.InstalledUICulture.NumberFormat.Clone();
            value = value.Replace(".", comma.NumberDecimalSeparator);

            return Convert.ToInt32(Convert.ToDouble(value));
           
        }

        private static Note GetNote(XElement node)
        {
            var rest = node.Descendants("rest").FirstOrDefault();
            var step = node.Descendants("step").FirstOrDefault();
            var alter = node.Descendants("alter").FirstOrDefault();
            var octave = node.Descendants("octave").FirstOrDefault();
            var duration = node.Descendants("duration").FirstOrDefault();
            var chord = node.Descendants("chord").FirstOrDefault();
            var voice = node.Descendants("voice").FirstOrDefault();
            var staff = node.Descendants("staff").FirstOrDefault();

            Note note = new Note();

            if (staff != null)
                note.Staff = int.Parse(staff.Value);

            if (voice != null)
                note.Voice = int.Parse(voice.Value);

            if (rest != null)
                note.IsRest = true;

            string stp = "";
            if (step != null)
            {
                stp = step.Value;
                note.Pitch.Step = stp[0];
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

            if (duration != null)
                note.Duration = int.Parse(duration.Value);

            if (chord != null)            
                note.IsChordTone = true;

            note.Lyric = GetLyric(node);

            return note;
        }
      
        private static Lyric GetLyric(XElement node)
        {
            var lyric = new Lyric();

            var lyrics = node.Descendants("lyric").FirstOrDefault();
            if (lyrics != null)
            {
                var syllabicNode = lyrics.Descendants("syllabic").FirstOrDefault();
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

                var textNode = node.Descendants("text").FirstOrDefault();
                if (textNode != null)
                    lyric.Text = textNode.Value;

            }

            return lyric;
        }

    }
}
