# TShelfDraft.cs
Hash: `3e3bbea14056b3fd`

## `public sealed class TShelfDraft`

Covers how the sources panel leaves, saves and finishes a held draft on a real workspace.
It builds each shelf through `TShelf.TShelfPrepare` and stores entries through `TShelf.TShelfEntrySave`.
A kept leave on a Source click records no voyage station and stays on the held draft.
A stored leave saves the held draft and shows the stored Source.
A leave with nothing unsaved asks nothing.
The rail's undo, redo and save act on the Source draft while the Source side edits.
A finish without storing drops the draft of the side in front, the Source draft or the entry draft.
A kept leave on an entry click stays on the held Source draft.
Only the save test sets the edit delay to zero.
