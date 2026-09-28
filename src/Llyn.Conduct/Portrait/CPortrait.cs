using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CPortrait
{
    internal static LPortraitMedium CPortraitMediumRead(CPortraitMedium medium)
    {
        return medium switch
        {
            CPortraitMedium.CPortraitMediumMarkup => LPortraitMedium.LPortraitMediumMarkup,
            CPortraitMedium.CPortraitMediumHtml => LPortraitMedium.LPortraitMediumHtml,
            CPortraitMedium.CPortraitMediumMarkdown => LPortraitMedium.LPortraitMediumMarkdown,
            CPortraitMedium.CPortraitMediumDocx => LPortraitMedium.LPortraitMediumDocx,
            CPortraitMedium.CPortraitMediumPdf => LPortraitMedium.LPortraitMediumPdf,
            _ => throw new ArgumentOutOfRangeException(nameof(medium), medium, null),
        };
    }

    internal static LPressTicket CPortraitTicketRead(CPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        return LPortraitPort.LEngineTicketRead(
            ticket.CPressTicketPrinter,
            ticket.CPressTicketWidth,
            ticket.CPressTicketHeight,
            ticket.CPressTicketLandscape,
            ticket.CPressTicketCopies,
            ticket.CPressTicketCollated,
            LPortraitSideRead(ticket.CPressTicketSide),
            LPortraitInkRead(ticket.CPressTicketInk));
    }

    private static LPressSide LPortraitSideRead(CPressSide side)
    {
        return side switch
        {
            CPressSide.CPressSideDefault => LPressSide.LPressSideDefault,
            CPressSide.CPressSideSingle => LPressSide.LPressSideSingle,
            CPressSide.CPressSideLong => LPressSide.LPressSideLong,
            CPressSide.CPressSideShort => LPressSide.LPressSideShort,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
        };
    }

    private static LPressInk LPortraitInkRead(CPressInk ink)
    {
        return ink switch
        {
            CPressInk.CPressInkDefault => LPressInk.LPressInkDefault,
            CPressInk.CPressInkColor => LPressInk.LPressInkColor,
            CPressInk.CPressInkGray => LPressInk.LPressInkGray,
            _ => throw new ArgumentOutOfRangeException(nameof(ink), ink, null),
        };
    }

    internal static LPortraitLabel CPortraitLabelRead(CPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(label);

        return LPortraitPort.LEngineLabelRead(
        [
            label.CPortraitLabelUnknown,
            label.CPortraitLabelMeaning,
            label.CPortraitLabelMeanings,
            label.CPortraitLabelCollocation,
            label.CPortraitLabelCollocations,
            label.CPortraitLabelIncoming,
            label.CPortraitLabelNote,
            label.CPortraitLabelForm,
            label.CPortraitLabelParadigm,
            label.CPortraitLabelFrequency,
            label.CPortraitLabelGlyph,
            label.CPortraitLabelScript,
            label.CPortraitLabelFanqie,
            label.CPortraitLabelExample,
            label.CPortraitLabelGloss,
            label.CPortraitLabelSource,
            label.CPortraitLabelMention,
            label.CPortraitLabelEtymology,
            label.CPortraitLabelSituation,
            label.CPortraitLabelRegister,
            label.CPortraitLabelTranslation,
            label.CPortraitLabelTag,
        ]);
    }

    internal static LPortraitLegend CPortraitLegendRead(CPortraitLegend legend)
    {
        ArgumentNullException.ThrowIfNull(legend);

        return LPortraitPort.LEngineLegendRead(
            [
                legend.CPortraitLegendUnknown,
                legend.CPortraitLegendUntitled,
                legend.CPortraitLegendUnwritten,
                legend.CPortraitLegendUnused,
                legend.CPortraitLegendOnce,
                legend.CPortraitLegendUses,
                legend.CPortraitLegendTranslation,
                legend.CPortraitLegendSource,
                legend.CPortraitLegendAuthor,
                legend.CPortraitLegendYear,
                legend.CPortraitLegendUrl,
                legend.CPortraitLegendNote,
                legend.CPortraitLegendDescription,
            ],
            legend.CPortraitLegendKind);
    }
}
