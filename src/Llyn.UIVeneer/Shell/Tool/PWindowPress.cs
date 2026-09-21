using System;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal async Task PWindowPressRun(Func<LPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        try
        {
            if (PWindowTicketRead() is LPressTicket ticket)
            {
                await print(ticket);
            }
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Print.Failed", exception);
        }
    }

    internal Task PWindowPressRun(Func<LPortraitLabel, LPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        return PWindowPressRun(ticket => print(PWindowLabelRead(), ticket));
    }

    internal LPressTicket? PWindowTicketRead()
    {
        PrintDialog dialog = new()
        {
            UserPageRangeEnabled = false,
        };

        if (dialog.ShowDialog() != true)
        {
            return null;
        }

        PrintTicket chosen = dialog.PrintTicket;

        return new LPressTicket(
            dialog.PrintQueue.FullName,
            PWindowPaperRead(chosen.PageMediaSize),
            chosen.PageOrientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape,
            chosen.CopyCount ?? 1,
            chosen.Collation != Collation.Uncollated,
            chosen.Duplexing switch
            {
                Duplexing.OneSided => LPressSide.LPressSideSingle,
                Duplexing.TwoSidedLongEdge => LPressSide.LPressSideLong,
                Duplexing.TwoSidedShortEdge => LPressSide.LPressSideShort,
                _ => LPressSide.LPressSideDefault,
            },
            chosen.OutputColor switch
            {
                OutputColor.Color => LPressInk.LPressInkColor,
                OutputColor.Grayscale or OutputColor.Monochrome => LPressInk.LPressInkGray,
                _ => LPressInk.LPressInkDefault,
            });
    }

    private static LPressPaper PWindowPaperRead(PageMediaSize? size)
    {
        if (size?.Width is not double width || size.Height is not double height || width <= 0 || height <= 0)
        {
            return LPressPaper.LPressPaperLocal;
        }

        return new LPressPaper(width / 96.0, height / 96.0);
    }

    internal LPortraitLabel PWindowLabelRead()
    {
        return new LPortraitLabel(
            PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
            PLocalizationCatalog.PLocalizationTextRead("Display.MeaningSingle"),
            PLocalizationCatalog.PLocalizationTextRead("Display.MeaningPlural"),
            PLocalizationCatalog.PLocalizationTextRead("Display.CollocationSingle"),
            PLocalizationCatalog.PLocalizationTextRead("Display.Collocation"),
            PLocalizationCatalog.PLocalizationTextRead("Display.Translated"),
            PLocalizationCatalog.PLocalizationTextRead("Display.Note"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Form"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Paradigm"),
            PLocalizationCatalog.PLocalizationTextRead("Frequency.Title"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Glyph"),
            PLocalizationCatalog.PLocalizationTextRead("Display.Script"),
            PLocalizationCatalog.PLocalizationTextRead("Display.Fanqie"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Example"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Gloss"),
            PLocalizationCatalog.PLocalizationTextRead("Reference.Title"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Mention"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Situation"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Register"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Translation"),
            PLocalizationCatalog.PLocalizationTextRead("Portrait.Tag"));
    }

    internal LPortraitLegend PWindowLegendRead(string realm)
    {
        Dictionary<LReferenceKind, string> kinds = [];
        foreach (LReferenceKind kind in System.Enum.GetValues<LReferenceKind>())
        {
            kinds[kind] = PLocalizationCatalog.PLocalizationTextRead(LReference.LReferenceKindResolve(kind));
        }

        return new LPortraitLegend(
            PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
            PLocalizationCatalog.PLocalizationTextRead(realm == "Example" ? "Example.Unwritten" : realm + ".Untitled"),
            PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten"),
            PLocalizationCatalog.PLocalizationTextRead(realm + ".UsageNone"),
            PLocalizationCatalog.PLocalizationTextRead(realm + ".UsageOne"),
            PLocalizationCatalog.PLocalizationTextRead(realm + ".UsageMany"),
            PLocalizationCatalog.PLocalizationTextRead("Example.Translation"),
            PLocalizationCatalog.PLocalizationTextRead("Reference.Title"),
            PLocalizationCatalog.PLocalizationTextRead("Source.Author"),
            PLocalizationCatalog.PLocalizationTextRead("Source.Year"),
            PLocalizationCatalog.PLocalizationTextRead("Source.Url"),
            PLocalizationCatalog.PLocalizationTextRead("Source.Note"),
            PLocalizationCatalog.PLocalizationTextRead("Situation.Description"),
            kinds);
    }
}
