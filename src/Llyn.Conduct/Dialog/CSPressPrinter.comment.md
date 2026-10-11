# CSPressPrinter.cs
Hash: `44d28bded7f044a8`

## `public sealed record CSPressPrinter(string? CSPressPrinterName, string CSPressPrinterText, bool CSPressPrinterChosen)`

One destination in the print window's printer list, ready to show.
The list opens with Save as PDF, then the printers in the spooler's order.

**Parameters**

- `CSPressPrinterName`: the queue name handed back to `CSPressPrinterSelect`, or null for Save as PDF.
- `CSPressPrinterText`: the display text, a wording key or the escaped queue name.
  The window formats it with `CSPressPrinterName`, so either shows as one value.
- `CSPressPrinterChosen`: whether this destination is the one the setting names.
