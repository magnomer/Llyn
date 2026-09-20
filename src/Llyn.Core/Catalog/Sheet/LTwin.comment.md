# LTwin.cs

## `public static class LTwin`

The numbering that tells apart catalog rows sharing one displayed name.
Two entries may carry the same headword, and two cards the same title.
A reader then cannot say which row is which.
The numbering is display text only, so stored names stay untouched.
It lives in Core so the engine can number the rows it returns for a view.
Panels that still build their own rows call it the same way.

## `public static void LTwinNameApply<LTwinRow>(IReadOnlyList<LTwinRow> rows, Func<LTwinRow, string> read, Action<LTwinRow, string> write)`

Writes each row's display name, appending `(1)`, `(2)` and so on to names more than one row carries.
A name held by a single row is written unchanged.
Numbers follow the order the rows are listed in, so the first row shown takes `(1)`.

## `public static void LTwinNameApply<LTwinRow>(IReadOnlyList<LTwinRow> rows, Func<LTwinRow, string> read, Action<LTwinRow, string> write, Func<LTwinRow, long> rank)`

Numbers twins by ascending rank instead of list position, so a row keeps its number under any sort or filter.
Entry lists pass the entry id, which makes the older entry `(1)` in every view.
Rows tied on rank fall back to list order.
Names are compared byte for byte, so case and spacing separate two names.
