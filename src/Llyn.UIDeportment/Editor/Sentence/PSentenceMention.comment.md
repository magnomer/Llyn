# PSentenceMention.cs

## `internal sealed partial class PSentence`

The row's side of the linking gesture: the chips showing the Mentions its Example holds.
The editor paints the chips from the sentence area's ready read after each redraw.
The row never resolves a Mention itself.
Every change goes out through a gate and comes back through the redraw.

## `public PMentionLine PSentenceChip { get; }`

The chip line under the sentence field.
