# PGroveItem.cs

## `internal sealed class PGroveItem : INotifyPropertyChanged`

One row of the series column: its key, its entry count and whether it is the chosen row.
Only the chosen mark changes in place, so a refresh never rebuilds a row the user is pointing at.

## `internal PGroveItem(LStem row, bool chosen)`

Copies one series off the engine row.

## `public bool PGroveItemChosen`

True while this is the chosen series, raising a notice as it changes.

## `internal static IReadOnlyList<PGroveItem> PGroveItemBuild(IReadOnlyList<LStem> rows)`

Copies a column of series into rows the list can show.

## `internal static bool PGroveItemMatch(PGroveItem held, PGroveItem fresh)`

True while the held row already carries everything the fresh one says.

## `internal static void PGroveItemSync(PGroveItem held, PGroveItem fresh)`

Carries the chosen mark of the fresh row onto the held one.

## `internal static void PGroveItemApply(FrameworkElement container, object item, string? _)`

Fills one series row's key and count and marks the row while it is the chosen one.
