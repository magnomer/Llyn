# QKindredItem.cs

## `internal sealed class QKindredItem : INotifyPropertyChanged`

One row of the entry list of the xiesheng panel: an entry of the chosen series.
It carries the name, the epithet and the language flag the row prints, and the chosen mark.

## `private QKindredItem(CVistaRow row, bool chosen)`

Copies one entry row off the Conduct row and finds its language flag.
The chosen mark is its own parameter.

## `public string QKindredItemHeadword`

The headword and the language are held only so a refresh can tell a changed row.

## `public bool QKindredItemChosen`

True while this is the entry the reader holds.

## `internal static IReadOnlyList<QKindredItem> QKindredItemBuild(IReadOnlyList<CVistaRow> rows)`

Copies a list of entry rows into rows the list can show.

## `internal static bool QKindredItemMatch(QKindredItem held, QKindredItem fresh)`

True while the held row already carries everything the fresh one says.

## `internal static void QKindredItemSync(QKindredItem held, QKindredItem fresh)`

Carries the chosen mark of the fresh row onto the held one and raises its notice.

## `internal static void QKindredItemRefine(FrameworkElement container, object item, string? _)`

Fills the flag, name and epithet of one entry row and marks the row while it is the chosen one.
The epithet leads with an en space, which the markup's format string once added.
