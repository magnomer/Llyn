# LEngineExport.cs

## `public sealed partial class LEngine`

The one entry point every export goes through.
The shell chooses a path and a format, and the engine does the rest.
No writer is reachable from the interface layer.

## `public void LEnginePressApply(LPress press)`

The shell hands in the platform's printing surface at startup.
Without it every format still works except PDF.

## `public async Task LEnginePortraitExport(long entryId, string path, LPortraitFormat format, LPortraitLabel label)`

The formats are written from the portrait, so they show exactly what the panel shows.
PDF is the rendered page printed, which is why it needs no layout of its own.
An unconfigured press is an error rather than a silent empty file.

Markup does not pass through the portrait.
It hands the one entry id to `LEngineMarkupExport`, which reads stored rows and writes the file.
The label is not needed there, because markup carries states and never a display word for them.

## `public async Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)`

Prints one entry on the printer the ticket names.
The page is the same rendered sheet the PDF export prints, so paper and file agree.
The shell only chose the printer: what is on the page was never read from the screen.

## `public async Task LEnginePortraitPrint(long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)`

Prints one example, source or situation, whichever `owner` names.
The page likeness is assembled from stored rows exactly as the entry portrait is.

## `private LPress LEnginePressRead()`

The press the shell handed in, or an error saying none did.
Both printing and the PDF export refuse the same way rather than writing nothing.
