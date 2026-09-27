namespace Llyn.Conduct;

public sealed record CPressTicket(
    string CPressTicketPrinter,
    double? CPressTicketWidth,
    double? CPressTicketHeight,
    bool CPressTicketLandscape,
    int CPressTicketCopies,
    bool CPressTicketCollated,
    CPressSide CPressTicketSide,
    CPressInk CPressTicketInk);
