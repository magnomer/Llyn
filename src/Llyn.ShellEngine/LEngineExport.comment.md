# LEngineExport.cs

## `public sealed partial class LEngine`

The one entry point every export goes through.
The shell chooses a path and a format, and the engine does the rest.
No writer is reachable from the interface layer.

## `public void LEnginePressApply(LPress press)`

The shell hands in the platform's printing surface at startup.
Without it every format still works except PDF.

## `public async Task LEnginePortraitExport(string entryId, string path, LPortraitFormat format, LPortraitLabel label)`

Markup is written from the stored draft, because it must keep what the display never shows.
The other formats are written from the portrait, so they show exactly what the panel shows.
PDF is the rendered page printed, which is why it needs no layout of its own.
An unconfigured press is an error rather than a silent empty file.

## `private string LEngineMarkupFormat(string entryId)`

The sources are the ones the entry's own citations name, reached through the Examples its cards quote.
A source that cannot be read is skipped, since the reader would reject a citation naming nothing.

## `private static void LEngineSourceRead(IReadOnlyList<LCardDraft> cards, List<string> ids)`

Collects the reference rows the given cards cite, without repeats.
