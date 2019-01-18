using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzLyrics.Api
{
    public abstract class Contribuitor
    {
        public string Name { get; set; }
        public Contribuitor(string name)
        {
            Name = name;
        }
    }
}
