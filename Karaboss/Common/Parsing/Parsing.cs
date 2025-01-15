using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaboss.Common
{
    internal delegate ParsingResult Parsing<T>(string input, out T result);
}
