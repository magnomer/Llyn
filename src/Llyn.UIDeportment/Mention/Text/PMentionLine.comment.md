# PMentionLine.cs
Hash: `22f095195fcc0546`

## `internal sealed class PMentionLine`

The chip line one editor row shows its Mentions on.
A card row, the etymology field and the corpus scribe each keep one.
It holds the chips alone and paints the ready chips its owner's driver built.

## `public ObservableCollection<PMentionChip> PMentionLineChip { get; }`

The chips in Mention order, bound by the line's items control.

## `internal void PMentionLineRefine(IReadOnlyList<PMentionChip> wanted)`

Paints the ready chips on the line.
The engine alone owns their order, so the line copies the wanted order and never chooses one.
A line that already holds the wanted chips is left alone, so it raises no change.
Any difference clears the line and adds every wanted chip in order.
The card rows, the etymology and the corpus transcript build their chips through [QMentionChip](QMentionChip.comment.md).
