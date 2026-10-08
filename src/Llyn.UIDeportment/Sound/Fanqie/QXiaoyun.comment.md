# QXiaoyun.cs
Hash: `b90f355fb2bf9c9e`

## `internal sealed class QXiaoyun`

The entry list of the yunjing panel, with the entry search field over it.
It shows the Entries at the chosen cell, and the one the reader stands on.
Choosing a row loads it back from the workspace into the reader, or the editor when that side is open.
It subscribes what it paints itself, so the owner `QYunjing` only builds and introduces it.

## `internal QXiaoyun(UserControl surface)`

Takes the yunjing page, and finds `PXiaoyun`, `PXiaoyunEmpty` and `PBeacon` in it by contract ID.
It sets the search hint, attaches the row fill and subscribes the search field and the row clicks.

## `private TextBox QBeacon`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QXiaoyunIntroduce(CYunjing yunjing)`

`QYunjingIntroduce` calls it once the Conduct yunjing exists.
It subscribes the entry panel's rows event and binds the list.

## `internal void QXiaoyunRefine()`

Lists the entries at the chosen cell afresh with the empty text the session names.

## `internal void QXiaoyunRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QBeaconObserve(object sender, TextChangedEventArgs e)`

A change of the entry search field hands its text to the find gate.

## `private void QXiaoyunObserve(object sender, RoutedEventArgs e)`

A click on the entry list hands the row's id to the select gate.
