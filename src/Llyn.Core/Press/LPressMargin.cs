using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPressMargin(
    double LPressMarginTop,
    double LPressMarginBottom,
    double LPressMarginLeft,
    double LPressMarginRight,
    string? LPressMarginName)
{
    private const double LPressMarginTolerance = 0.0001;

    public static LPressMargin LPressMarginDefault { get; } =
        new(12.0 / 25.4, 12.0 / 25.4, 12.0 / 25.4, 12.0 / 25.4, "Default");

    public static LPressMargin LPressMarginNone { get; } = new(0.0, 0.0, 0.0, 0.0, "None");

    public static LPressMargin LPressMarginNarrow { get; } = new(0.25, 0.25, 0.25, 0.25, "Narrow");

    public static LPressMargin LPressMarginWide { get; } = new(1.0, 1.0, 1.0, 1.0, "Wide");

    public static IReadOnlyList<LPressMargin> LPressMarginPreset { get; } =
        new[] { LPressMarginDefault, LPressMarginNone, LPressMarginNarrow, LPressMarginWide };

    public static LPressMargin LPressMarginResolve(double top, double bottom, double left, double right, bool metric)
    {
        double unit = metric ? 25.4 : 1.0;
        Func<double, double> inch = value => double.IsFinite(value) && value > 0.0 ? value / unit : 0.0;
        LPressMargin margin = new(inch(top), inch(bottom), inch(left), inch(right), null);
        foreach (LPressMargin preset in LPressMarginPreset)
        {
            if (Math.Abs(preset.LPressMarginTop - margin.LPressMarginTop) < LPressMarginTolerance
                && Math.Abs(preset.LPressMarginBottom - margin.LPressMarginBottom) < LPressMarginTolerance
                && Math.Abs(preset.LPressMarginLeft - margin.LPressMarginLeft) < LPressMarginTolerance
                && Math.Abs(preset.LPressMarginRight - margin.LPressMarginRight) < LPressMarginTolerance)
            {
                return preset;
            }
        }

        return margin;
    }
}
