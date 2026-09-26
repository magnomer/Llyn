using System;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

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
            QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
            QLocalizationCatalog.QLocalizationTextRead("Display.MeaningSingle"),
            QLocalizationCatalog.QLocalizationTextRead("Display.MeaningPlural"),
            QLocalizationCatalog.QLocalizationTextRead("Display.CollocationSingle"),
            QLocalizationCatalog.QLocalizationTextRead("Display.Collocation"),
            QLocalizationCatalog.QLocalizationTextRead("Display.Translated"),
            QLocalizationCatalog.QLocalizationTextRead("Display.Note"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Form"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Paradigm"),
            QLocalizationCatalog.QLocalizationTextRead("Frequency.Title"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Glyph"),
            QLocalizationCatalog.QLocalizationTextRead("Display.Script"),
            QLocalizationCatalog.QLocalizationTextRead("Display.Fanqie"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Example"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Gloss"),
            QLocalizationCatalog.QLocalizationTextRead("Reference.Title"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Mention"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Etymology"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Situation"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Register"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Translation"),
            QLocalizationCatalog.QLocalizationTextRead("Portrait.Tag"));
    }

    internal LPortraitLegend PWindowLegendRead(string realm)
    {
        Dictionary<LReferenceKind, string> kinds = [];
        foreach (LReferenceKind kind in System.Enum.GetValues<LReferenceKind>())
        {
            kinds[kind] = QLocalizationCatalog.QLocalizationTextRead(LReference.LReferenceKindResolve(kind));
        }

        return new LPortraitLegend(
            QLocalizationCatalog.QLocalizationTextRead("Display.Unknown"),
            QLocalizationCatalog.QLocalizationTextRead(realm == "Example" ? "Example.Unwritten" : realm + ".Untitled"),
            QLocalizationCatalog.QLocalizationTextRead("Example.Unwritten"),
            QLocalizationCatalog.QLocalizationTextRead(realm + ".UsageNone"),
            QLocalizationCatalog.QLocalizationTextRead(realm + ".UsageOne"),
            QLocalizationCatalog.QLocalizationTextRead(realm + ".UsageMany"),
            QLocalizationCatalog.QLocalizationTextRead("Example.Translation"),
            QLocalizationCatalog.QLocalizationTextRead("Reference.Title"),
            QLocalizationCatalog.QLocalizationTextRead("Source.Author"),
            QLocalizationCatalog.QLocalizationTextRead("Source.Year"),
            QLocalizationCatalog.QLocalizationTextRead("Source.Url"),
            QLocalizationCatalog.QLocalizationTextRead("Source.Note"),
            QLocalizationCatalog.QLocalizationTextRead("Situation.Description"),
            kinds);
    }
}
