# PTwin.cs

## `internal static class PTwin`

The numbering that tells apart catalog rows sharing one displayed name.
Two entries may carry the same headword, and two cards the same title.
A reader then cannot say which row is which.
The numbering is display text only, so stored names stay untouched.

## `internal static void PTwinNameApply<TRow>(IReadOnlyList<TRow> rows, Func<TRow, string> read, Action<TRow, string> write)`

Writes each row's display name, appending `(1)`, `(2)` and so on to names more than one row carries.
A name held by a single row is written unchanged.
Numbers follow the order the rows are listed in, so the first row shown takes `(1)`.
Names are compared byte for byte, so case and spacing separate two names.
