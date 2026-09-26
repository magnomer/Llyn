# PBylineItem.cs

## `internal sealed class PBylineItem`

One stored Author offered in the dropdown: the id of the stored Author and the name it reads.
Choosing a row credits that Author by id, so a rename in the workspace follows the credit.

## `public string PBylineItemLead`

The name is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The comparison is the culture's own and reports how long the match ran.
A match under a culture's rules is not always as long as the text that found it.
A name the typing does not appear in is held whole in the first piece.
A row is never lost to a search that cannot point at itself.

## `internal static long? PBylineItemRead(object? chosen)`

The Author a lit row names, or null while no row is lit.
The veneer thus passes a list's choice through unshaped.

## `internal static IReadOnlyList<PBylineItem> PBylineItemBuild(IReadOnlyList<LAuthor> rows, string word)`

One item per Author the engine offered, each split around the word typed.

## `internal static void PBylineItemApply(FrameworkElement container, object item, MouseButtonEventHandler press)`

Fills the three runs of one offered byline row.
It subscribes the press handed in on the row once, removing it first so a refill never doubles it.
