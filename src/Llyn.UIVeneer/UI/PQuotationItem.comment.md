# PQuotationItem.cs

## `internal sealed class PQuotationItem`

One Entry as a row of the corpus panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries quote the Example chosen in the catalog beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `public string PQuotationItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool PQuotationItemMatch(PQuotationItem held, PQuotationItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`PSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PQuotationItemSync(PQuotationItem held, PQuotationItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `PSplice` kept.
