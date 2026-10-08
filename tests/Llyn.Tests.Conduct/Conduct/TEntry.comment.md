# TEntry.cs
Hash: `7ba0fd27b577acb9`

## `public sealed class TEntry`

Covers the editor's head field gates end to end on a real workspace.
Field gates write the headword, the trimmed note and the reading, and an empty language changes nothing.
The reads answer empty on an empty desk, and an added etymon reads back by headword.
It builds its editors through `TEditor`, so both files open the same stored entry.
