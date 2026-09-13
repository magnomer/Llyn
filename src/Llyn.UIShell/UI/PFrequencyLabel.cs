using Llyn.Core;

namespace Llyn.UIShell;

internal static class PFrequencyLabel
{
    internal static string PFrequencyLabelName(LFrequency frequency)
    {
        return frequency.LFrequencyBand ?? frequency.LFrequencyRaw;
    }

    internal static string PFrequencyLabelTip(LFrequency frequency, string unit)
    {
        string figure = frequency.LFrequencyNumeric
            ? frequency.LFrequencyRaw + " " + unit
            : frequency.LFrequencyRaw;
        return frequency.LFrequencySource + ": " + figure;
    }
}
