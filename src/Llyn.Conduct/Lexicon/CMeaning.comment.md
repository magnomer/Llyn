# CMeaning.cs
Hash: `f2b826b40505da82`

## `public sealed record CMeaning(long CMeaningId, string CMeaningName, int CMeaningDepth);`

One row of the sense menu, already in reading order.

**Parameters**

- `CMeaningId`: the Meaning the row picks.
- `CMeaningName`: the title, the definition, or the unknown label.
- `CMeaningDepth`: how deep the Meaning sits under its parents.
