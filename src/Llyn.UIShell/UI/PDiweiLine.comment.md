# PDiweiLine.cs

## `internal sealed class PDiweiLine`

One row of a category page: the final's reading, the rime, and every character placed there.
Tones are ignored, so 蘭 and 爛 share the 寒 row.

## `private readonly List<string> _pDiweiLineCharacters = [];`

The characters in order of first appearance, each once.

## `internal PDiweiLine(LFanqieRow row, LHypothesis? hypothesis)`

Takes the rime and 開合 from the first placement of the row and the final's reading from the hypothesis.

## `public string PDiweiLineReading`

The final between slashes, such as `/an/`, or empty when the hypothesis cannot resolve it.

## `public string PDiweiLineRime`

The rime as the category store keys it, such as 寒.

## `public bool PDiweiLineRounded`

Whether the row is 合口, so the template can draw the chip.

## `public IReadOnlyList<string> PDiweiLineCharacters`

The characters placed in the row, each a chip back to its entry.

## `internal void PDiweiLineAdd(string character)`

Adds a character once, skipping blanks and repeats.
