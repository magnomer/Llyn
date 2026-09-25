using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LFrequency(
    string LFrequencySource,
    string LFrequencyRaw,
    string? LFrequencyBand,
    long? LFrequencyOnce = null,
    string? LFrequencyUnit = null)
{
    private static readonly (double LFrequencyLimit, string LFrequencyName)[] LFrequencyBands =
    [
        (10000, "Core"),
        (100000, "Everyday"),
        (1000000, "Advanced"),
    ];

    private const string LFrequencyRare = "Rare";

    private static readonly string[] LFrequencyRanks =
        [LFrequencyRare, .. LFrequencyBands.Select(static band => band.LFrequencyName).Reverse()];

    public static IReadOnlyList<string> LFrequencyScale => LFrequencyRanks;

    public int LFrequencyRank => LFrequencyBand is null ? 0 : Array.IndexOf(LFrequencyRanks, LFrequencyBand) + 1;

    public string LFrequencyFigure => LFrequencyUnit is string unit ? unit + " " + LFrequencyRaw : LFrequencyRaw;

    public static string? LFrequencyBandResolve(LSourceSpec spec, double raw)
    {
        if (LFrequencyOnceRead(spec, raw) is not double interval)
        {
            return null;
        }

        foreach ((double limit, string name) in LFrequencyBands)
        {
            if (interval <= limit)
            {
                return name;
            }
        }

        return LFrequencyRare;
    }

    public static long? LFrequencyOnceResolve(LSourceSpec spec, double raw)
    {
        if (LFrequencyOnceRead(spec, raw) is not double interval)
        {
            return null;
        }

        if (interval < 1)
        {
            return 1;
        }

        int digits = (int)Math.Floor(Math.Log10(interval)) - 1;
        double step = Math.Pow(10, Math.Max(digits, 0));
        return (long)(Math.Round(interval / step) * step);
    }

    public static double? LFrequencyOnceRead(LSourceSpec spec, double raw)
    {
        ArgumentNullException.ThrowIfNull(spec);

        double interval;
        if (spec.LSourceSpecTotal is double total && raw > 0)
        {
            interval = total / raw;
        }
        else if (spec.LSourceSpecFactor is double factor && spec.LSourceSpecBase is double power)
        {
            interval = factor * Math.Pow(power, raw);
        }
        else if (spec.LSourceSpecFactor is double scale)
        {
            interval = scale * raw;
        }
        else
        {
            return null;
        }

        return double.IsNaN(interval) || double.IsInfinity(interval) || interval < 0 ? null : interval;
    }
}
