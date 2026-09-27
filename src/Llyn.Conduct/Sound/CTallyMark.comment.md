# CTallyMark.cs

## `public sealed record CTallyMark(string CTallyMarkText, int CTallyMarkCount, IReadOnlyList<string> CTallyMarkCharacters);`

One reflex mark in a tally, with the characters that carry it.

**Parameters**

- `CTallyMarkText`: the mark as printed.
- `CTallyMarkCount`: how many characters carry the mark.
- `CTallyMarkCharacters`: the characters carrying the mark.
