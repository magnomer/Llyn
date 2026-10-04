# LAuthorRow.cs
Hash: `5aa095a0df758003`

## `public sealed record LAuthorRow(long LAuthorRowId, string LAuthorRowName, int LAuthorRowPosition, bool LAuthorRowEarlier, bool LAuthorRowLater)`

One credit row of the source editor, already decided for the veneer to copy.
The blank row the user types a new credit into is the editor's own and is not among these.

**Parameters**

- `LAuthorRowId` — The credited Author.
- `LAuthorRowName` — The name the engine holds for the Author.
- `LAuthorRowPosition` — The index the row takes among the credits.
- `LAuthorRowEarlier` — Whether the row can move up.
- `LAuthorRowLater` — Whether the row can move down.

## `public static IReadOnlyList<LAuthorRow> LAuthorRowCreate(IReadOnlyList<LAuthor> credits)`

The credits in order, each with its place and its two move verdicts.
