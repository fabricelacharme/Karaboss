using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXml.Domain
{

    public enum EndingTypes
    {
        start,
        stop
    }

    public class Ending
    {
        public int Number { get; internal set; }
        public EndingTypes Type { get; internal set; }

    }
}
