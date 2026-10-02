# QCohortItem.cs
Hash: `f9097ed5bf415e27`

## `internal sealed class QCohortItem`

One Entry as a row of the tenor panel's entry list.
It mirrors the taxonomy panel's row, because both lists answer the same question of the workspace.
That question is which Entries stand under the thing chosen in the catalog beside them.
The headword and the shown name are separate, so two Entries sharing a headword can be numbered apart.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `internal QCohortItem(long id, string headword, string language, string epithet = "", bool chosen = false)`

A null epithet is stored as empty, so `QCohortItemMatch` compares strings only.

## `public string QCohortItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static bool QCohortItemMatch(QCohortItem held, QCohortItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void QCohortItemSync(QCohortItem held, QCohortItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `QSplice` kept.

## `public bool QCohortItemChosen`

The only value that announces a change, because `QCohortItemSync` sets it on a row already shown.
The kept row then repaints its mark without being rebuilt.
