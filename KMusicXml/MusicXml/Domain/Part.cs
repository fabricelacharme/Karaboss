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

        public int Numerator { get; internal set; }
        public int Denominator { get; internal set; }

        public Part()
		{
			Id = string.Empty;						
			// Fab
			MidiChannel = 1;
			MidiProgram = 1;
			Volume = 80;
			Pan = 0;
            
		}

        /// <summary>
        /// Clone existing Part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static Part Clone(Part part)
        {
            Part _part = new Part();

            _part.Id = part.Id;
            _part.MidiChannel = part.MidiChannel;
            _part.MidiProgram = part.MidiProgram;
            _part.Volume = part.Volume;
            _part.Pan = part.Pan;

            _part.Numerator = part.Numerator;
            _part.Denominator = part.Denominator;
            _part.Division = part.Division;
            
            return _part;
        }


        public static Part CreateList(XDocument doc, XElement partlistElement)
        {
            Part _part = new Part();

            string id = partlistElement.Attributes("id").FirstOrDefault()?.Value;
            _part.Id = id;

            _part.MidiChannel = (int?)partlistElement.Descendants("midi-channel").FirstOrDefault() ?? 1;
            _part.MidiProgram = (int?)partlistElement.Descendants("midi-program").FirstOrDefault() ?? 1;
            _part.Volume = (int?)partlistElement.Descendants("volume").FirstOrDefault() ?? 80;
            _part.Pan = (int?)partlistElement.Descendants("pan").FirstOrDefault() ?? 0;

            int tempo = 100;

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

                    // Check we are in the good track
                    int initcurrentvoice = 1;
                    int currentvoice = 1;

                    foreach (var measureElement in partElement.Descendants("measure"))
                    {

                        // Stop if 'staves' element
                        if (currentvoice != initcurrentvoice)
                            break;

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

                            /* 
                             * TODO
                             * Sometimes number = X1
                             * case of repeat 
                             * 
                             * 
                             * 
                            */
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


                    }
                }
            }
            
            return _part;   
        }


        /// <summary>
        /// Load everything about a part
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="partlistElement"></param>
        /// <returns></returns>
        public static Part CreateInit(XDocument doc, XElement partlistElement)
        {
            Part _part = new Part();

            string id = partlistElement.Attributes("id").FirstOrDefault()?.Value;
            _part.Id = id;

            _part.MidiChannel = (int?)partlistElement.Descendants("midi-channel").FirstOrDefault() ?? 1;
            _part.MidiProgram = (int?)partlistElement.Descendants("midi-program").FirstOrDefault() ?? 1;
            _part.Volume = (int?)partlistElement.Descendants("volume").FirstOrDefault() ?? 80;
            _part.Pan = (int?)partlistElement.Descendants("pan").FirstOrDefault() ?? 0;

            //String measuresXpath = string.Format("//part[@id='{0}']/measure", _part.Id);
            //XNode N =  doc.XPathSelectElement(measuresXpath);
            
            int tempo = 100;

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

                    
                    // Check we are in the good track
                    //int currentvoice = _part.Voice;

                    foreach (var measureElement in partElement.Descendants("measure"))
                    {
                        // Stop if 'staves' element
                        //if (currentvoice != _part.Voice)
                        //    break;

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
                            
                            /* 
                             * TODO
                             * Sometimes number = X1
                             * case of repeat 
                             * 
                             * 
                             * 
                            */
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
                        /*
                         *  voir https://www.w3.org/2021/06/musicxml40/tutorial/midi-compatible-part/
                         *  backup et forward
                         * 
                         */
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


        #region deleteme

        /*
        public static Part Create(XDocument doc, XElement partElement)
        {
            Part _part = new Part();

            bool result;


            _part.Division = (int?)partElement.Descendants("divisions").FirstOrDefault() ?? 4;

            int tempo = 100;

            foreach (var measureElement in partElement.Descendants("measure"))
            {
                Measure curMeasure = new Measure();

                curMeasure.Number = int.Parse(measureElement.Attribute("number").Value);

                //int curTempo;
                //result = int.TryParse(doc.Descendants("measure")
                //    .Where(m => int.Parse(m.Attribute("number").Value) == curMeasure.Number)
                //    .Descendants("sound")
                //    ?.FirstOrDefault(s => s.Attribute("tempo") != null)
                //    ?.Attribute("tempo").Value, out curTempo);
                //if (result)
                //{
                //    tempo = curTempo;
                //}
                //else
                //{
                //    curTempo = tempo;
                //} 
                //curMeasure.Tempo = curTempo;

                int curTempo = (int?)doc.Descendants("measure")
                    .Where(m => int.Parse(m.Attribute("number").Value) == curMeasure.Number)
                    .Descendants("sound")
                    ?.FirstOrDefault(s => s.Attribute("tempo") != null)
                    ?.Attribute("tempo") ?? tempo;
                tempo = curTempo;
                curMeasure.Tempo = curTempo;


                //var pitches = from pitch in measureElement.Descendants("note")
                //              select new
                //              {
                //                  rest = pitch.Descendants("rest").FirstOrDefault(),
                //                  step = pitch.Descendants("step").FirstOrDefault(),
                //                  alter = pitch.Descendants("alter").FirstOrDefault(),
                //                  octave = pitch.Descendants("octave").FirstOrDefault(),
                //                  duration = pitch.Descendants("duration").FirstOrDefault()
                //              };

                var pitches = measureElement.Descendants("note")
                              .Select(n => new
                              {
                                  rest = n.Descendants("rest").FirstOrDefault(),
                                  step = n.Descendants("step").FirstOrDefault(),
                                  alter = n.Descendants("alter").FirstOrDefault(),
                                  octave = n.Descendants("octave").FirstOrDefault(),
                                  duration = n.Descendants("duration").FirstOrDefault()
                              });

                foreach (var pitch in pitches)
                {
                    string rest = "";
                    if (pitch.rest != null)
                    {
                        rest = "REST";
                    }

                    string step = "";
                    if (pitch.step != null)
                    {
                        step = pitch.step.Value;
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
                    }

                    string octave = "";
                    if (pitch.octave != null)
                    {
                        octave = pitch.octave.Value;
                    }

                    int duration = 1;
                    if (pitch.duration != null)
                    {
                        duration = int.Parse(pitch.duration.Value);
                    }

                    curMeasure.Notes.Add("NOTE_" + rest + step + accidental + octave);

                    curMeasure.Durations.Add(duration);
                }

                _part.Measures.Add(curMeasure);
            }
            string id = partElement.Attributes("id").FirstOrDefault()?.Value;
            if (id == null)
            {
                id = "NO ID";
            }
            _part.ID = id;

            string name = doc.Descendants("score-part").Where(p => String.Equals(p.Attribute("id").Value.ToString(), id)).FirstOrDefault()?.Descendants("part-name")?.FirstOrDefault().Value;
            if (name == null)
            {
                name = "NO NAME";
            }
            _part.Name = name;

            //var pitches = from pitch in partElement.Descendants("note")
            //              select new
            //              {
            //                  rest = pitch.Descendants("rest").FirstOrDefault(),
            //                  step = pitch.Descendants("step").FirstOrDefault(),
            //                  alter = pitch.Descendants("alter").FirstOrDefault(),
            //                  octave = pitch.Descendants("octave").FirstOrDefault(),
            //                  duration = pitch.Descendants("duration").FirstOrDefault()
            //              };

            //foreach (var pitch in pitches)
            //{
            //    string rest = "";
            //    if (pitch.rest != null)
            //    {
            //        rest = "REST";
            //    }

            //    string step = "";
            //    if (pitch.step != null)
            //    {
            //        step = pitch.step.Value;
            //    }

            //    string accidental = "";
            //    if (pitch.alter != null)
            //    {
            //        switch (int.Parse(pitch.alter.Value))
            //        {
            //            case 1:
            //                accidental = "S";
            //                break;
            //            case -1:
            //                accidental = "F";
            //                break;
            //            default:
            //                break;
            //        }
            //    }

            //    string octave = "";
            //    if (pitch.octave != null)
            //    {
            //        octave = pitch.octave.Value;
            //    }

            //    int duration = 1;
            //    if (pitch.duration != null)
            //    {
            //        duration = int.Parse(pitch.duration.Value);
            //    }

            //    _part.Notes.Add("NOTE_" + rest + step + accidental + octave);

            //    _part.Durations.Add(duration);
            //}

            _part.Raw = partElement.ToString();           

            return _part;
        }
        */
        
        #endregion deleteme

    }
}
