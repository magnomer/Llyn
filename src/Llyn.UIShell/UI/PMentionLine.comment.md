# PMentionLine.cs

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row and the corpus scribe each keep one.
It holds the chips and the names they were read under.
So a redraw asks the engine only for what is new.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineShow(LEngine engine, string text, IReadOnlyList<LMentionDraft> mentions, string silent)`

Redraws the chips from the draft's Mentions over the given text.
Chips are diffed by position and replaced only where they differ, so an unchanged chip is not rebuilt.
The headwords are read in one batched call per show, and only for ids not read before.
A Meaning title is read once per sense id, falling back to its definition when the title is empty.

## `internal void PMentionLineClear()`

Empties the chips and forgets every name read, for when the row leaves the draft it was read under.
