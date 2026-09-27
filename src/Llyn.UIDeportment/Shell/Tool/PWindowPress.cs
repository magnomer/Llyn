using System;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Controls;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal async Task PWindowPressRun(Func<CPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        try
        {
            if (PWindowTicketRead() is CPressTicket ticket)
            {
                await print(ticket);
            }
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Print.Failed", exception);
        }
    }

    internal Task PWindowPressRun(Func<CPortraitLabel, CPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        return PWindowPressRun((CPressTicket ticket) => print(PWindowPortraitRead(), ticket));
    }

    internal Task PWindowPressRun(string realm, Func<CPortraitLabel, CPortraitLegend, CPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        return PWindowPressRun((CPortraitLabel label, CPressTicket ticket) =>
            print(label, PWindowLegendCreate(realm), ticket));
    }

    internal Task PWindowPressRun(string realm, Func<CPortraitLegend, CPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(print);

        return PWindowPressRun((CPressTicket ticket) => print(PWindowLegendCreate(realm), ticket));
    }

    internal Task PWindowPressRun(Func<LPortraitLabel, LPressTicket, Task> print)
    {
        return PWindowPressRun(QPortrait.QPortraitPressCreate(print));
    }

    internal CPressTicket? PWindowTicketRead()
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

        return new CPressTicket(
            dialog.PrintQueue.FullName,
            PWindowPaperRead(chosen.PageMediaSize?.Width),
            PWindowPaperRead(chosen.PageMediaSize?.Height),
            chosen.PageOrientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape,
            chosen.CopyCount ?? 1,
            chosen.Collation != Collation.Uncollated,
            chosen.Duplexing switch
            {
                Duplexing.OneSided => CPressSide.CPressSideSingle,
                Duplexing.TwoSidedLongEdge => CPressSide.CPressSideLong,
                Duplexing.TwoSidedShortEdge => CPressSide.CPressSideShort,
                _ => CPressSide.CPressSideDefault,
            },
            chosen.OutputColor switch
            {
                OutputColor.Color => CPressInk.CPressInkColor,
                OutputColor.Grayscale or OutputColor.Monochrome => CPressInk.CPressInkGray,
                _ => CPressInk.CPressInkDefault,
            });
    }

    private static double? PWindowPaperRead(double? pixels)
    {
        return pixels > 0 ? pixels / 96.0 : null;
    }

    internal CPortraitLabel PWindowPortraitRead()
    {
        return new CPortraitLabel(
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

    internal CPortraitLegend PWindowLegendCreate(string realm)
    {
        Dictionary<string, string> kinds = [];
        foreach (LReferenceKind kind in System.Enum.GetValues<LReferenceKind>())
        {
            string key = LReference.LReferenceKindResolve(kind);
            kinds[key] = QLocalizationCatalog.QLocalizationTextRead(key);
        }

        return new CPortraitLegend(
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
