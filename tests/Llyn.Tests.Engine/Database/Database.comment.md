# Database

Tests of the SQLite stores and the recovery of an unopenable database.
They stand apart because they pin what must survive a rebuild, a repeated launch or a failed write.
Fixtures that need the built schema take their workspace from `TWorkspace.TWorkspacePrepare`.

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

## `TEtymologyArchive.cs`

Covers the etymology store, holding the narrative row with its spans and the ordered source links.
Spans come back sorted by offset whatever order they were saved in.
Saving blank text drops the row, and its spans go with it.
A span must fit the text, name an entry, and overlap no other, and a broken one writes nothing.
Repeats, the entry itself and an empty id are skipped, and the rest keep their given order.
An empty list is how the links are dropped.
Deleting an entry takes every link and span naming it away, leaving the narrative standing.

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

Commit: `c2f335c331fe`
- `TDatabaseSession.cs` `1fa6062d74eacb4d`
- `TDiweiArchive.cs` `7ff34907ce49c7c7`
- `TDoctor.cs` `24949bade3a5f5ab`
- `TEtymologyArchive.cs` `978f595216e4bddc`
- `TLacunaCell.cs` `7987b0edee4d5f16`
- `TStemArchive.cs` `90ac6e65c0a258ff`
- `TVocabulary.cs` `ec13137bc408eab1`
