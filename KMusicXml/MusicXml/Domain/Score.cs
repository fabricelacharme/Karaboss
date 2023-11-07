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

        /// <summary>
        /// Create score
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Score Create(XDocument doc)
        {
            Score _score = new Score();

            foreach (var partlistElement in doc.Descendants("score-part"))
            {
                _score.PartList.Add(Part.Create(doc, partlistElement));                             
            }            

            return _score;
        }

    }
}
