# QGroveItem.cs
Hash: `823d0ceea72ab80c`

## `internal sealed class QGroveItem : INotifyPropertyChanged`

One row of the series column: its key, its entry count and whether it is the chosen row.
Only the chosen mark changes in place, so a refresh never rebuilds a row the user is pointing at.

## `private QGroveItem(CStem row, bool chosen)`

Copies one series off the Conduct row, with the chosen mark as its own parameter.

## `public bool QGroveItemChosen`

True while this is the chosen series.

## `internal static IReadOnlyList<QGroveItem> QGroveItemBuild(IReadOnlyList<CStem> rows)`

Copies a column of series into rows the list can show.

## `internal static bool QGroveItemMatch(QGroveItem held, QGroveItem fresh)`

True while the held row already carries the id, key and count of the fresh one.

## `internal static void QGroveItemSync(QGroveItem held, QGroveItem fresh)`

Carries the chosen mark of the fresh row onto the held one and raises its notice.

## `internal static void QGroveItemRefine(FrameworkElement container, object item, string? _)`

Fills the key and count of one series row and marks the row while it is the chosen one.
