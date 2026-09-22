# LRequestEtymology.cs

## `public sealed record LRequestEtymologyText(`

Sets the prose of the entry's etymology to the text given.
The spans held on the draft are carried across the edit, and a span the edit destroyed is dropped.
One key for the whole field, so typing folds into one undo step.

## `public sealed record LRequestEtymologyMention(`

Makes the span at the offset given stand for one Entry, or clears it.
A zero Entry removes whatever span covers the range and adds none.
Any span the range touches is dropped first, so spans never overlap.
The key carries the offset, so two spans of one text are separate steps.

## `public sealed record LRequestEtymonAddition(`

Adds a direct link to the source Entry named, at the position given.
A link already held is refused rather than doubled.

## `public sealed record LRequestEtymonRemoval(`

Drops the direct link to the source Entry named.

## `public sealed record LRequestEtymonShift(`

Moves the direct link to the source Entry named to another position.
The key carries the Entry, so dragging one chip folds into one step.
