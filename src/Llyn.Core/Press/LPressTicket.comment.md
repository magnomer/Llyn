# LPressTicket.cs

## `public sealed record LPressTicket`

What the reader chose in the printer dialog, handed to the press with the page.
Choosing a printer is the shell's business, while what is printed is the engine's.
Every choice the dialog offers travels here, so nothing the reader picked is dropped on the way.

**Parameters**

- `LPressTicketPrinter` - the name of the chosen printer.
- `LPressTicketPaper` - the sheet size the printer was asked for.
- `LPressTicketLandscape` - whether the paper is turned.
- `LPressTicketCopies` - how many copies are printed.
- `LPressTicketCollated` - whether copies come out as whole sets rather than page by page.
- `LPressTicketSide` - which sides of the sheet are printed on.
- `LPressTicketInk` - whether the page is printed in color or in gray.
