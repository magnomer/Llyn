using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed record LGalleyView(
    IReadOnlyList<LGalleyDestination> LGalleyViewPrinter,
    IReadOnlyList<LGalleyStock> LGalleyViewPaper,
    int? LGalleyViewCopies,
    bool? LGalleyViewCollation,
    LPressScope LGalleyViewScope,
    string? LGalleyViewText,
    bool LGalleyViewValid,
    IReadOnlyList<bool> LGalleyViewIncluded,
    LPressOrientation LGalleyViewOrientation,
    string? LGalleyViewMargin,
    double? LGalleyViewTop,
    double? LGalleyViewBottom,
    double? LGalleyViewLeft,
    double? LGalleyViewRight,
    bool LGalleyViewMetric,
    int LGalleyViewScale,
    LPressColor? LGalleyViewColor,
    LPressSide? LGalleyViewSide,
    bool LGalleyViewBackground,
    bool LGalleyViewPdf,
    bool LGalleyViewKnown,
    int? LGalleyViewSheets,
    bool LGalleyViewStale)
{
    public static LGalleyView LGalleyViewRead(LGalley galley)
    {
        ArgumentNullException.ThrowIfNull(galley);

        bool typed = galley.LGalleyScope == LPressScope.LPressScopeCustom;
        LPressSetting setting = galley.LGalleySetting;
        LPressDigest digest = galley.LGalleyDigest;
        LPressPrinter? printer = digest.LPressDigestPrinter;
        bool export = printer is null;

        List<LGalleyDestination> printers = [new LGalleyDestination(null, false, export)];
        printers.AddRange(galley.LGalleyPrinter.Select(item => new LGalleyDestination(
            item.LPressPrinterName,
            item.LPressPrinterDefault,
            item == printer)));

        List<LGalleyStock> papers = digest.LPressDigestPaper
            .Select((paper, index) => new LGalleyStock(
                paper.LPressPaperName,
                paper.LPressPaperWidth,
                paper.LPressPaperHeight,
                index == digest.LPressDigestChosen))
            .ToList();

        LPressMargin margin = galley.LGalleyMargin;
        bool custom = margin.LPressMarginName is null;

        return new LGalleyView(
            printers,
            papers,
            export ? null : setting.LPressSettingCopies,
            export || setting.LPressSettingCopies == 1 || !printer!.LPressPrinterCollation
                ? null
                : setting.LPressSettingCollation,
            galley.LGalleyScope,
            typed ? galley.LGalleyRange : null,
            !typed || galley.LGalleyRangeValid,
            digest.LPressDigestIncluded,
            setting.LPressSettingOrientation,
            margin.LPressMarginName,
            custom ? margin.LPressMarginTop : null,
            custom ? margin.LPressMarginBottom : null,
            custom ? margin.LPressMarginLeft : null,
            custom ? margin.LPressMarginRight : null,
            galley.LGalleyMetric,
            setting.LPressSettingScale,
            export
                ? null
                : setting.LPressSettingColor == LPressColor.LPressColorDefault
                    ? LPressColor.LPressColorFull
                    : setting.LPressSettingColor,
            export
                ? null
                : setting.LPressSettingSide == LPressSide.LPressSideDefault
                    ? LPressSide.LPressSideSingle
                    : setting.LPressSettingSide,
            setting.LPressSettingBackground,
            export,
            printer is not { LPressPrinterKnown: false },
            export ? null : digest.LPressDigestSheets,
            galley.LGalleyStale);
    }
}
