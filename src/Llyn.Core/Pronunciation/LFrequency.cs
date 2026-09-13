using System.Globalization;

namespace Llyn.Core;

public sealed record LFrequency(string LFrequencySource, string LFrequencyRaw, string? LFrequencyBand)
{
    public bool LFrequencyNumeric =>
        double.TryParse(LFrequencyRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

    public static LFrequency LFrequencyParse(string stored)
    {
        int separator = stored.IndexOf('|');
        if (separator < 0)
        {
            return new LFrequency(string.Empty, stored, null);
        }

        return new LFrequency(stored[..separator], stored[(separator + 1)..], null);
    }

    public string LFrequencyFormat()
    {
        return $"{LFrequencySource}|{LFrequencyRaw}";
    }
}
