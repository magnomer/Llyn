# PRosterItem.cs

## `internal sealed class PRosterItem`

One row of the favorite catalog: the headword, its language, and the flag the language pack carries.
The id is held so the click can name the entry the row stands on.
A language with no readable flag leaves the image empty and shows none.

## `public bool PRosterItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
It is the only value of the row that changes after the row is built.
The engine row carries it, and `PSplice` moves the mark in place, so the list keeps its scroll position.

## `public string PRosterItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`PRosterItemHeadword` keeps the plain headword for everything that is not display.

## `public string PRosterItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool PRosterItemMatch(PRosterItem held, PRosterItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`PSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PRosterItemSync(PRosterItem held, PRosterItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `PSplice` kept.
