namespace Llyn.Core;

public sealed record LPressTicket(
    string LPressTicketPrinter,
    LPressPaper LPressTicketPaper,
    bool LPressTicketLandscape,
    int LPressTicketCopies,
    bool LPressTicketCollated,
    LPressSide LPressTicketSide,
    LPressInk LPressTicketInk);
