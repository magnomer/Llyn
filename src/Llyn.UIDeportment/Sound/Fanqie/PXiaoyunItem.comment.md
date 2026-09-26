# PXiaoyunItem.cs

## `internal sealed class PXiaoyunItem`

One Entry as a row of the yunjing panel's entry list, written with a character at the chosen cell.
It mirrors the tenor panel's row: flag, headword and the shown name, numbered apart when headwords twin.
The mark saying which row the reader stands on is the one thing that changes after the row is built.

## `public string PXiaoyunItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

## `internal static IReadOnlyList<PXiaoyunItem> PXiaoyunItemBuild(IReadOnlyList<LVistaRow> rows)`

A plain copy loop over the entries at the chosen cell.

## `internal static bool PXiaoyunItemMatch(PXiaoyunItem held, PXiaoyunItem fresh)`

Whether the two rows show the same values, the chosen mark left aside.
`LSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## `internal static void PXiaoyunItemSync(PXiaoyunItem held, PXiaoyunItem fresh)`

Copies the chosen mark of the fresh row onto the held row that `LSplice` kept.

## `internal static void PXiaoyunItemApply(FrameworkElement container, object item, string? _)`

Fills one entry row's flag, name and epithet and marks the row while it is the chosen one.
The epithet leads with an en space, which the markup's format string once added.
