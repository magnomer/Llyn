# Rehearsal

This folder holds the file adapters for flashcard decks.
They write the deck files the phone app reads and the manifest of what Llyn wrote.
Every format rule of the phone's text file lives here, not in Core.

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

### `LDepotCellFormat`

In TSV style a cell holding a quote, TAB, CR or LF is quoted, with inner quotes doubled.
The rule for a quote without a break is an inference until the phone probe.
In Basic style nothing is quoted, and each line break becomes `|`.
A TAB becomes a space, since Basic has no quoting to keep it inside a cell.

### `LDepotRead`

The read detects a byte order mark, so a phone file in UTF-8 or UTF-16 reads alike.
It undoes the TSV quoting and skips blank lines.
Leading `*` lines are settings, and the first other line is the header.

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
It keeps its data under the key `rehearsal`, apart from the Joplin manifest.
Decks are keyed by their id, and media names are written sorted, so a rewrite is stable.
Unreadable text answers an empty manifest, as the Joplin manifest does.
A file fault becomes an `LVaultFault`.

## Stamp

Commit: `aae5349dbb94`
- `LDeckManifestFile.cs` `50773590e8e51a53`
- `LDepotFolder.cs` `a9ae910fc9888561`
