# PRosterItem.cs

## `internal sealed class PRosterItem`

One row of the favorite catalog: the headword, its language, and the flag the language pack carries.
The id is held so the click can name the entry the row stands on.
A language with no readable flag leaves the image empty and shows none.

## `public bool PRosterItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The panel sets it instead of refilling the list, so the catalog keeps its scroll position.
