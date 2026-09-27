# QShelfItem.cs

## `internal sealed class QShelfItem`

Presentation item for one Source row in the shelf list.
Carries the resolved name, the credited authors, the year, and the citation count.
The id is identity and never displayed.
A Source is shown by the first field it actually states, then by its id if it names itself nowhere.
It is never offered as a blank row the reader could not tell from the next one.
`Untitled` is a stated title and shows as one, and `Anonymous` is a credited Author.

## `internal QShelfItem(CCatalogReference row, string unknown, string unset, bool chosen)`

Builds the row from the catalog row the engine returned.
The name, the credits and the citation count arrive with it, so nothing is derived or read again here.
The two texts are handed in rather than read here.
A row is built while the list is being filled.

## `internal static IReadOnlyList<QShelfItem> QShelfItemBuild(IReadOnlyList<CCatalogReference> rows)`

One item per engine row, in the order the engine returned them.
The two placeholder texts are localized once for the whole list.
A plain copy loop, so the panel that asks for it carries no loop of its own.

## `public string QShelfItemCount { get; }`

How many Entries and Examples cite this Source, as the row shows it.
It is the figure that decides whether a delete is legal.
The catalog carries it and not only the editor.

## `public bool QShelfItemChosen`

Whether this row is the one the panel stands on, which the row fill marks for an accent edge.
It is the only value of the row that changes after the row is built.
The engine row carries it, and `LSplice` moves the mark in place, so the list keeps its scroll position.

## `internal static bool QShelfItemMatch(QShelfItem held, QShelfItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QShelfItemSync(QShelfItem held, QShelfItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.

## `internal static void QShelfItemApply(FrameworkElement container, object item, string? _)`

Fills one shelf row's named parts and marks the row while it is the chosen one.
The sources panel and the authors panel both attach it, and each takes the clicks on its own list.
