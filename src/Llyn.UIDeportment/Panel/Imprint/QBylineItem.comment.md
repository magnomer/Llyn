# QBylineItem.cs
Hash: `306f99953f0cd7a3`

## `internal sealed class QBylineItem`

One stored Author offered in the dropdown: the id of the stored Author and the name it reads.
Choosing a row credits that Author by id, so a rename in the workspace follows the credit.

## `public string QBylineItemLead`

The name is held in three pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered rather than guessing.
The engine split the name where it matched, so the item only copies the pieces.

## `internal static long? QBylineItemRead(object? chosen)`

The Author a lit row names, or null while no row is lit.
The veneer thus passes a list's choice through unshaped.

## `internal static IReadOnlyList<QBylineItem> QBylineItemBuild(IReadOnlyList<CAuthor> rows)`

One item per Author the engine offered, each already split around the word typed.

## `internal static void QBylineItemRefine(FrameworkElement container, object item, MouseButtonEventHandler press)`

Fills the three runs of one offered byline row.
It subscribes the press handed in on the row once, removing it first so a refill never doubles it.
