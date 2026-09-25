# PRepertoireEntry.cs

## `public partial class PRepertoire`

The Entry side of the Repertoire panel.
`POccurrence` lists the entries referencing the chosen Situation, or every Entry while none is chosen.
A chosen row swaps the Situation reading for the panel's own entry display.
The mode toggle swaps that display for `PEditor`.
The rail's new and save answer here, and the deportment picks the side each acts on.
The catalog and the Situation reading live in `PRepertoireBrowse.cs`.

## Inline notes

## `private void POccurrenceFind()`

Refills the middle column with the entries referencing the chosen Situation, or every Entry while none is chosen.
The engine matches the typed text and drops the hidden languages, so the panel decides nothing about what matches.
An empty result is shown rather than hidden, and its text says whether nothing references the Situation or nothing matches.
Rows sharing a headword are numbered afterwards, so the reader can tell them apart.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.

### Occurrence vista

The deportment's child vista, whose chosen entry the right-hand side stands on while it shows an Entry.
The right-hand side shows one or the other and never both.
An Entry row swaps the Situation reading for the entry display in place, and a Situation row swaps it back.
The chosen Situation keeps its mark meanwhile, because it still narrows the middle column.

## `private void POccurrenceHandle(object sender, RoutedEventArgs e)`

A clicked row asks the deportment to show that Entry.
The deportment asks about unsaved work before it moves, and keeps the side the panel stood on.

## `private void PRepertoireStoreHandle(object sender, RoutedEventArgs e)`

The rail's save, standing for whichever editor is in front.
The deportment saves an open Entry through the entry editor, and otherwise commits the held Situation.

## `private void PRepertoireFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, through the deportment.
With no Situation chosen and no Entry shown, it opens the editor on a Situation nothing has stored yet.
With a Situation chosen, or an Entry shown, it starts a new Entry carrying that Situation instead.
