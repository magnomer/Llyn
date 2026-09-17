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

## `public string PMembershipItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`LTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PMembershipItemHeadword` keeps the plain headword for everything that is not display.

## `public string PMembershipItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.
