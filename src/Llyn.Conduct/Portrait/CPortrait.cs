using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CPortrait
{
    internal static LPortraitMedium CPortraitMediumRead(CPortraitMedium medium)
    {
        return (LPortraitMedium)medium;
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
            (LPressSide)ticket.CPressTicketSide,
            (LPressInk)ticket.CPressTicketInk);
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
