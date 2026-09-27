# CTally.cs

## `public sealed record CTally(string CTallyLanguage, string CTallyKind, IReadOnlyList<CTallyMark> CTallyMarks);`

One reflex language's tally within a diwei section.

**Parameters**

- `CTallyLanguage`: the reflex language the tally names.
- `CTallyKind`: the reflex kind, such as literary or colloquial.
- `CTallyMarks`: the marks in the notation the section chose.
