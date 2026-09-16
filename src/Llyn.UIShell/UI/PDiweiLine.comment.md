# PDiweiLine.cs

## `internal sealed class PDiweiLine`

One row of a category page: a reading from the hypothesis, its label, and every character placed there.
On an initial's page the row is a rime with its final.
Tones are ignored there, so 蘭 and 爛 share the 寒 row.
On a rime's page the row is an initial with its onset.
So 蘭 and 難 sit on the 來 and 泥 rows.

## `private readonly List<string> _pDiweiLineCharacters = [];`

The characters in order of first appearance, each once.

## `internal PDiweiLine(string kind, LFanqieRow row, LHypothesis? hypothesis)`

Takes the label from the first placement of the row and its reading from the hypothesis.
`kind` says which page the row belongs to: the rime and 開合 for an initial's, the initial for a rime's.

## `public string PDiweiLineReading`

The final or the onset between slashes, such as `/an/` or `/l/`, or empty when the hypothesis cannot resolve it.

## `public string PDiweiLineLabel`

The rime as the category store keys it, such as 寒, or the initial, such as 來.

## `public bool PDiweiLineRounded`

Whether the row is 合口, so the template can draw the chip, never on a rime's page.

## `public int PDiweiLineRank`

The initial's position across the hypothesis places on a rime's page, -1 when unlisted or on an initial's page.

## `public IReadOnlyList<string> PDiweiLineCharacters`

The characters placed in the row, each a chip back to its entry.

## `internal static void PDiweiLineSort(List<PDiweiLine> lines)`

Orders the rows of one section: by rank as the pack lists the initials, unranked rows after, then by label.
On an initial's page every row is unranked, so the rimes sort by label alone.

## `internal void PDiweiLineAdd(string character)`

Adds a character once, skipping blanks and repeats.

## `private static int PDiweiRankNormalize(int rank)`

An unlisted rank turned into the last position, so it sorts after every listed one.
