# TShelf.cs
Hash: `fa452ff3c4f6ef9b`

## `public sealed class TShelf`

Covers the sources panel's gates end to end on a real workspace.
A chosen Source shows its colophon, enables the bin and reads its worded tally.
A fresh request with a Source chosen opens an entry that cites it from its first paint.
A fresh request with nothing chosen opens a Source draft in the source editor.
Leaving a fresh entry's edit mode returns to the chosen Source in its read area.
The bin ignores the entry side and deletes the chosen Source only from the Source side.
Every offered order keeps every Source listed, a null order keeps the order already set, and name order sorts.
An unmatched query empties the shelf and closes the shown Source.
A hidden language marks the shelf filtered, and an empty filter clears the mark.
Toggling the Source side into edit mode opens its draft, and toggling it off drops the draft.
A print with nothing shown prints nothing.
The leave and draft gates live in `TShelfDraft`, and the notices live in `TShelfVista`.
An export writes only an entry on display.

## `internal static CShelf TShelfPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the shelf over the atelier with its own editor, and the shelf restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each notice at once.
`TShelfDraft`, `TShelfImprint`, `TShelfRoll`, `TShelfVista` and `TFaultPortrait` share it.

## `internal static LEntry TShelfEntrySave(LEngine engine, string headword)`

Stores one English entry with a single meaning.
`TShelfDraft` and `TFaultPortrait` share it.
