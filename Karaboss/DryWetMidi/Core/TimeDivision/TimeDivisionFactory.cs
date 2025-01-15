using Karaboss.DryWetMidi.Common;

namespace Karaboss.DryWetMidi.Core
{
    internal static class TimeDivisionFactory
    {
        #region Methods

        internal static TimeDivision GetTimeDivision(short division)
        {
            if (division < 0)
            {
                return new SmpteTimeDivision((SmpteFormat)(-division.GetHead()), division.GetTail());
            }

            return new TicksPerQuarterNoteTimeDivision(division);
        }

        #endregion
    }
}
