using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXml.Domain
{
    public class TempoChange
    {

        internal TempoChange()
        {

            // Fab
            Tempo = 0;
        }

        public float Tempo { get; internal set; }
    }
}
