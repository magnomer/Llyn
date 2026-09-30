# QQuotationItem.cs

## `internal sealed class QQuotationItem`

One Entry as a row of the corpus panel's entry list.
It mirrors the taxonomy panel's row, because every middle column answers the same shape of question.
That question is which Entries quote the Example chosen in the catalog beside them.
An Entry referencing from several of its cards is still one row, because the row stands for the Entry.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `internal QQuotationItem(CVistaRow row, bool chosen)`

Copies one row the controller mapped, the flag found once for its language.
The chosen mark comes as a parameter, so the mark has one kind of writer.

## `public string QQuotationItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool QQuotationItemMatch(QQuotationItem held, QQuotationItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QQuotationItemSync(QQuotationItem held, QQuotationItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `QSplice` kept.
