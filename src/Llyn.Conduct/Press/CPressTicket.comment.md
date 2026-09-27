# CPressTicket.cs

## `public sealed record CPressTicket(`

What the print dialog answered, as a driver hands it to a panel.
The panel controller turns it into the engine's ticket.

**Parameters**

- `CPressTicketPrinter`: the chosen printer's full name.
- `CPressTicketWidth`: the sheet width in inches, or none when the dialog named no usable size.
- `CPressTicketHeight`: the sheet height in inches, or none when the dialog named no usable size.
- `CPressTicketLandscape`: whether the sheet is turned on its side.
- `CPressTicketCopies`: how many copies to print.
- `CPressTicketCollated`: whether copies are printed as whole sets.
- `CPressTicketSide`: which sides of the sheet a print lands on.
- `CPressTicketInk`: whether the print is in color or in gray.
