# Shipment

This folder holds the records of the deck export: the depot port, the deck sheet, the manifest and the receipt.
A deck is studied on a phone flashcard app, and Llyn writes the deck file that app reads.
Every record is positional and immutable, so a change replaces the record whole.
Every rule here reads only its arguments.
Times arrive as stored strings, so nothing here reads the clock.

## `LDeckReceipt`

The tally of one deck export, shown to the user when it ends.
It counts cards written, media copied and media removed.
A failed file does not stop the export, so failures are listed rather than raised.
It is apart from `LReceipt`, which counts Joplin notes.

## `LDepot`

The outward port for the folder the phone app syncs.
Each deck is one file named by its code, with its media in a sibling folder.
The port names a deck by folder and code, and knows nothing of file formats.
The save is asynchronous, since its adapter writes through the workspace commit.
The digest answers null when the deck has no file there.
The digest lets a workflow tell a file Llyn wrote from one the phone wrote.
A media name is a plain file name inside the deck's media folder.
The media resolve names a source file by its content, so Application needs no file system.

## `LDeckSheet`

One deck file as cells, before any quoting.
The settings come first as key and value pairs.
The header names the columns, and each row holds one card's cells in header order.
Quoting, breaks and column order belong to the file adapter, so the sheet holds plain text.

## `LDeckManifest`

What Llyn last wrote for each deck, keyed by deck id.
Each deck keeps the file digest, the media names and the time written.
Only names listed here may be deleted from a media folder.
A deck id is stable, while a code can change, so the id is the key.

## `LDeckManifestVault`

The store for the deck manifest.
A read answers an empty manifest when nothing was saved.
A save replaces the manifest whole, so the manifest has one writer.

## Stamp

Commit: `2f9a06b8eb95`
- `LDeckManifest.cs` `bb1a6b2adf77f68a`
- `LDeckManifestVault.cs` `9e92ead069bcee95`
- `LDeckReceipt.cs` `ce0f0d44640eb380`
- `LDeckSheet.cs` `662227a4ac0c3f1b`
- `LDepot.cs` `4df7ef7b4f8b9eef`
