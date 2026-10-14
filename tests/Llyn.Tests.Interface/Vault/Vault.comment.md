# Vault

This folder holds the persistence relays.
It also holds the in-memory entry-side stores a fake rig is built from.

## TInterfaceDatabase

The relays for the infrastructure layer.
They open a store and run one store operation.
Entry-owned rows, forms, speeches, examples, media, notes, inflections, lacunae and readings are relayed here.
The database session, the doctor, the realm, the revision log and the workspace root are relayed here too.
A retirement of a part of speech is built and applied here, against the speech store.
The fanqie store saves and reads a character's rows here, and sets a row's representative rank.
The stores of what hangs on a meaning or collocation are relayed in `TInterfaceMeaning`.
The Ancestor store saves and reads a map and creates, lists and deletes relations and tongues here.
The Ancestor draft builders make one draft box, or turn a stored map into drafts and resolve drafts back.
Each relay is transparent.

## `TRealmValueRead`

The realm value of a test workspace, which the migration and realm tests compare across a rebuild.

## `TInterfaceIdentity`

The last temporary id a test built by hand.
It starts far below the ids a fresh workspace issues, so a hand-built id never names a real draft row.
A test counting on an unknown row would otherwise meet the row the engine just made, depending on test order.

## `TInflectionAnalysisSave`

The archive owns serialization and null handling, so the relay preserves every supplied analysis value unchanged.

## `TRevisionChangeRead`

Reads persisted revision changes directly in position order, independently of the archive adapter.

## TInterfaceVault

The relays for the persistence ports.
They open an adapter typed as its port and run one port operation.
A vault test proves the adapter keeps the port's promise, so it never sees the adapter's own type.
Each relay is transparent.
The language loader without arguments uses the temp folder and a source client that always answers not found.
The language builder makes a pack with no flag and blank fonts that declares the given varieties.
The vault fault builder makes the fault a port raises when its disk refuses, carrying an I/O cause.

## `TVaultFake`

An in-memory entry vault backed by a dictionary.
It counts every entry read so a test can prove the engine went through it.
The operations the test never drives throw, so a stray call is a failure rather than a silent pass.
A delete drops the row, so the entry clerk can be proved to leave a tombstone behind one.
The other readers and finders answer empty, so an engine read meets no rows rather than a failure.
The query find is the exception and answers every stored row.
`TRigFake` seats it as both the entry port and the entry query port of a rig built from fakes.
The add stores an entry under the next id, as an archive would, and returns it with that id.

## `TVaultFakeAncestor`

An in-memory Ancestor store that keeps one map per entry, as a fake rig needs.
A save mints a new id for each box and rewrites each child id to match, as an archive would.
The five built-in relations are seeded and cannot be deleted.
A create returns the item already held under the trimmed name, so a name never repeats.
A listed relation or tongue counts the boxes that use it across every map.
A delete of a custom item clears it from every box and keeps the boxes.
It then reports whether a row went.

## `TVaultFakeCue`

An in-memory cue store with cells per cue, keyed from one past a hundred.
A create appends to the deck, and a delete renumbers each deck it left.
A delete drops the cues' cells too, as the cascade does.
An update writes the flag, the exclusion and both texts, as the archive does.
A cell save keeps the cells by column, then by index.
A holder read answers the cue's own entry id, since the fake keeps no lexical rows.

## `TVaultFakeDeck`

An in-memory deck store with bearings per deck, as a fake rig needs.
It is built over a `TVaultFakeCue` and a `TVaultFakeStencil`, so its delete can mirror the archive's.
A delete drops the deck and its bearings, then the stencil it owns if that stencil is unsaved.
It removes the deck's cues through the cue port, as the cascade does in SQLite.
So a workflow test over the fakes sees what SQLite does.
A create appends to the folder, and a delete renumbers the decks left there.
An update keeps the grain, the stencil and the added time, as the archive does.

## `TVaultFakeFolder`

An in-memory deck folder store keyed from one past a hundred, as a fake rig needs.
It is built over a `TVaultFakeDeck`, so its delete can mirror the foreign key.
A folder delete sets its decks to the top level through the deck port.
So the deck fake stays the only writer of a deck.
A create appends the folder after the last one.

## `TVaultFakePronunciation`

An in-memory pronunciation store keyed from one, so a clerk save can run without SQLite.
Rows are kept by id and read back by entry, and the order set is accepted and ignored.
A recording save is refused, since no clerk test writes audio through it.

## `TVaultFakeReflex`

An in-memory reflex store, so an entry clerk save can run its reflex sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.
The anchor scan answers nothing, since no clerk test tallies anchors.

## `TVaultFakeRevision`

An in-memory revision log keyed from one, so a clerk test can read back what a delete recorded.
The change lists are kept as handed in, in the order recorded.

## `TVaultFakeStencil`

An in-memory stencil store keyed from one past a hundred, as a fake rig needs.
The list read answers saved stencils only, as the archive does.
An update replaces the name and the slots and keeps the saved mark.
The port has no delete, since no workflow deletes a stencil on its own.
The deck fake removes an owned stencil through the internal `TVaultFakeDelete`.

## `TVaultFakeTombstone`

An in-memory tombstone shelf keyed by entry id, so a clerk test can prove a delete filed one.
The read answers the tombstone filed for the entry, or null when none was.
No production seam reads a tombstone back, so only the fake answers.

## `TVaultFakeTranscription`

An in-memory transcription store, so an entry clerk save can run its transcription sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.

## `TVaultFakeWorkspace`

An in-memory workspace row, so an identity issuer and a revision pointer work without a database.
The floor descends by one on every adjust, as the archive's does, so a clerk test sees negative ids.
The size is always zero, since nothing here is on disk.
The realm is always the empty `Guid`, since no schema ever wrote one here.

## Stamp

Commit: `aae5349dbb94`
- `TInterfaceDatabase.cs` `da6742d17a5b907e`
- `TInterfaceVault.cs` `77bad45b5f2ce34c`
- `TVaultFake.cs` `7d9c55a8e3e857fb`
- `TVaultFakeAncestor.cs` `c045bd98f192efe5`
- `TVaultFakeCue.cs` `458fbecacde3fc62`
- `TVaultFakeDeck.cs` `47f068bafbd3bf0e`
- `TVaultFakeFolder.cs` `04b2a4e56b6ba408`
- `TVaultFakePronunciation.cs` `23fc443861d386b7`
- `TVaultFakeReflex.cs` `f5c05bb32d9f3a49`
- `TVaultFakeRevision.cs` `6f60bed6da1e60c5`
- `TVaultFakeStencil.cs` `80d9b2203f0acc6f`
- `TVaultFakeTombstone.cs` `00e6cc058522a937`
- `TVaultFakeTranscription.cs` `76c3478e6a91afcf`
- `TVaultFakeWorkspace.cs` `4823df3b3751b6f0`
