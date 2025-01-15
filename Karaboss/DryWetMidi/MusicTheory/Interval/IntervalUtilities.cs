using Karaboss.DryWetMidi.Common;

namespace Karaboss.DryWetMidi.MusicTheory
{
    internal static class IntervalUtilities
    {
        #region Methods

        internal static bool IsIntervalValid(int halfSteps)
        {
            return halfSteps >= -SevenBitNumber.MaxValue && halfSteps <= SevenBitNumber.MaxValue;
        }

        #endregion
    }
}
