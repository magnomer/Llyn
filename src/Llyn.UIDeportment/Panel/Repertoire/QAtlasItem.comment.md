# QAtlasItem.cs
Hash: `593d5537a55622fd`

## `internal sealed class QAtlasItem : INotifyPropertyChanged`

Presentation item for one Situation row in `QAtlas`.
Carries the title, the kind, and the usage count the row shows, and the Situation id the row loads through.
The id is identity and never displayed.
Two Situations may carry the same title, so a row is never found by what it reads.
A title standing empty is not one thing.
It may never have been written, or it may have been written and be unknown now.
The row is given the mark for the second so the two stay distinct in the catalog.
It is given the untitled text for the first so no row stands blank beside the next.

## `internal QAtlasItem(CCatalogSituation row, bool chosen)`

Builds the row from the found Situation, whose kind and count already arrive worded.
The engine already named an untitled row and an unknown kind, so the row only copies the text.
The chosen mark comes as its own parameter, so the mark has one kind of writer.
A row is built while the list is being filled.

## `public string QAtlasItemCount { get; }`

How many Meanings and Collocations reference this Situation, as the row shows it.
It is the figure that decides whether a delete is legal.
The catalog carries it and not only the display.

## `public bool QAtlasItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The catalog row carries it, and `QSplice` moves the mark in place, so the list keeps its scroll position.

## `internal static bool QAtlasItemMatch(QAtlasItem held, QAtlasItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QAtlasItemSync(QAtlasItem held, QAtlasItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `QSplice` kept.
