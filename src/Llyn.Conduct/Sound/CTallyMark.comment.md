# CTallyMark.cs
Hash: `ed651055b37d10f5`

## `public sealed record CTallyMark(string CTallyMarkText, int CTallyMarkCount, IReadOnlyList<string> CTallyMarkCharacters);`

One reflex mark in a tally, with the characters that carry it.

**Parameters**

- `CTallyMarkText`: the mark as printed.
- `CTallyMarkCount`: how many characters carry the mark.
- `CTallyMarkCharacters`: the characters carrying the mark.
