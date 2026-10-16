# Rehearsal

This folder holds the SQLite adapters of the flashcard study ports.
Each opens one session per call, so a call made inside an open session joins its transaction.
Table and column names are persisted keys and stay lowercase, independent of code member names.

## `LCueArchive`

The adapter of `LCueVault` over the `cue` and `cue_cell` tables.
The read of every cue is one select, ordered by deck and position.
A create takes the positions after the deck's last cue and stamps the added time.
An update of a missing cue throws.
A delete notes the deck of each removed cue, then renumbers those decks.
So the next create's position never collides on the unique place index.
Cells go with their cue by cascade.
Cells are deleted and reinserted on save, which is safe because nothing references a cell.
Cells read back by column, then by index.

### `LCueHolderRead`

One query takes the first holder the cue's references name.
An entry reference answers itself while the entry exists.
A sense or a collocation answers its `entry_parent`.
An example answers the lowest entry holding a sense or collocation that shows it.

## `LDeckArchive`

The adapter of `LDeckVault` over the `deck` and `bearing` tables.
A create takes the next position in its folder and stamps both timestamps with the current time.
An update stamps the updated time and leaves the grain, the stencil and the added time alone.
An update of a missing deck throws.
The delete reads the deck's stencil, deletes the deck row, then deletes that stencil if unsaved.
The order is forced by the deck's stencil reference, which carries no delete clause.
The deck's cues and bearings go with it by cascade.
The delete then renumbers the decks left in the folder.
So the next create's position never collides on the unique place index.
The full read orders top-level decks first, then each folder's decks by position.

## `LDeckFolderArchive`

The adapter of `LDeckFolderVault` over the `deck_folder` table.
A folder create takes one past the highest folder position.
Folder positions carry no unique index, so a gap after a delete is harmless.
An update of a missing folder throws.
A delete sets the folder's decks to the top level by the foreign key.

## `LStencilArchive`

The adapter of `LStencilVault` over the `stencil` and `stencil_slot` tables.
Slots are deleted and reinserted on update, which is safe because nothing references a slot.
Slots read back by column, then by index.

## Stamp

Commit: `121691f60459`
- `LCueArchive.cs` `62ac67a663846646`
- `LDeckArchive.cs` `407dc9353301b5ab`
- `LDeckFolderArchive.cs` `73021d288ce30e57`
- `LStencilArchive.cs` `1ccca2346503cc5c`
