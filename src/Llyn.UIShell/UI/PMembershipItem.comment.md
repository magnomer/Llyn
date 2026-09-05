# PMembershipItem.cs

## `internal sealed class PMembershipItem`

Presentation item for one entry row in `PMembership`.
Carries the headword and language the row shows, and the entry id the row loads through.
The id is identity and never displayed.
It is the taxonomy panel's own row, kept apart from the library panel's index row.
The two panels list the same entries but reach them through different questions.
The flag is resolved once for the language and handed to the row, not read from disk by the row.
A row is built while its list is being filled, and reading a file there would stall the fill.
