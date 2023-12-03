using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordsAnalyser.cmt_exception
{
    public class mt_exceptions
    {
        public class Error: Exception { }

    


        public class FormatError: Error { }

    


        public class NoteFormatError: Error { }

    


        public class KeyError:Error { }

    


        public class RangeError: Error { }

    


        public class FingerError: Error { }

    

    }
}
