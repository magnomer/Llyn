# Database

Tests of the SQLite stores and the recovery of an unopenable database.
They stand apart because they pin what must survive a rebuild, a repeated launch or a failed write.
Fixtures that need the built schema take their workspace from `TWorkspace.TWorkspacePrepare`.

## `TAncestorArchive.cs`

Covers the stored ancestor map of an entry, its relations and its tongues.
A map saved with unsaved keys reads back as a tree, each source pointing at its child's stored id.
Sources keep their position under a child, and the root keeps its language and gloss.
Saving an empty list clears the map.
A box naming a missing child, or a ring of boxes, throws, since no tree can store it.
Deleting a linked entry keeps the box that named it, with its text and no link.
A map saved after its linked entry was deleted keeps that box as text too.
Deleting the owner entry takes its whole map away.
Deleting a child box takes its sources away.
A fresh workspace holds five built-in relations that cannot be deleted.
A new relation name is trimmed, and creating it again answers the same relation.
A used custom relation or tongue reports its box count.
Deleting it clears it from each box and keeps the boxes.
A draft saved with an already deleted relation or tongue stores it cleared and loses no box.
The entry load and the draft archive both give the map back as it was written.

## `TDeckArchive.cs`

Covers the stored decks, their bearings and the stencil list.
A created deck reads back every schedule value it was given.
An update that clears the quorum reads back null.
A deck delete takes its cues by cascade.
It takes the deck's own unsaved stencil and its slots too.
It keeps a saved stencil the deck pointed at.
A reversed order set reads back reversed and numbered from zero.
Two saved bearings read back as written.
A meaning cue reads back bound to its meaning.
Deleting that meaning keeps the cue and leaves it orphaned.
Nine text cells saved out of order read back by index.
The stencil list holds the saved stencil with its slots and leaves the owned one out.

## `TDatabaseSession.cs`

Covers the unit of work and the ordering it makes possible.
A session spanning several stores lands as one transaction or not at all.
A nested session leaves the decision to the outermost one.
Every ordered set stays numbered `0 … n-1` across a removal and a rewrite of the set.
Those are the reorders the unique position indexes used to make impossible.
A session whose connection is already closed still releases the ambient slot when disposed.
The next session must open cleanly, or every later store call would be refused.
An example update keeps its translations and the row ids of reordered ones.
The test identity counter hands out negative values that never repeat.

## `TDiweiArchive.cs`

Covers the diwei store over stored fanqie rows and a small hypothesis.
A character with two placements links every part of each.
It counts entries through the fanqie rows they anchor.
An entry with no anchored row counts nowhere, whatever its headword holds.
Two categories together pick the entries of one cell, and two that share no row pick none.
Applying stores the derived reading and tone class on each row, blank where the hypothesis has no answer.
A rebuild without a hypothesis drops the tone classes and orphaned categories, and blanks the readings.
A refetch drops the old row's links by cascade and the category left orphaned.
A rime key carries the 合口 mark on the rime and the division after it, and drops the 重紐 letter.
The 重紐 letter is read back on its own, empty for a rime that ends in no capital.

## `TDoctor.cs`

Covers the recovery of last resort, when the workspace database cannot be opened at all.
The healthy case is here because it is the one that must not fire.
A doctor that reset a working database would lose the user's work on every launch.
A database written at another version is the migration runner's to rebuild, so the doctor stays out.
A file that is not a database at all is refused by SQLite itself, and that is the doctor's case.
Both must end with the program holding a database it can use.

The repeat case guards the backup name.
Two rescues inside one second would otherwise land on the same name.
The second would erase the first copy, which this recovery promises never to do.

A file that cannot be opened at all, here a folder standing in its place, is not the doctor's case.
The fault passes through and nothing is set aside, since the file was never shown to be broken.
Corrupt and not-a-database errors rescue, while busy, full and read errors do not.
A busy or locked file is reported as such, so the launch can tell the user which program to close.

## `TLacunaCell.cs`

Covers the cell column of the lacuna store.
A one-value miss and a cell miss on the same first value are both kept.
The one-value row reads back with an empty cell.
A workspace one version older has a lacuna table without the cell column.
The rebuild carries its row across, and the row takes the empty cell.

## `TStemArchive.cs`

Covers the series store: the links a character's series makes, their counts and the entries they reach.
A character listed under two series links to both, and each series counts the entries it reaches.
Naming two series at once asks for their intersection, as a cell of the rime table does.
Relinking a character to another series drops the series nothing links to any more.
A character with no stored series row has nothing to link from, so no series is made.
A rebuild cuts every stored series text again and links each character to all of its series.
Dropping a character's series row takes its links with it.
A refetch relinks the row, so series it no longer names lose their link and other characters keep theirs.
The stored text is cut on the separator, trimmed and deduped, and left whole without one.

## `TVocabulary.cs`

Covers the integer identity of the vocabulary rows and the links that point at them.
A pack seeded twice must hand back the same row ids.
Otherwise every link would go stale on the second launch.
A renamed pack row keeps its id, so an entry that linked it shows the new name without being touched.
A user-added preset takes a negative code, so no pack can ever collide with it.
An entry whose part of speech no preset names stores the text and no link.
A draft that already links a value stores that link, and its display name is not consulted.
An inflection feature row hangs off the inflection's own id and links a `morphology_value` row.
A link to a value row that does not exist is refused by the clerk's link check.
The write then leaves nothing behind.

## Inline notes

### TDatabaseSession.cs: Assert.Equal(0, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM entry"))

The store committed its own nested session.
The outermost one never does.

## Stamp

Commit: `aae5349dbb94`
- `TAncestorArchive.cs` `b0f2c0f0c1f050fa`
- `TDatabaseSession.cs` `1fa6062d74eacb4d`
- `TDeckArchive.cs` `0890bf4a844ed16a`
- `TDiweiArchive.cs` `7ff34907ce49c7c7`
- `TDoctor.cs` `24949bade3a5f5ab`
- `TLacunaCell.cs` `7987b0edee4d5f16`
- `TStemArchive.cs` `90ac6e65c0a258ff`
- `TVocabulary.cs` `ec13137bc408eab1`
