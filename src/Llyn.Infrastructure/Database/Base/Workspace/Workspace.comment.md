# Workspace

This folder holds the stores that hang off the workspace itself.
They are favorites, revisions, tombstones and the workspace row.

## `LFavoriteArchive`

It reads and writes the favorite marks.
A mark is one row keyed by its entry, so an entry is marked at most once.
The row is deleted with the entry.
Marking an already marked entry keeps the earlier stamp.
`LFavoriteFind` matches the headword folded for case, and an empty query returns every marked entry.
Rows come back by headword, then language, then entry id, the tie keys `LCatalogEntry` uses, so both sides agree.

## `LRevisionArchive`

It is the adapter of `LRevisionVault` and persists revisions with their ordered changes.
A revision is written once, complete.
`LRevisionRecord` stamps it with a fresh opaque id and the UTC time.
It inserts its changes in list order in one transaction.
So a half-written revision never reaches the database.
Each stored position comes from that order, not from the position the caller set.
An empty change list records an empty revision.

Change rows name their targets as recorded text rather than by foreign key.
So a revision stays readable after the rows it describes are deleted, which is the case the history exists for.
Nothing here deletes lexical data, since the stores that own it do.

## `LTombstoneArchive`

It is the adapter of `LTombstoneVault` and records that an Entry was deleted.
A tombstone is written after the Entry row is gone.
It names the row as recorded text, so it survives the deletion.
The revision it is filed under is a real reference and must already exist.
`entry_ref` is the primary key, so recording the same Entry twice is a conflict rather than a second row.
This store never deletes lexical data, since `LEntryArchive.LEntryDelete` does and the caller records the tombstone afterwards.

## `LWorkspaceArchive`

It is the adapter of `LWorkspaceVault` and persists the workspace row, the operational state of the session.
There is one such row per database, under id 1, so opening the workspace never searches for it.
The fixed id is this store's, not the caller's.
A caller-chosen id would let a second row appear that the read could never find.

The row names only rows of this database: the Entry each duplex side shows and the session revision.
How a panel orders, what it hides and which tab stands open live in the settings file instead.
The pane and revision columns are references the schema clears rather than defends.
Deleting an Entry sets the pane that showed it back to empty instead of blocking the delete.
Losing the whole row would lose no dictionary content.

`LWorkspaceStateRead` creates an empty row first when the database has none, so the caller never receives `null`.
`LWorkspaceStateSave` writes the row back, creating it when absent.
The identity floor is written only downward.
A state read before another engine issued an id would otherwise raise the floor.
That id could come round again.

`LWorkspaceFloorAdjust` lowers the identity floor by one and returns it.
One statement lowers and reads, so two engines on one workspace never receive the same number.
The row is created at minus one when the database has none.

`LWorkspaceSizeRead` gives the file length, or zero before it exists.

`LWorkspaceRealmRead` reads the realm row the schema writes once when it creates the store.
The bytes go back to a `Guid` in the order `LSchemaRealmCreate` wrote them.
A missing row throws, since the schema guarantees one and a guess would mislabel the workspace.

## Inline notes

### LWorkspaceArchive.cs: if (!reader.Read())

The insert above guarantees the row.
Reading nothing back would mean the row was removed between the two statements.
The session makes that impossible, so the code says so rather than dereference an empty reader.

## Stamp

Commit: `c2f335c331fe`
- `LFavoriteArchive.cs` `710ded0bebc2ea1f`
- `LRevisionArchive.cs` `ebcfc8e3e6412824`
- `LTombstoneArchive.cs` `d55ca427e93e2789`
- `LWorkspaceArchive.cs` `cf591d9618840594`
