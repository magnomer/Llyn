# Rehearsal

This folder creates the tables a Deck owns and the tables a Deck reads from.
Rehearsal is study, so these rows are no lexical object.
They point at lexical rows, and no lexical row points back at them.
Table and column names are persisted keys and stay lowercase, independent of code member names.

## `LSchemaDeck`

`LSchemaDeckCreate` creates the deck folders, the stencils, the decks, their bearings, the cues and the cue cells.
One command holds every CREATE, as `LSchemaAncestor` does.

A deck folder and a stencil reference nothing.
So a rebuild never sweeps them, and a deck never loses its folder or stencil there.

A stencil holds its slots by cascade.
The deck's `stencil_ref` carries no ON DELETE clause.
So a stencil a deck owns cannot be deleted under it.
A deck delete removes the deck row first and its owned stencil second, in one transaction.

A deck's `folder_ref` is set to none when its folder goes.
That never fires on a live delete of a folder holding decks.
The folder-delete workflow re-places those decks first.
Nulling them in place could collide on the unique place index.

`deck_code` is the file name of the deck's export, so it is unique.
The column folds case because Windows file names are case-blind.
The fold covers ASCII only.
So the Core code check is the full rule, and the index is a second guard.

A bearing is one direction of a deck, keyed by deck and direction.
A cue is one card of a deck, and its cells hold the card's typed columns.
Both cascade with their owner.

A cue reaches lexical rows through five references, one per grain.
Each is set to none when its row goes, so a lexical delete is never blocked.
The cue then stays, orphaned.
Every CHECK holds when a reference is none.
A meaning and a collocation are never bound together.

`LSchemaCueSettle` sets to none every cue reference whose row is absent from the named schema.
The rebuild runs with foreign keys off, so a sweep delete fires no SET NULL.
The foreign-key check also reports a SET NULL reference as a violation.
Without the settle, the sweep would delete the cue instead of orphaning it.
The sweep runs it at the top of every round.
A round can delete a Meaning, a Collocation or an Example whose own parent is gone.
The next round's settle then orphans the cue that pointed at it.
A single settle before the sweep would miss that chain.
`DatabaseCreate_SweptSenseParent_KeepsCueOrphaned` pins it.

## Stamp

Commit: `aae5349dbb94`
- `LSchemaDeck.cs` `58291bfae3d431c3`
