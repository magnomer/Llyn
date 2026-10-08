# QFootnote.cs
Hash: `0659f4f617103495`

## `internal sealed class QFootnote`

The middle column of the Source panel, with the entry search field over it.
It lists every Entry quoting an Example that cites the chosen Source, or every Entry while none is chosen.
Choosing a row shows that Entry in the display, through the shelf's entry gate.
It subscribes what it paints itself, so the owner `QReference` only builds and introduces it.

## `internal QFootnote(UserControl surface)`

Takes the Source page, and finds `PFootnote`, `PFootnoteEmpty` and `PRummage` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QFootnoteIntroduce(CShelf shelf, CAtelier atelier)`

`QReferenceIntroduce` calls it once the Conduct shelf exists.
It subscribes the entry list's rows event and the workspace opening.
It binds the list, takes each row's click, and attaches its row fill.

## `private async void QFootnoteVistaRefine()`

Answers the workspace opening for the entry list, which has its own first paint.
It subscribes before the owner, so it starts before the Source list's own load.
It awaits the flag load inside that request, then paints the entry rows.
Its one request is `CFootnoteRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QFootnoteRefine()`

Refills the entry list, and shows the empty text while no row stands.
The aperture chooses the text's key by whether the list is being searched.

## `private void QFootnoteRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QRummageObserve(object sender, TextChangedEventArgs e)`

Hands the typed entry search to the entry list's query gate.

## `private void QFootnoteObserve(object sender, RoutedEventArgs e)`

Hands the clicked entry's id to the shelf's entry gate.
