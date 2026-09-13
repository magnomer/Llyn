using Llyn.Core;

namespace Llyn.UIShell;

internal static class PFrequencyLabel
{
    internal static string PFrequencyLabelFormat(LFrequency frequency)
    {
        return frequency.LFrequencyBand ?? frequency.LFrequencyRaw;
    }

    internal static string PFrequencySourceFormat(LFrequency frequency, string unit)
    {
        string figure = frequency.LFrequencyNumeric
            ? frequency.LFrequencyRaw + " " + unit
            : frequency.LFrequencyRaw;
        return frequency.LFrequencySource + ": " + figure;
    }
}
