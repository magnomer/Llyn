# LTallyRow.cs
Hash: `c92108897254182f`

## `public sealed record LTallyRow(string LTallyRowLanguage, string LTallyRowKind, IReadOnlyList<LTallyMark> LTallyRowMarks)`

One tally line of a Diwei section with its marks already taken from the shown set.
The page picks the set once, so no reader chooses between the phonemic and the respelled marks.

**Parameters**

- `LTallyRowLanguage` — The reflex language the line names.
- `LTallyRowKind` — The reflex kind, such as literary or colloquial.
- `LTallyRowMarks` — The marks of the shown set, never empty.
