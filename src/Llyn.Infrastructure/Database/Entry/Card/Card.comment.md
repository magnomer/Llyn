# Card

This folder holds the stores of the Meaning tree, the collocations and the glosses an entry's cards carry.

## `LCollocationArchive`

It persists the collocations an entry owns.
A collocation is a stable-id row assigned an opaque id here, ordered within its entry.
Updating rewrites title, expression and meaning in place, and reordering rewrites positions only, so ids survive it.
It links to zero, one or many Meanings of its entry, in link order.
The first link is its home.
The constructor also builds the link over `collocation_sense` that the Meaning links are written through.

Order within the entry is a unique index, so a position is never written one row at a time.
A new collocation is appended to the end.
`LCollocationOrderSet` renumbers the whole set through `LDatabaseOrder`.
The card update names the whole order because it has already applied every card and knows the ids.

`LCollocationCreate` takes its id from the space Collocations share with Meanings, `LSchemaCollocationSequence`.
`LCollocationUpdate` leaves the id, owning entry and position alone and throws when no collocation carries the id.

`LCollocationDelete` removes every row that links to it, Meaning links included, through the foreign-key cascade.
The independent rows those links pointed at, such as Examples and Meanings, are left standing.
The collocations left under the same entry are renumbered so positions stay contiguous.

`LCollocationSenseRead` gives the linked Meaning ids in link order, with the first as the home.
It throws when the collocation id is not positive.
`LCollocationSenseAttach` renumbers the set around the link, and a Meaning already linked moves to the position.
No check is made here that the Meaning belongs to the same entry.
`LCollocationSenseDetach` closes the gap and both the Meaning and the collocation survive.
Deleting either one removes its links through the cascade.

## `LMeaningArchive`

It persists the Meaning tree an entry owns.
Each meaning is a stable-id node assigned an opaque id here.
It may nest under another meaning in the same entry.
That parent link is validated before the row is written.
Reordering rewrites `position` only, so ids never change.
Deleting a meaning removes its subordinate meanings through the cascade, and deleting the entry removes the whole tree.

Sibling order is a unique index, so a position is never written one row at a time.
A new meaning is appended to the end of its sibling group.
`LMeaningOrderSet` renumbers the whole group through `LDatabaseOrder`.
Updating a meaning therefore changes its content and nothing about where it sits.

`LMeaningCreate` takes its id from the space Meanings share with Collocations, `LSchemaCollocationSequence`.
A named parent must be an existing meaning in the same entry.
Otherwise an `InvalidOperationException` is thrown before anything is written.

`LMeaningRead` gives the whole tree as a flat list ordered by parent then position.
A caller rebuilds the hierarchy from each `LMeaning.LMeaningParentId`, and siblings arrive in order.
`LMeaningSingleRead` is the one-row read, `null` when none carries the id.

`LMeaningUpdate` changes title and definition only and throws when no meaning carries the id.

`LMeaningParentUpdate` moves a Meaning under another Meaning of the same entry, or out to the entry's roots.
The row goes to the end of the group it joins, and the group it left is renumbered behind it.
So neither group is left with a gap, which the unique sibling index would punish.
A Meaning cannot be moved inside its own subtree.
Nothing happens when the row already sits under that parent.

`LMeaningOrderSet` names one sibling group, the top-level meanings when `parentId` is `null`, else the children of that parent.
The card update names the group by owner because it has already applied every card and knows the ids.

`LMeaningDelete` removes the meaning with its inline definition field and its subordinate meanings down the tree.
It also removes its example, tag and situation association rows.
Ancestor meanings are untouched, and the siblings left are renumbered so positions stay contiguous.
The independent Examples, Tags and Situations it referenced are left standing, and only the links go.

## `LGlossArchive`

It persists Glosses, the renderings of an Example's sentence in other languages.
A Gloss is the Example's own row, so it is read and written by Example.
`LExampleArchive` calls the static reader and saver here for every Example it returns or writes.
The rows sit in the `example_translation` table, named after the Translation the schema speaks of.

`LGlossExampleRead` gives the Glosses in position order.
The static form reads a whole set of Examples in one statement, keyed by Example.
The ids travel as one JSON parameter, so the whole corpus fits in one statement.
An Example with no Gloss has no key, and the caller reads that as an empty list.

`LGlossExampleSave` rewrites one Example's list and answers the written ids in input order.
The position of each row is its index in the list.
So the caller states the order by the order it sends.
Existing rows are shelved by shifting their position far up, so the unique (example, position) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LMentionExampleSave`.
The static form runs on a connection the caller holds, so an Example insert and its Glosses commit together.

## Inline notes

### LCollocationArchive.cs: LCollocationHolderRead

It gives the entry a collocation belongs to, or null when no collocation carries the id.

### LCollocationArchive.cs: private static IReadOnlyList<long> LCollocationSiblingRead

It gives the collocations of one entry in card order.

### LMeaningArchive.cs: private const string LMeaningSubtreeQuery

It walks the named meaning and every meaning beneath it with a recursive term over sense_parent.
The cycle check starts from this set.

### LMeaningArchive.cs: LMeaningHolderRead

It gives the entry a meaning belongs to and the parent it hangs from.
Those two values name its sibling group.
Both are null when no meaning carries the id.

### LMeaningArchive.cs: private static void LMeaningCycleValidate

A Meaning may not be moved under a Meaning that already sits inside it.
The subtree query answers whether the proposed parent is one of the row's own descendants.
Such a move would cut the branch off the entry and leave a ring the tree walk never ends on.

### LMeaningArchive.cs: private static IReadOnlyList<long> LMeaningSiblingRead

It gives the ids of one sibling group in order.
Root meanings have no parent id to key on, so their group is the entry's parentless meanings.
That is the same group the ifnull() expression index treats as one.

## Stamp

Commit: `c2f335c331fe`
- `LCollocationArchive.cs` `7d6be511d2061c2d`
- `LGlossArchive.cs` `71ec037cb2ae9ae1`
- `LMeaningArchive.cs` `61a2b84772780047`
