# PMentionLine.cs

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row and the corpus scribe each keep one.
It holds the chips alone and reads their words from the engine on every show.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineShow(LWindow window, string text, IReadOnlyList<LMentionDraft> mentions, string silent)`

Redraws the chips from the draft's Mentions over the given text.
The engine resolves every Mention in one call, and a Mention standing for nothing shows `silent` as its name.
Chips are diffed by position and replaced only where they differ, so an unchanged chip is not rebuilt.

## `internal void PMentionLineClear()`

Empties the chips, for when the row leaves the draft it was shown under.
