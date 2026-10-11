using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LPressPaper(double LPressPaperWidth, double LPressPaperHeight, string? LPressPaperName = null)
{
    private const double LPressPaperTolerance = 0.01;

    public static LPressPaper LPressPaperLetter { get; } = new(8.5, 11.0, "Letter");

    public static LPressPaper LPressPaperMetric { get; } = new(8.27, 11.69, "A4");

    public static LPressPaper LPressPaperLocal =>
        RegionInfo.CurrentRegion.IsMetric ? LPressPaperMetric : LPressPaperLetter;

    public static IReadOnlyList<LPressPaper> LPressPaperStandard { get; } = new[]
    {
        new LPressPaper(11.69, 16.54, "A3"),
        LPressPaperMetric,
        new LPressPaper(5.83, 8.27, "A5"),
        new LPressPaper(6.93, 9.84, "B5"),
        LPressPaperLetter,
        new LPressPaper(8.5, 14.0, "Legal"),
        new LPressPaper(11.0, 17.0, "Tabloid"),
    };

    public static IReadOnlyList<LPressPaper> LPressPaperRead(LPressPrinter? printer)
    {
        return printer is { LPressPrinterKnown: true, LPressPrinterPaper.Count: > 0 }
            ? printer.LPressPrinterPaper
            : LPressPaperStandard;
    }

    public bool LPressPaperMatch(LPressPaper other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Math.Abs(LPressPaperWidth - other.LPressPaperWidth) < LPressPaperTolerance
            && Math.Abs(LPressPaperHeight - other.LPressPaperHeight) < LPressPaperTolerance;
    }
}
