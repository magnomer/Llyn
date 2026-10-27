# Rehearsal

This folder is the locker module: the export of flashcard decks to the phone folder.
It writes the deck files the phone app reads and keeps the manifest of what Llyn wrote.
Every format rule of the phone's text file lives here, not in Core or Application.
Only `LLockerEntrance` is public.
The adapters, the sheet and the manifest records are internal.

## `LLockerEntrance`

The module's entrance, and the rig's `LDepot`.
It implements the port explicitly, so its own public surface is the constructor.
One export reads the manifest, checks the digest, copies media, saves the file, prunes media and saves the manifest.
The manifest read and the manifest save hold the entrance's lock.
File work runs outside it.
The clock stamps the time written, so a test can fix it.

### `LDepotExport`

A deck file whose digest differs from the manifest's is a phone export.
The run refuses it with `Refusal.Phone`, because Llyn never overwrites a file it has not read.
A file present with no manifest entry is refused the same way.
A shipment whose cues, faces and typed rows differ in count throws, since no row could pair them.

### Media

Only a picture or sound cell holding a fully qualified path is copied.
Application resolves every location first, so any other value is written as it stands.
A path is copied under the name `LDepotMediaResolve` gives its content.
The same content is copied once, and an existing name is never rewritten.
A missing source or a failed copy is named in the receipt's failures, and the cell is left empty.

The media are copied before the deck file is saved.
So a saved file never names a media file that is missing.

### Ownership

The manifest lists only the names this deck copied.
A name that was already in the folder is used but never claimed.
After the save, an owned name the deck no longer uses is deleted.
So a file the phone or the user placed there is never deleted.

The manifest is saved in a `finally`, so a failed run still records what it copied.
A failed save keeps the digest Llyn last wrote.
A failed delete keeps its name, so the next run tries again.

## `LDeckSheet`

One deck file as cells, before any quoting.
The settings come first as key and value pairs.
The header names the columns, and each row holds one card's cells in header order.

## `LDeckManifest`

What Llyn last wrote for each deck, keyed by deck id.
Each deck keeps the file digest, the media names and the time written.
Only names listed here may be deleted from a media folder.
A deck id is stable, while a code can change, so the id is the key.

## `LDepotFolder`

The phone folder adapter.
A deck file is `<code>.txt` directly in the folder.
Its media sit in `<code> Media/` beside it.
A code or media name that is not one plain file name answers null on read and throws on write.
So the adapter never reads `Backups/` or `_Sync/`, where the phone keeps copies.

### `LDepotSave`

The save writes UTF-8 without a byte order mark, through `LWorkspaceRoot.LWorkspaceFileSave`.
A reader never sees half a file.
Setting lines come first as three cells, `*`, key and value.
The header follows, then one line per row, each ending in LF.
The `Text 1` column is moved first in the header and every row.
The phone app reads the first column as `Text 1`, so `ID` never leads.
A header without `Text 1` throws, since the phone could not read the file.

### `LDepotHeaderResolve`

The header holds `Text 1`, every column some cue fills, and `ID` last.
Columns follow side order, then notes, extra info and categories.

### `LDepotRowFormat`

A row holds each cell under its column's name, the cue id under `ID`, and empty text elsewhere.
The first cell of a column wins when two cells name it.

### `LDepotCellFormat`

In TSV style a cell holding a quote, TAB, CR or LF is quoted, with inner quotes doubled.
A cell without any of these is written as it stands.
In Basic style nothing is quoted, and each line break becomes `|`.
A TAB becomes a space, since Basic has no quoting to keep it inside a cell.

### `LDepotDigestRead`

The digest is the lowercase SHA-256 of the file bytes.
It compares a file with the one Llyn wrote, so any change to the bytes shows.

### `LDepotMediaResolve`

A media name is the first 16 hex digits of the source's SHA-256, then its lowercase extension.
The name comes from content, so the same picture keeps one name across rebuilds.
A missing source answers null.

### `LDepotMediaCopy`

A copy goes through a pending file and the workspace commit, as the deck file does.

## `LDeckManifestFile`

The deck manifest store, copying `LManifestFile`.
A read answers an empty manifest when nothing was saved, and a save replaces it whole.
It keeps its data under the key `rehearsal`, apart from the Joplin manifest.
Decks are keyed by their id, and media names are written sorted, so a rewrite is stable.
Unreadable text answers an empty manifest, as the Joplin manifest does.
A file fault becomes an `LVaultFault`.

## Stamp

Commit: `1f3467bbe5c6`
- `LDeckManifest.cs` `38a65f49e35c6959`
- `LDeckManifestFile.cs` `c9f7da0f0061fb1f`
- `LDeckSheet.cs` `fd073dc1a559d79a`
- `LDepotFolder.cs` `5dcc6df0c0bd8588`
- `LLockerEntrance.cs` `444da7a71eca5b8b`
