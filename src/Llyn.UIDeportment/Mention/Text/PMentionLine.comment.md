# PMentionLine.cs

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row, the etymology field and the corpus scribe each keep one.
It holds the chips alone and paints the ready labels its owner reads.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineRefine(IReadOnlyList<CMentionLabel> labels)`

Paints the ready labels on the line as chips.
A label carrying a key shows the key's text as its name.
Conduct chose that key for a Mention standing for nothing.
Chips are diffed by position and replaced only where they differ, so an unchanged chip is not rebuilt.
The card rows, the etymology and the corpus transcript paint their areas' reads through it directly.
