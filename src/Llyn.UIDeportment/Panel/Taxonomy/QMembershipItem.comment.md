# QMembershipItem.cs
Hash: `ce3c25bec96a4c5b`

## `internal sealed class QMembershipItem`

Presentation item for one entry row in `QMembership`.
Carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
It is the taxonomy panel's own row, kept apart from the library panel's index row.
The two panels list the same entries but reach them through different questions.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.
The row announces its chosen flag, so the mark moves without the list being rebuilt.

## `internal QMembershipItem(long id, string headword, string language, string epithet = "", bool chosen = false)`

The epithet defaults to empty and the mark to unchosen, and a null epithet reads as empty.
The shown name is a required init, so no row is built without the engine's numbering.

## `public required string QMembershipItemName { get; init; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`QMembershipItemHeadword` keeps the plain headword for everything that is not display.

## `public string QMembershipItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool QMembershipItemMatch(QMembershipItem held, QMembershipItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QMembershipItemSync(QMembershipItem held, QMembershipItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `QSplice` kept.

## `public bool QMembershipItemChosen`

Whether this row is the one the panel stands on, which the row template paints an accent edge for.
The engine row carries it, and `QSplice` moves the mark in place, so the list keeps its scroll position.
