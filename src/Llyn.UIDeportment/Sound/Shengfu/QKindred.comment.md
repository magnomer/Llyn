# QKindred.cs
Hash: `d2cc76085ab94811`

## `internal sealed class QKindred`

The entry list of the xiesheng panel, with the entry search field over it.
It shows the Entries the chosen series reaches, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QXiesheng` only builds and introduces it.

## `internal QKindred(UserControl surface)`

Takes the xiesheng page, and finds `PKindred`, `PKindredEmpty` and `PSextant` in it by contract ID.
It sets the search hint, attaches the row fill and subscribes the search field and the row clicks.

## `private TextBox QSextant`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QKindredIntroduce(CXiesheng xiesheng)`

`QXieshengIntroduce` calls it once the Conduct xiesheng exists.
It subscribes the entry panel's rows event and binds the list.

## `internal void QKindredRefine()`

Copies the entry list and its empty line again.

## `internal void QKindredRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QSextantObserve(object sender, TextChangedEventArgs e)`

Narrows the entry list as the field is typed into.

## `private void QKindredObserve(object sender, RoutedEventArgs e)`

Hands the pressed row to the panel's row gate, which records the voyage station and opens it.
