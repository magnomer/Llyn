# CPressTicket.cs

## `public sealed record CPressTicket(`

What the print dialog answered, as a driver's envoy answers the gate's question.
The panel controller turns it into the engine's ticket.

**Parameters**

- `CPressTicketPrinter`: the chosen printer's full name.
- `CPressTicketWidth`: the sheet width as the dialog measured it, in device-independent pixels, or none.
- `CPressTicketHeight`: the sheet height as the dialog measured it, in device-independent pixels, or none.
- `CPressTicketLandscape`: whether the sheet is turned on its side.
- `CPressTicketCopies`: how many copies to print.
- `CPressTicketCollated`: whether copies are printed as whole sets.
- `CPressTicketSide`: which sides of the sheet a print lands on.
- `CPressTicketInk`: whether the print is in color or in gray.
