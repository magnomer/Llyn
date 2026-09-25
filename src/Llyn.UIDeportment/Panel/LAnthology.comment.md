# LAnthology.cs

## `public sealed class LAnthology`

The deportment of the corpus panel's example list: the panel state over the example vista and its rows.
The list finds rows and their usage, and takes the query, order and kind filter.
Its panel loads, edits and deletes the chosen Example.
The delete seam asks the removal seam with how many entries cite the chosen Example.

## `public bool LAnthologyNarrowed`

Whether the query or the filter hides any row, as the vista answers it.

## `private int LAnthologyUsageRead(long? id)`

How many entries cite the given Example, read fresh from the engine.
Zero when no Example is given.

## `private bool LAnthologyDeleteConfirm()`

Asks the removal seam whether to delete the chosen Example, given its usage.

## `public void LAnthologyCitationSet(string title)`

Points the Example's citation at the Reference the engine resolves the typed title to.
The engine reads the Reference cited now off the held draft, so an unchanged title keeps it.
