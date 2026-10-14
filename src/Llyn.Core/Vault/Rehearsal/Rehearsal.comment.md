# Rehearsal

This folder holds the persistence ports for flashcard study.
Rehearsal rows point at lexical rows, and no lexical port reads them back.
Each port lists what the deck workflows ask of storage, and nothing about how rows are kept.
Every adapter is a `{Base}Archive` in Infrastructure over the workspace database.
A test may answer from memory, so the deck workflows are proved without a database.

## `LDeckVault`

The persistence port for decks and their bearings.
A create appends the deck to its folder and answers it with its stored id and timestamps.
The single read answers null when no deck has the id.
An update writes every setting, the folder and the position.
The grain and the stencil stay as created.
A deck's grain fixes what its cues bind, and its stencil is its own for life.
The order set renumbers one folder's decks in the given order.
A delete removes the deck and then the stencil it owns, in one session.
A saved stencil is never owned, so a delete keeps it.
A bearing save replaces the deck's bearings whole.

## `LCueVault`

The persistence port for a deck's cues and a free cue's cells.
A create appends the cues to the deck in the given order.
It answers them with their stored ids, positions and added time.
An update writes the flag, the exclusion and both texts.
The grain and the references stay as created.
A cue's grain follows its deck, and only a deleted lexical row clears a reference.
The order set renumbers one deck's cues in the given order.
A delete removes the cues and renumbers each deck they left.
A cell save replaces the cue's cells whole.

### `LCueHolderRead`

Answers the entry that holds the cue's bound row, or null when none does.
The schema lets only an entry cue store an entry id.
So a meaning, collocation, example or pair cue reaches its entry through the row it binds.
The answer is read when asked, so it follows the rows as they are now.
The port has no entry read yet, since its only reader comes with a later phase.

## `LDeckFolderVault`

The persistence port for the folders that group decks.
It is its own port because folders are a separate role from the decks they hold.
A folder create appends the folder after the last one.
An update writes the name and the position.
A folder delete leaves the deck re-placing to the workflow, which runs first.

## `LStencilVault`

The persistence port for stencils and their slots.
The list read answers saved stencils only, since an owned stencil shows through its deck.
The single read answers either kind, or null when no stencil has the id.
An update writes the name and replaces the slots whole.
The saved mark stays as created, since saving as reusable copies the row instead.
The port has no delete, since no workflow reads one.
An owned stencil goes with its deck, inside the deck delete.

## Stamp

Commit: `aae5349dbb94`
- `LCueVault.cs` `6f985cf5ef03da1c`
- `LDeckFolderVault.cs` `0efe13113613f037`
- `LDeckVault.cs` `8e470e7af6c57ef0`
- `LStencilVault.cs` `0cd9fbc81cb03923`
