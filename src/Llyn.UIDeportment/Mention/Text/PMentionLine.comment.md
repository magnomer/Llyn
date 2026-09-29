# PMentionLine.cs

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row and the corpus scribe each keep one.
It holds the chips alone and reads their words from the engine on every show.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineShow(CAtelier atelier, string text, IReadOnlyList<CMentionDraft> mentions, string silent)`

Redraws the chips from the draft's Mentions over the given text.
The engine resolves every Mention in one call, and a Mention standing for nothing is named by `silent`.
It hands the labels to `PMentionLineRefine`, whose key lookup words a silent chip the same way.

## `internal void PMentionLineRefine(IReadOnlyList<CMentionLabel> labels)`

Paints the ready labels on the line as chips.
A label carrying a key shows the key's text as its name.
Conduct chose that key for a Mention standing for nothing.
Chips are diffed by position and replaced only where they differ, so an unchanged chip is not rebuilt.
The card rows paint the sentence area's read through it directly.

## `internal void PMentionLineClear()`

Empties the chips, for when the row leaves the draft it was shown under.
