# LOutpostNote.cs
Hash: `54e940cc97fb43e6`

## `public sealed record LOutpostNote(string LOutpostNoteId, string LOutpostNoteFolder, string LOutpostNoteTitle, string LOutpostNoteBody);`

One Joplin note as Llyn pushes it.
Ids are 32 lowercase hex characters, the form Joplin accepts for a caller-chosen id.
A stable id lets a later push overwrite the same note instead of adding a copy.

**Parameters**

- `LOutpostNoteId` — The note's fixed Joplin id.
- `LOutpostNoteFolder` — The fixed id of the notebook that holds the note.
- `LOutpostNoteTitle` — The title Joplin lists the note under.
- `LOutpostNoteBody` — The note's content as Markdown.
