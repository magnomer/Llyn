# TShelf.cs

## `public sealed class TShelf`

Covers the sources panel's gates end to end on a real workspace.
A chosen Source shows its colophon, enables the bin and reads its worded tally.
A fresh request with a Source chosen opens an entry that cites it from its first paint.
A fresh request with nothing chosen opens a Source draft in the source editor.
Leaving a fresh entry's edit mode returns to the chosen Source in its read area.
The bin ignores the entry side and deletes the chosen Source only from the Source side.
Every offered order keeps every Source listed, a null order keeps the vista's own, and name order sorts.
An unmatched query empties the shelf and closes the shown Source.
A hidden language marks the shelf filtered, and an empty filter clears the mark.
A chosen Source records the voyage station, and a kept leave records nothing and stays on the held draft.
A stored leave saves the held draft and shows the stored Source.
A leave with nothing unsaved asks nothing.
The rail's undo, redo and save act on the Source draft while the Source side edits.
A finish without storing drops the draft of the side in front, the Source draft or the entry draft.
A kept leave on an entry click stays on the held Source draft.
Toggling the Source side into edit mode opens its draft, and toggling it off drops the draft.
An entry notice with the entry side closed shows the chosen Source again.
A close clears both sides, and a print with nothing shown prints nothing.
An export writes only an entry on display.

## `internal static CShelf TShelfPrepare(CAtelier atelier, CEnvoy envoy)`

Builds the shelf over the atelier with its own editor, and the shelf restores both vistas itself.
Its seam answers that the tab is in front, and its marshal runs each notice at once.
`TShelfImprint` shares it.

## `private static LEntry TShelfEntrySave(LEngine engine, string headword)`

Stores one English entry with a single meaning.
