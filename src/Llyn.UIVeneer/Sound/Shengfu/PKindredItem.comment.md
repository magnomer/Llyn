# PKindredItem.cs

## `internal sealed class PKindredItem : INotifyPropertyChanged`

One row of the entry list of the xiesheng panel: an entry of the chosen series.
It carries the name, the epithet and the language flag the row prints, and the chosen mark.

## `internal PKindredItem(LVistaRow row, bool chosen)`

Copies one entry row off the engine row and finds its language flag.

## `public bool PKindredItemChosen`

True while this is the entry the reader holds, raising a notice as it changes.

## `internal static IReadOnlyList<PKindredItem> PKindredItemBuild(IReadOnlyList<LVistaRow> rows)`

Copies a list of entry rows into rows the list can show.

## `internal static bool PKindredItemMatch(PKindredItem held, PKindredItem fresh)`

True while the held row already carries everything the fresh one says.

## `internal static void PKindredItemSync(PKindredItem held, PKindredItem fresh)`

Carries the chosen mark of the fresh row onto the held one.
