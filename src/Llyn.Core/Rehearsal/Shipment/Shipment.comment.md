# Shipment

This folder holds the records of the deck export: the depot port, the shipment and the receipt.
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

The outward port for the folder the phone app syncs, one capability, exporting a deck.
Each deck is one file named by its code, with its media in a sibling folder.
The export answers the finished receipt.
The file format, the digest check, the media and the manifest stay behind it.
It is asynchronous, since its adapter writes through the workspace commit.
It refuses with `LRefusal.LRefusalPhone` when the deck file there is not the one Llyn last wrote.
A media fault goes to the fault sink and the receipt's failures, and the export goes on.

## `LDepotShipment`

One deck as Application hands it to the depot.
It holds the folder, the deck id, the code and the Basic style flag.
It holds the cue ids, their faces and the typed rows too.
The cue ids, the faces and the typed rows are parallel lists, one entry per cue.
A typed row holds the trimmed typed location of a media cell.
That is a cell that is a local file or resolved to nothing.
It holds null for any other cell.
A media cell arrives resolved.
A local file is its full path, a web address is its text, and an unresolved one is empty.
The depot copies a media cell only when it is a full path.
It names the typed text when the copy fails.

## Stamp

Commit: `1f3467bbe5c6`
- `LDeckReceipt.cs` `ce0f0d44640eb380`
- `LDepot.cs` `5e8bf506ffc127c3`
- `LDepotShipment.cs` `d85bbb6b1e910938`
