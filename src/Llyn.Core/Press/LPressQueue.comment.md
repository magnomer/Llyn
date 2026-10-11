# LPressQueue.cs
Hash: `88309db3989d2040`

## `public interface LPressQueue`

The printer list a print window offers beside Save as PDF.
Listing printers asks the operating system's spooler, which is a platform concern.
It is its own port, apart from `LPress`, because listing printers needs no browser.
The host supplies the implementation at startup, as it does for any other platform port.

## `Task<IReadOnlyList<LPressPrinter>> LPressQueueRead()`

Lists the installed printers with what each reported it can do.
A printer that reports nothing is still listed, marked unknown.
With no printer installed the list is empty, and saving as PDF remains.
A spooler that cannot be read raises, so the caller can tell the user why no printer shows.
