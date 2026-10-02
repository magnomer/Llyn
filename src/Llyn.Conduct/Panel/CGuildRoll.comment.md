# CGuildRoll.cs
Hash: `e9e0b1b258b3eab4`

## `public sealed record CGuildRoll(`

The guild's roll answer, ready to paint in one go.
The vita comes with the rows, so the view never reads the sheet a second time.

**Parameters**

- `CGuildRollRows`: the roll as the view lists it, each count worded.
- `CGuildRollEmpty`: whether the roll lists no row, so the empty notice shows.
- `CGuildRollVita`: the read sheet of the chosen Author, or the sheet of nobody while none is stored.
