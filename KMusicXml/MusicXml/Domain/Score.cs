using System.Collections.Generic;
using System.Xml.Linq;

namespace MusicXml.Domain
{
	public class Score
	{
		// équivalent de piece
		public Score()
		{		
			MovementTitle = string.Empty;
		}

		public string MovementTitle { get; set; }

		public Identification Identification { get; set; }

		public List<Part> Parts { get; set; }
        public List<Part> PartList { get; set; } = new List<Part>();

        public static Score Create(XDocument doc)
        {
            Score _score = new Score();

            foreach (var partlistElement in doc.Descendants("score-part"))
            {
                _score.PartList.Add(Part.CreateInit(doc, partlistElement));
                
                /*
                // Check if a part contains 2 tracks
                Part _part = _score.PartList[_score.PartList.Count - 1];
                if (_part.Staves > 1)
                {
                    Part _cpart = Part.Clone(_part);
                    _score.PartList.Add(_cpart);
                }
                */
            }

            

            /*
            foreach (var partElement in doc.Descendants("part"))
            {
                _score.PartList.Add(Part.Create(doc, partElement));
            }
            */
            
            

            return _score;
        }

    }
}
