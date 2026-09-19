# LDiweiLine.cs

## `public sealed class LDiweiLine`

One line of a Diwei page section: a label, its reconstructed reading, and the characters placed there.
An initial page lines by rime and roundedness, a rime page by initial ranked under the Hypothesis.

## `internal static void LDiweiLineSort(List<LDiweiLine> lines)`

Orders the lines of a section by Hypothesis rank, unranked last, then by label.

## `internal void LDiweiLineAdd(string character)`

Adds a placed character once, skipping blanks.
