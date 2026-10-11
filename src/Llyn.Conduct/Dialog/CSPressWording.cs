using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CSPressWording
{
    internal static CSPressSetting LSPressWordingRead(LGalleyView view, int page, bool running)
    {
        return new CSPressSetting(
            view.LGalleyViewPrinter.Select(LSPressWordingRead).ToList(),
            view.LGalleyViewPaper.Select(LSPressWordingRead).ToList(),
            view.LGalleyViewCopies,
            view.LGalleyViewCollation,
            LSPressWordingRead(view.LGalleyViewScope),
            view.LGalleyViewText,
            view.LGalleyViewValid ? null : "Print.PagesInvalid",
            view.LGalleyViewIncluded,
            page,
            view.LGalleyViewOrientation switch
            {
                LPressOrientation.LPressOrientationPortrait => CSPressOrientation.CSPressOrientationPortrait,
                LPressOrientation.LPressOrientationLandscape => CSPressOrientation.CSPressOrientationLandscape,
                _ => throw new ArgumentOutOfRangeException(nameof(view), view.LGalleyViewOrientation, null),
            },
            LSPressWordingRead(view.LGalleyViewMargin),
            view.LGalleyViewTop,
            view.LGalleyViewBottom,
            view.LGalleyViewLeft,
            view.LGalleyViewRight,
            view.LGalleyViewMetric ? "Print.UnitMillimeter" : "Print.UnitInch",
            view.LGalleyViewScale,
            view.LGalleyViewColor switch
            {
                null => null,
                LPressColor.LPressColorFull => CSPressColor.CSPressColorFull,
                LPressColor.LPressColorGray => CSPressColor.CSPressColorGray,
                _ => throw new ArgumentOutOfRangeException(nameof(view), view.LGalleyViewColor, null),
            },
            LSPressWordingRead(view.LGalleyViewSide),
            view.LGalleyViewBackground,
            view.LGalleyViewPdf ? "Print.Save" : "Print.Print",
            view.LGalleyViewKnown ? null : "Print.CapabilityUnknown",
            view.LGalleyViewSheets,
            view.LGalleyViewSheets == 1 ? "Print.SheetsOne" : "Print.Sheets",
            view.LGalleyViewStale,
            !running,
            running ? "Print.Printing" : "Print.Idle");
    }

    internal static string? LSPressWordingRead(LPressStatus status)
    {
        return status switch
        {
            LPressStatus.LPressStatusDone => null,
            LPressStatus.LPressStatusOffline => "Print.PrinterUnavailable",
            LPressStatus.LPressStatusError => "Print.Failed",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
        };
    }

    private static CSPressPrinter LSPressWordingRead(LGalleyDestination printer)
    {
        return new CSPressPrinter(
            printer.LGalleyDestinationName,
            printer switch
            {
                { LGalleyDestinationName: null } => "Print.SaveAsPdf",
                { LGalleyDestinationDefault: true } => "Print.PrinterDefault",
                { LGalleyDestinationName: string name } => name
                    .Replace("{", "{{", StringComparison.Ordinal)
                    .Replace("}", "}}", StringComparison.Ordinal),
            },
            printer.LGalleyDestinationChosen);
    }

    private static CSPressPaper LSPressWordingRead(LGalleyStock paper)
    {
        return new CSPressPaper(
            paper.LGalleyStockName switch
            {
                null => "Print.PaperCustom",
                "A3" => "Print.PaperA3",
                "A4" => "Print.PaperA4",
                "A5" => "Print.PaperA5",
                "B5" => "Print.PaperB5",
                "Letter" => "Print.PaperLetter",
                "Legal" => "Print.PaperLegal",
                "Tabloid" => "Print.PaperTabloid",
                _ => throw new InvalidOperationException("The sheet carries an unknown standard name."),
            },
            paper.LGalleyStockWidth,
            paper.LGalleyStockHeight,
            paper.LGalleyStockChosen);
    }

    private static List<CSPressChoice<CSPressRange>> LSPressWordingRead(LPressScope scope)
    {
        CSPressRange chosen = scope switch
        {
            LPressScope.LPressScopeAll => CSPressRange.CSPressRangeAll,
            LPressScope.LPressScopeCurrent => CSPressRange.CSPressRangeCurrent,
            LPressScope.LPressScopeCustom => CSPressRange.CSPressRangeCustom,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null),
        };
        return Enum.GetValues<CSPressRange>()
            .Select(range => new CSPressChoice<CSPressRange>(
                range,
                range switch
                {
                    CSPressRange.CSPressRangeAll => "Print.PagesAll",
                    CSPressRange.CSPressRangeCurrent => "Print.PagesCurrent",
                    CSPressRange.CSPressRangeCustom => "Print.PagesCustom",
                    _ => throw new ArgumentOutOfRangeException(nameof(scope), range, null),
                },
                range == chosen))
            .ToList();
    }

    private static List<CSPressChoice<CSPressMargin>> LSPressWordingRead(string? name)
    {
        CSPressMargin chosen = name switch
        {
            null => CSPressMargin.CSPressMarginCustom,
            "Default" => CSPressMargin.CSPressMarginDefault,
            "None" => CSPressMargin.CSPressMarginNone,
            "Narrow" => CSPressMargin.CSPressMarginNarrow,
            "Wide" => CSPressMargin.CSPressMarginWide,
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };
        return Enum.GetValues<CSPressMargin>()
            .Select(margin => new CSPressChoice<CSPressMargin>(
                margin,
                margin switch
                {
                    CSPressMargin.CSPressMarginDefault => "Print.MarginsDefault",
                    CSPressMargin.CSPressMarginNone => "Print.MarginsNone",
                    CSPressMargin.CSPressMarginNarrow => "Print.MarginsNarrow",
                    CSPressMargin.CSPressMarginWide => "Print.MarginsWide",
                    CSPressMargin.CSPressMarginCustom => "Print.MarginsCustom",
                    _ => throw new ArgumentOutOfRangeException(nameof(name), margin, null),
                },
                margin == chosen))
            .ToList();
    }

    private static List<CSPressChoice<CSPressSide>>? LSPressWordingRead(LPressSide? side)
    {
        if (side is null)
        {
            return null;
        }

        CSPressSide chosen = side switch
        {
            LPressSide.LPressSideSingle => CSPressSide.CSPressSideSingle,
            LPressSide.LPressSideLong => CSPressSide.CSPressSideLong,
            LPressSide.LPressSideShort => CSPressSide.CSPressSideShort,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
        };
        return Enum.GetValues<CSPressSide>()
            .Select(item => new CSPressChoice<CSPressSide>(
                item,
                item switch
                {
                    CSPressSide.CSPressSideSingle => "Print.SideSingle",
                    CSPressSide.CSPressSideLong => "Print.SideLong",
                    CSPressSide.CSPressSideShort => "Print.SideShort",
                    _ => throw new ArgumentOutOfRangeException(nameof(side), item, null),
                },
                item == chosen))
            .ToList();
    }
}
