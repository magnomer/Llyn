# LNoteVault.cs

## `public interface LNoteVault`

The persistence port for the Note rows the engine reads and writes.
It lists exactly what the engine asks of note storage, and nothing about how rows are kept.
`LNoteArchive` in Infrastructure is its adapter over the workspace database.

## `void LNoteSave(LNote note);`

Writes `note` as the entry's Note, replacing any Note the entry already had.
The text is stored exactly as given, and the engine hands it over as normalized Markdown.

## `LNote? LNoteRead(long entryId);`

Reads the entry's Note, or `null` when it has none.

## `void LNoteDelete(long entryId);`

Removes the entry's Note, if it has one.
The entry itself is untouched.
