# QOccurrence.cs
Hash: `c87543bf86cbbb8d`

## `internal sealed class QOccurrence`

The middle column of the Repertoire panel, with the entry search field over it.
It lists the entries referencing the chosen Situation, or every Entry while none is chosen.
A chosen row swaps the Situation reading for the panel's own entry display.
It subscribes what it paints itself, so the owner `QRepertoire` only builds and introduces it.

## `internal QOccurrence(UserControl scope)`

Takes the Repertoire page, and finds `PSortie`, `POccurrence` and `POccurrenceEmpty` in it by contract ID.
It sets the entry search hint and subscribes the search field.

## `internal void QOccurrenceIntroduce(CRepertoire repertoire, CAtelier atelier)`

`QRepertoireIntroduce` calls it once the repertoire Conduct exists.
It subscribes the occurrence list's rows event.
It takes the atelier only to hear the workspace opening, which gives the column its first paint.
It binds the list and attaches its row fill.

## `private async void QOccurrenceVistaRefine()`

Answers `CWorkspaceOpened` for the entry column, which shows the same flags as the catalog.
It waits for the flag load, then lists the column once.
Its one request is `COccurrenceRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QSortieObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the entry search text to the occurrence vista, whose announcement refills the entry column.

## `private void QOccurrenceRefine()`

Refills the middle column with the entries referencing the chosen Situation, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the driver decides nothing about what matches.
A failed read is reported by the Conduct occurrence list, which answers no rows, so the driver only fills.
An empty result is shown rather than hidden, and its text says whether nothing references the Situation or nothing matches.
The Conduct occurrence list picks that wording from whether its vista holds a query.
Rows sharing a headword arrive numbered, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

## Inline notes

### Occurrence vista

The Conduct's child vista, whose chosen entry the right-hand side stands on while it shows an Entry.
The right-hand side shows one or the other and never both.
An Entry row swaps the Situation reading for the entry display in place, and a Situation row swaps it back.
The chosen Situation keeps its mark meanwhile, because it still narrows the middle column.

## `private void QOccurrenceRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QOccurrenceObserve(object sender, RoutedEventArgs e)`

A clicked row asks the Conduct to show that Entry.
The Conduct asks about unsaved work before it moves, and keeps the side the panel stood on.

## `private void QOccurrenceItemRefine(FrameworkElement container, object item, string? _)`

Fills one occurrence row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.
