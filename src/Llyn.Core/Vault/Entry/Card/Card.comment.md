# Card

This folder holds the persistence ports for the meanings, collocations, examples and glosses of an entry's cards.

## `LMeaningVault`

The persistence port for the Meaning rows, a tree of senses under an entry.

## `LMeaningCreate`

Inserts a meaning with a fresh opaque id at the end of its sibling group.
It returns the stored meaning with that id and its assigned position filled in.
A named parent must be an existing meaning in the same entry.
Otherwise an `InvalidOperationException` is thrown before anything is written.

## `LMeaningRead`

Reads the entry's whole Meaning tree as a flat list ordered by parent then position.
So a caller rebuilds the hierarchy from each meaning's parent id.
Siblings arrive already in order.

## `LMeaningSingleRead`

The one-row read a caller needs when it holds a Meaning id alone.
It answers null when no meaning carries that id.

## `LMeaningUpdate`

Updates the title and definition of the meaning.
The id, entry, parent link and position are untouched.
Where a Meaning sits among its siblings is changed by `LMeaningOrderSet`, which renumbers the whole group.
It throws when no meaning carries that id.

## `LMeaningParentUpdate`

Moves a Meaning under another Meaning of the same entry, or out to the entry's roots.
The row goes to the end of the group it joins, and the group it left is renumbered behind it.
So neither group is left with a gap, which the unique sibling index would punish on the next append.
A Meaning cannot be moved inside its own subtree, because a Meaning is not its own ancestor.
Nothing happens when the row already sits under that parent.

## `LMeaningDelete`

Deletes the meaning together with everything it owns.
That is its inline definition field, its subordinate meanings down the tree, and its example, tag and situation association rows.
Ancestor meanings are untouched, and the siblings left are renumbered so positions stay contiguous.
The independent Examples, Tags and Situations it referenced are left standing.
Only the links go.

## `LMeaningOrderSet`

Rewrites the positions of one sibling group so they follow the given order.
The group is the entry's top-level meanings when the parent is null, else the children of that parent.
Every id in the order is a meaning of that group.
A meaning left out keeps a position past the last.

## `LCollocationVault`

The persistence port for the Collocation rows, cards linked to the meanings they illustrate.

## `LCollocationCreate`

Inserts a collocation with a fresh opaque id at the end of its entry's order.
It returns the stored collocation with that id and its assigned position filled in.

## `LCollocationRead`

Reads the entry's collocations in card order.

## `LCollocationUpdate`

Updates the title, expression and meaning of the collocation.
The id, owning entry and position are untouched.
Card order is changed by `LCollocationOrderSet`, which renumbers the whole set.
It throws when no collocation carries that id.

## `LCollocationDelete`

Deletes the collocation.
Its meaning links and its other association rows go with it through the foreign-key cascade.
The independent Examples, Tags and Situations they pointed at are left standing.
The collocations left under the same entry are renumbered so their positions stay contiguous.

## `LCollocationOrderSet`

Rewrites the positions of the entry's collocations so they follow the given order.
A collocation left out keeps a position past the last.

## `LCollocationSenseRead`

Reads the ids of the meanings the collocation links, in link order.
The first is its home.

## `LCollocationSenseAttach`

Links the collocation to the meaning at a position in its link order.
`LCollocationSenseDetach` removes one link and leaves the meaning and the collocation standing.

## `LExampleVault`

The persistence port for the Example rows, the sentences that cards quote.

## `LExampleCreate`

Inserts an example with a fresh opaque id and returns the stored Example with that id filled in.
The new Example is referenced by nothing until it is attached to a referrer.
An Example's language is optional and an empty one is stored as written.
A sentence may be written in two languages, so no one language can be demanded of it.

## `LExampleRead`

Reads the Example by id, or null when no such Example exists.
The overload without an id gives every stored Example in insertion order.
One statement answers for the whole table, rather than one per Example.

## `LExampleUpdate`

Rewrites the Example's language, text, Source reference, Gloss list and Mention list.
The Example's own id and every reference pointing at it are untouched.
So an update never changes where the Example appears or in what order.
It throws when no Example carries that id.

## `LExampleTextUpdate`

Rewrites only the text of the Example.
A Mention whose span no longer fits the new text is dropped, and the rest are kept as they stand.
Shifting a span to follow an edit is the engine's work, not the store's.
It throws when no Example carries that id.

## `LExampleSourceUpdate`

Sets or clears the single Source the Example cites.
An unspecified source clears the citation.
Only the reference moves.
The Source row itself is never created, changed or removed here.
It throws when no Example carries that id.

## `LExampleReferenceRead`

Counts the references that still point at the Example.
It is the number `LExampleDelete` refuses a delete over.
A caller that has just detached one reference reads this to learn whether it was the last one.
No store of its own has to know which association tables exist.
The overload without an id counts the rows referencing each Example across both association tables at once.
The browsing panel needs the figure for every row it lists, so one statement answers for all of them.

## `LExampleDelete`

Deletes the Example, clearing every reference to it first when detach is set.
The clearing, the count and the delete share one session.
So the row is judged against the references as they stand at that moment.
The Source it cited is left standing, because a citation is a pointer and never ownership.

## `LExampleUsageRead`

Every card quoting one Example, itemized rather than counted.
A Meaning or Collocation row names the Entry it belongs to.
A row carries the entry id it is followed through.
So it stays followable after the text it shows is edited.
A quoting card with no wording of its own falls back to the definition or expression beneath it.

## `LGlossVault`

The persistence port for the Gloss rows, the renderings of an Example's sentence in other languages.

## `LGlossExampleRead`

Every Gloss of the Example, in position order.

## `LGlossExampleSave`

Rewrites the Gloss list of one Example and answers the written ids in input order.
The position of each row is its index in the list.
So the caller states the order by the order it sends.
Existing rows are shelved by shifting their position far up, so the unique (example, position) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LMentionExampleSave`.

## Stamp

Commit: `c2f335c331fe`
- `LCollocationVault.cs` `e8addaf6f4280679`
- `LExampleVault.cs` `5d07c9ed93b8b1be`
- `LGlossVault.cs` `84566b6b656d2469`
- `LMeaningVault.cs` `dd2452bda2202cc2`
