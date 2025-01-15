using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaboss.Interaction
{
    internal interface ITempoMapValuesCache
    {
        #region Properties

        IEnumerable<TempoMapLine> InvalidateOnLines { get; }

        #endregion

        #region Methods

        void Invalidate(TempoMap tempoMap);

        #endregion
    }
}
