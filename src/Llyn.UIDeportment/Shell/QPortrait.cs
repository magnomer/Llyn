using System;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal static class QPortrait
{
    internal static LPortraitMedium QPortraitMediumRead(CPortraitMedium medium)
    {
        return (LPortraitMedium)medium;
    }

    internal static LPressTicket QPortraitTicketRead(CPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        return new LPressTicket(
            ticket.CPressTicketPrinter,
            QPortraitPaperRead(ticket.CPressTicketWidth, ticket.CPressTicketHeight),
            ticket.CPressTicketLandscape,
            ticket.CPressTicketCopies,
            ticket.CPressTicketCollated,
            (LPressSide)ticket.CPressTicketSide,
            (LPressInk)ticket.CPressTicketInk);
    }

    private static LPressPaper QPortraitPaperRead(double? width, double? height)
    {
        return width is double across && height is double down
            ? new LPressPaper(across, down)
            : LPressPaper.LPressPaperLocal;
    }

    internal static LPortraitLabel QPortraitLabelRead(CPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(label);

        return new LPortraitLabel(
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
            label.CPortraitLabelTag);
    }
}
