using System.Globalization;

namespace Llyn.Core;

public sealed record LPressPaper(double LPressPaperWidth, double LPressPaperHeight)
{
    public static LPressPaper LPressPaperLetter { get; } = new(8.5, 11.0);

    public static LPressPaper LPressPaperMetric { get; } = new(8.27, 11.69);

    public static LPressPaper LPressPaperLocal =>
        RegionInfo.CurrentRegion.IsMetric ? LPressPaperMetric : LPressPaperLetter;
}
