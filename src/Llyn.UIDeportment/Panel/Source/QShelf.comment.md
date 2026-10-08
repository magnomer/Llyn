# QShelf.cs
Hash: `d249657b23c1f4b4`

## `internal sealed class QShelf`

The left column of the Source panel, with the Source search field over it.
It lists every Source the workspace holds, including one nothing cites.
Choosing a row shows that Source in the colophon, through the shelf's select gate.
It subscribes what it paints itself, so the owner `QReference` only builds, introduces and first paints it.

## `internal QShelf(UserControl surface)`

Takes the Source page, and finds `PShelf`, `PShelfEmpty` and `PSurvey` in it by contract ID.
It sets the search hint and subscribes the search field.

## `internal void QShelfIntroduce(CShelf shelf, QColophon colophon)`

`QReferenceIntroduce` calls it once the Conduct shelf exists.
It takes the colophon driver, because each Source answer carries the colophon's tally.
It subscribes the Source list's rows event.
It binds the list, takes each row's click, and attaches its row fill.

## `internal void QShelfRefine(CShelfRoll roll)`

Paints the rows, the empty notice and the colophon tally from one shelf answer.
The rows are spliced, so the list keeps its scroll position.
`QReferenceVistaRefine` hands in the roll its flag load answered, so the paint asks Conduct nothing.

## `private void QShelfRefine()`

Reads the roll and paints it, in answer to the rows notice.

## `private void QSurveyObserve(object sender, TextChangedEventArgs e)`

Hands the typed Source search to the query gate.

## `private void QShelfObserve(object sender, RoutedEventArgs e)`

Hands the clicked Source's id to the shelf's select gate.
