using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LPressSetting(
    string? LPressSettingPrinter,
    int LPressSettingCopies,
    bool LPressSettingCollation,
    LPressRange LPressSettingRange,
    LPressOrientation LPressSettingOrientation,
    LPressPaper LPressSettingPaper,
    LPressMargin LPressSettingMargin,
    int LPressSettingScale,
    LPressColor LPressSettingColor,
    LPressSide LPressSettingSide,
    bool LPressSettingBackground)
{
    public static LPressSetting LPressSettingCreate(LPressPrinter? printer)
    {
        LPressSetting setting = new(
            printer?.LPressPrinterName,
            1,
            true,
            new LPressRange(string.Empty),
            LPressOrientation.LPressOrientationPortrait,
            LPressPaper.LPressPaperLocal,
            LPressMargin.LPressMarginDefault,
            100,
            LPressColor.LPressColorDefault,
            LPressSide.LPressSideDefault,
            true);

        return setting.LPressSettingClamp(printer);
    }

    public LPressSetting LPressSettingClamp(LPressPrinter? printer)
    {
        bool known = printer is { LPressPrinterKnown: true };
        int most = printer is null ? 1 : Math.Clamp(printer.LPressPrinterCopies ?? 999, 1, 999);

        IReadOnlyList<LPressPaper> papers = LPressPaper.LPressPaperRead(printer);
        LPressPaper paper =
            (LPressSettingPaper is null ? null : papers.FirstOrDefault(LPressSettingPaper.LPressPaperMatch))
            ?? papers.FirstOrDefault(LPressPaper.LPressPaperLocal.LPressPaperMatch)
            ?? papers[0];

        LPressOrientation orientation = Enum.IsDefined(LPressSettingOrientation)
            ? LPressSettingOrientation
            : LPressOrientation.LPressOrientationPortrait;
        bool landscape = orientation == LPressOrientation.LPressOrientationLandscape;
        double width = landscape ? paper.LPressPaperHeight : paper.LPressPaperWidth;
        double height = landscape ? paper.LPressPaperWidth : paper.LPressPaperHeight;

        LPressMargin? margin = LPressSettingMargin;
        bool fits = margin is not null
            && new[]
            {
                margin.LPressMarginTop, margin.LPressMarginBottom, margin.LPressMarginLeft, margin.LPressMarginRight,
            }.All(value => double.IsFinite(value) && value >= 0.0)
            && margin.LPressMarginTop + margin.LPressMarginBottom < height
            && margin.LPressMarginLeft + margin.LPressMarginRight < width;

        LPressColor color = LPressSettingColor == LPressColor.LPressColorDefault
            || (known && printer!.LPressPrinterColor.Contains(LPressSettingColor))
                ? LPressSettingColor
                : LPressColor.LPressColorDefault;
        LPressSide side = LPressSettingSide == LPressSide.LPressSideDefault
            || (known && printer!.LPressPrinterSide.Contains(LPressSettingSide))
                ? LPressSettingSide
                : LPressSide.LPressSideDefault;

        return this with
        {
            LPressSettingPrinter = printer?.LPressPrinterName,
            LPressSettingCopies = Math.Clamp(LPressSettingCopies, 1, most),
            LPressSettingCollation = LPressSettingCollation && printer is { LPressPrinterCollation: true },
            LPressSettingRange = LPressSettingRange ?? new LPressRange(string.Empty),
            LPressSettingOrientation = orientation,
            LPressSettingPaper = paper,
            LPressSettingMargin = fits ? margin! : LPressMargin.LPressMarginDefault,
            LPressSettingScale = Math.Clamp(LPressSettingScale, 10, 200),
            LPressSettingColor = printer is null ? LPressColor.LPressColorDefault : color,
            LPressSettingSide = printer is null ? LPressSide.LPressSideDefault : side,
        };
    }

    public bool LPressSettingMatch(LPressSetting other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return LPressSettingOrientation == other.LPressSettingOrientation
            && LPressSettingPaper.LPressPaperMatch(other.LPressSettingPaper)
            && LPressSettingMargin == other.LPressSettingMargin
            && LPressSettingScale == other.LPressSettingScale
            && LPressSettingBackground == other.LPressSettingBackground;
    }
}
