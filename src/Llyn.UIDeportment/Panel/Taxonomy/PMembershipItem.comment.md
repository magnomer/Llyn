# PMembershipItem.cs

## `internal sealed class PMembershipItem`

Presentation item for one entry row in `PMembership`.
Carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
It is the taxonomy panel's own row, kept apart from the library panel's index row.
The two panels list the same entries but reach them through different questions.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.
The row announces its chosen flag, so the mark moves without the list being rebuilt.

## `public required string PMembershipItemName { get; init; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
`PMembershipItemHeadword` keeps the plain headword for everything that is not display.

## `public string PMembershipItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool PMembershipItemMatch(PMembershipItem held, PMembershipItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PMembershipItemSync(PMembershipItem held, PMembershipItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.
