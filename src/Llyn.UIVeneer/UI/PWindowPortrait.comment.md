# PWindowPortrait.cs

## `public partial class PWindow`

The window's part in exporting an entry: the file, the format and the words a document is written with.
Every panel that shows an entry exports through here, so all offer the same formats and share one failure path.
It renders nothing, opens no writer, and knows no file format.

## `private static readonly IReadOnlyList<(string Key, string Suffix, LPortraitFormat Kind)> PWindowPortraitKinds`

One row per offered format, keeping the dialog filter and the chosen format in step.
The dialog returns a one-based index into this same list, so the two cannot drift apart.

## `internal async Task PWindowPortraitExport(string file, Func<string, LPortraitFormat, LPortraitLabel, Task> export)`

Asks the reader for a file and a format, then hands both to the panel's export with the window's labels.
The panel names the default file after its chosen record and runs the export through its own deportment.
A cancelled dialog is not a failure and leaves nothing behind.
The filter index chooses the format, so the reader picks it where they pick the name.
The whole attempt stands together, because a half-written document is of no use.

### `FileName = LVista.LVistaFileRead(vista),`

The headword is offered as the file name, since that is what the reader would type.
The vista cleans it below the shell, so the panel names no file rule of its own.
