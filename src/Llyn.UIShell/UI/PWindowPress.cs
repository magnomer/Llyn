using System;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

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
            PLocalizationTextRead("Display.Unknown"),
            PLocalizationTextRead("Display.MeaningSingle"),
            PLocalizationTextRead("Display.MeaningPlural"),
            PLocalizationTextRead("Display.CollocationSingle"),
            PLocalizationTextRead("Display.Collocation"),
            PLocalizationTextRead("Display.Translated"),
            PLocalizationTextRead("Display.Note"),
            PLocalizationTextRead("Portrait.Form"),
            PLocalizationTextRead("Portrait.Paradigm"),
            PLocalizationTextRead("Frequency.Title"),
            PLocalizationTextRead("Portrait.Glyph"),
            PLocalizationTextRead("Display.Script"),
            PLocalizationTextRead("Display.Fanqie"),
            PLocalizationTextRead("Portrait.Example"),
            PLocalizationTextRead("Portrait.Gloss"),
            PLocalizationTextRead("Reference.Title"),
            PLocalizationTextRead("Portrait.Mention"),
            PLocalizationTextRead("Portrait.Situation"),
            PLocalizationTextRead("Portrait.Register"),
            PLocalizationTextRead("Portrait.Translation"),
            PLocalizationTextRead("Portrait.Tag"));
    }

    internal LPortraitLegend PWindowLegendRead(string realm)
    {
        Dictionary<LReferenceKind, string> kinds = [];
        foreach (LReferenceKind kind in System.Enum.GetValues<LReferenceKind>())
        {
            kinds[kind] = PLocalizationTextRead(PReference.PReferenceKindRead(kind));
        }

        return new LPortraitLegend(
            PLocalizationTextRead("Display.Unknown"),
            PLocalizationTextRead(realm == "Example" ? "Example.Unwritten" : realm + ".Untitled"),
            PLocalizationTextRead("Example.Unwritten"),
            PLocalizationTextRead(realm + ".UsageNone"),
            PLocalizationTextRead(realm + ".UsageOne"),
            PLocalizationTextRead(realm + ".UsageMany"),
            PLocalizationTextRead("Example.Translation"),
            PLocalizationTextRead("Reference.Title"),
            PLocalizationTextRead("Source.Author"),
            PLocalizationTextRead("Source.Year"),
            PLocalizationTextRead("Source.Url"),
            PLocalizationTextRead("Source.Note"),
            PLocalizationTextRead("Situation.Description"),
            kinds);
    }
}
