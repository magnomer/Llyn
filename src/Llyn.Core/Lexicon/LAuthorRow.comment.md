# LAuthorRow.cs

## `public sealed record LAuthorRow`

One credit row of the source editor, already decided for the veneer to copy.
The blank row the user types a new credit into is the editor's own and is not among these.

**Parameters**
- `LAuthorRowId`: the credited Author.
- `LAuthorRowName`: the name the engine holds for the Author.
- `LAuthorRowPosition`: the index the row takes among the credits.
- `LAuthorRowEarlier`: whether the row can move up.
- `LAuthorRowLater`: whether the row can move down.

## `public static IReadOnlyList<LAuthorRow> LAuthorRowCreate(IReadOnlyList<LAuthor> credits)`

The credits in order, each with its place and its two move verdicts.
