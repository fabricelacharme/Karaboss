using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXml.Domain;
using Sanford.Multimedia.Midi;

namespace MusicXml
{
    public class MusicXmlReader
    {
        private Sequence sequence;

        // Constructor
        public MusicXmlReader() 
        {
            MidiTags.ResetTags();
        }

        /// <summary>
        /// Read MusicXml score and convert to Midi
        /// </summary>
        /// <param name="SC"></param>
        /// <returns></returns>
        public Sequence Read(MusicXml.Domain.Score SC) 
        { 
            //Debug.Print(SC.ToString());
            String Name = null;
            string Id = null;
            
            Identification Identification = SC.Identification;
            String MovementTitle = SC.MovementTitle;
            
            // List of tracks
            List<Part> Parts = SC.Parts;
            foreach (Part part in Parts)
            {
                Name = part.Name.Trim();
                Id = part.Id.Trim();
                int MidiChannel = part.MidiChannel;
                int MidiProgram = part.MidiProgram;
                int Volume = part.Volume;
                int Pan = part.Pan;

                List<Measure> Measures = part.Measures;
                foreach (Measure measure in Measures)
                {
                    decimal W = measure.Width;
                    MeasureAttributes measureAttributes = measure.Attributes;
                    if (measureAttributes != null)
                    {
                        int division = measureAttributes.Divisions;
                        Time t = measureAttributes.Time;
                        Clef clef = measureAttributes.Clef;
                        Key key = measureAttributes.Key;                        
                    }

                    List<MeasureElement> lstME = measure.MeasureElements;   
                    foreach (MeasureElement measureElement in lstME)
                    {
                        object obj = measureElement.Element;
                        MeasureElementType metype = measureElement.Type;

                        switch (metype)
                        {
                            case MeasureElementType.Backup:break;
                            case MeasureElementType.Note: 
                                Note note = (Note)obj;
                                string accidental = note.Accidental;
                                int staff = note.Staff;
                                bool isrest = note.IsRest;
                                bool ischordtone = note.IsChordTone;
                                Pitch pitch = note.Pitch;
                                int voice = note.Voice;
                                Lyric lyric = note.Lyric;
                                string ntype = note.Type;
                                int duration = note.Duration;

                                break;
                            case MeasureElementType.Forward:break;                               

                        }
                    }

                }
                

            }

            return sequence;
        }



    }
}
