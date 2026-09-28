# QRepertoireEntry.cs

## `internal sealed partial class QRepertoire`

The Entry side of the Repertoire panel.
`QOccurrence` lists the entries referencing the chosen Situation, or every Entry while none is chosen.
A chosen row swaps the Situation reading for the panel's own entry display.
The mode toggle swaps that display for `PEditor`.
The rail's new and save answer here, and the deportment picks the side each acts on.
The catalog and the Situation reading live in `QRepertoireBrowse.cs`.

## Inline notes

## `private void QOccurrenceFind()`

Refills the middle column with the entries referencing the chosen Situation, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing references the Situation or nothing matches.
The Conduct occurrence list picks that wording from whether its vista holds a query.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### Occurrence vista

The deportment's child vista, whose chosen entry the right-hand side stands on while it shows an Entry.
The right-hand side shows one or the other and never both.
An Entry row swaps the Situation reading for the entry display in place, and a Situation row swaps it back.
The chosen Situation keeps its mark meanwhile, because it still narrows the middle column.

## `private void QOccurrenceHandle(object sender, RoutedEventArgs e)`

A clicked row asks the deportment to show that Entry.
The deportment asks about unsaved work before it moves, and keeps the side the panel stood on.

## `private void QOccurrenceApply(FrameworkElement container, object item, string? _)`

Fills one occurrence row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The epithet leads with an en space, as its string format did.

## `private void QRepertoireStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
The deportment saves an open Entry through the entry editor, and otherwise commits the held Situation.

## `private void QRepertoireFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, through the deportment.
With no Situation chosen and no Entry shown, it opens the editor on a Situation nothing has stored yet.
With a Situation chosen, or an Entry shown, it starts a new Entry carrying that Situation instead.
