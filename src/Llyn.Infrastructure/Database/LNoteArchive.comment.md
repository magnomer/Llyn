# LNoteArchive.cs

## `public sealed class LNoteArchive`

Persists the single Note an entry owns.
The note table is keyed by `entry_id`, so at most one Note exists per entry.
That is a schema fact.
Saving a Note for an entry that already has one replaces it rather than adding a second.
Deleting the entry removes its Note through the foreign-key cascade.

## `public LNoteArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public void LNoteSave(LNote note)`

Writes `note` as the entry's Note, replacing any Note the entry already had.
The text is stored exactly as given — see the format TODO on `LNote`.

## `public LNote? LNoteRead(long entryId)`

Reads the entry's Note, or `null` when it has none.

## `public void LNoteDelete(long entryId)`

Removes the entry's Note, if it has one.
The entry itself is untouched.
