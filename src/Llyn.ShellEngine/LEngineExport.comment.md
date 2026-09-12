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

Markup is refused rather than written.
The workspace holds no markup writer, and a refusal says so where an empty file would not.
The shell still offers the format, so the seam a writer attaches to stays visible.
