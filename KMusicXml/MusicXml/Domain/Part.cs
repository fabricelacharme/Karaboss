using System;
using System.Collections.Generic;
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
            _part.Volume = (int?)partlistElement.Descendants("volume").FirstOrDefault() ?? 80;
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
                                var rest = childnode.Descendants("rest").FirstOrDefault();
                                var step = childnode.Descendants("step").FirstOrDefault();
                                var alter = childnode.Descendants("alter").FirstOrDefault();
                                var octave = childnode.Descendants("octave").FirstOrDefault();
                                var duration = childnode.Descendants("duration").FirstOrDefault();
                                var chord = childnode.Descendants("chord").FirstOrDefault();
                                var voice = childnode.Descendants("voice").FirstOrDefault();
                                var staff = childnode.Descendants("staff").FirstOrDefault();

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
                                {
                                    note.IsChordTone = true;
                                }

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


                    #region deleteme
                    /*
                    // For each measure, search for notes and backup, forward elements
                    foreach (var measureElement in partElement.Descendants("measure"))
                    {
                        Measure curMeasure = new Measure();

                        // Attributes containing everything
                        curMeasure.Attributes = new MeasureAttributes();

                        #region measure number (to be improved)
                        // ===================================
                        // Measure number
                        // ===================================
                        try
                        {
                            curMeasure.Number = int.Parse(measureElement.Attribute("number").Value);
                        }
                        catch (Exception e)
                        {
                            
                           
                            break;
                        }
                        #endregion measure number

                        #region tempo
                        // TEMPO
                        try
                        {
                            int curTempo = (int?)doc.Descendants("measure")
                                .Where(m => int.Parse(m.Attribute("number").Value) == curMeasure.Number)
                                .Descendants("sound")
                                ?.FirstOrDefault(s => s.Attribute("tempo") != null)
                                ?.Attribute("tempo") ?? tempo;
                            tempo = curTempo;
                            curMeasure.Tempo = curTempo * 10000;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            curMeasure.Tempo = tempo * 10000;
                        }
                        #endregion tempo
                      
                        #region notes
                        // ==================================
                        // NOTES
                        // ==================================
                        var pitches = measureElement.Descendants("note")
                                      .Select(n => new
                                      {
                                          rest = n.Descendants("rest").FirstOrDefault(),
                                          step = n.Descendants("step").FirstOrDefault(),
                                          alter = n.Descendants("alter").FirstOrDefault(),
                                          octave = n.Descendants("octave").FirstOrDefault(),
                                          duration = n.Descendants("duration").FirstOrDefault(),
                                          chord = n.Descendants("chord").FirstOrDefault(),
                                          voice = n.Descendants("voice").FirstOrDefault(),
                                          staff = n.Descendants("staff").FirstOrDefault()
                                      });

                        

                        foreach (var pitch in pitches)
                        {
                            
                            var note = new Note();
                            MeasureElement trucmeasureElement = null;

                            if (pitch.staff != null)
                                note.Staff = int.Parse(pitch.staff.Value);

                            if (pitch.voice != null)
                            {
                                note.Voice = int.Parse(pitch.voice.Value);
                                // TODO : Notes belonging to another track in the same part
                                //if (note.Voice != _part.Voice)
                                //    break;
                            }

                            string rest = "";
                            if (pitch.rest != null)
                            {
                                rest = "REST";
                                note.IsRest = true;
                            }

                            string step = "";
                            if (pitch.step != null)
                            {
                                step = pitch.step.Value;
                                note.Pitch.Step = step[0];
                            }

                            string accidental = "";
                            if (pitch.alter != null)
                            {
                                switch (int.Parse(pitch.alter.Value))
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
                                note.Pitch.Alter = int.Parse(pitch.alter.Value);
                            }
                            note.Accidental = accidental;
                            

                            string octave = "";
                            if (pitch.octave != null)
                            {
                                octave = pitch.octave.Value;
                                note.Pitch.Octave = int.Parse(octave);
                            }

                            int duration = 1;
                            if (pitch.duration != null)
                            {
                                duration = int.Parse(pitch.duration.Value);
                                note.Duration = duration;
                            }

                            if (pitch.chord != null)
                            {
                                note.IsChordTone = true;
                            }

                            

                            curMeasure.Notes.Add("NOTE_" + rest + step + accidental + octave);

                            curMeasure.Durations.Add(duration);

                            trucmeasureElement = new MeasureElement { Type = MeasureElementType.Note, Element = note };
                            if (trucmeasureElement != null)
                                curMeasure.MeasureElements.Add(trucmeasureElement);      
                            
                        }
                        #endregion notes


                        #region backup
                            var backups = measureElement.Descendants("backup")
                        .Select(n => new
                        {
                            duration = n.Descendants("duration").FirstOrDefault()
                        });
                        
                        foreach (var bkp in backups)
                        {
                            if (bkp.duration != null)
                            {
                                MeasureElement trucmeasureElement = null;
                                var backup = new Backup();
                                backup.Duration = int.Parse(bkp.duration.Value);

                                trucmeasureElement = new MeasureElement { Type = MeasureElementType.Backup, Element = backup };
                                if (trucmeasureElement != null)
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                            }
                        }

                        #endregion backup


                        #region forward                        
                        var forwards = measureElement.Descendants("forward")
                        .Select(n => new
                        {
                            duration = n.Descendants("duration").FirstOrDefault()
                        });

                        foreach (var fwd in forwards)
                        {
                            if (fwd.duration != null)
                            {
                                MeasureElement trucmeasureElement = null;
                                var forward = new Forward();
                                forward.Duration = int.Parse(fwd.duration.Value);

                                trucmeasureElement = new MeasureElement { Type = MeasureElementType.Forward, Element = forward };
                                if (trucmeasureElement != null)
                                    curMeasure.MeasureElements.Add(trucmeasureElement);
                            }
                        }

                        #endregion forward


                        _part.Measures.Add(curMeasure);
                    }
                    */
                    #endregion deleteme

                    

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
      

    }
}
