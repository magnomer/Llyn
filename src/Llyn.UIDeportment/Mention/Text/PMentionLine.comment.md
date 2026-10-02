# PMentionLine.cs
Hash: `2aae44d50459f259`

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row, the etymology field and the corpus scribe each keep one.
It holds the chips alone and paints the ready chips its owner's driver built.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineRefine(IReadOnlyList<PMentionChip> wanted)`

Paints the ready chips on the line.
Chips are diffed by position and replaced only where they differ, so an unchanged chip is not rebuilt.
The card rows, the etymology and the corpus transcript build their chips through [QMentionChip](QMentionChip.comment.md).
