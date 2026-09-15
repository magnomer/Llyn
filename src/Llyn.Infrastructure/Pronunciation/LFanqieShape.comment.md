# LFanqieShape.cs

## `internal sealed class LFanqieShape`

The compiled patterns of one book for one character, built once per answer.
Every pattern comes from the pack row, with the character in place of its token.

## `private const string LFanqieShapeToken = "{word}";`

The placeholder in a pack pattern that the character replaces, escaped for the regex.

## `private static readonly TimeSpan LFanqieShapePatience = TimeSpan.FromSeconds(2);`

How long any one pattern may run over one text before it is given up.

## `internal Regex LFanqieShapeMarker { get; }`

Marks the queried character inside a group, so only groups carrying it are read.

## `internal Regex? LFanqieShapeSplit { get; }`

Parts a cell into its groups, or `null` when a cell is one group.

## `internal Regex? LFanqieShapeHead { get; }`

Reads the named parts `rime`, `heading` and `division` from a group, or `null` to keep the head as text only.

## `internal Regex? LFanqieShapeColumn { get; }`

Reads the named parts `division` and `tone` from a column heading, or `null`.

## `internal Regex? LFanqieShapeRounded { get; }`

Means the marked character in a group is rounded, or `null` when the book does not say.

## `internal Regex? LFanqieShapeLine { get; }`

Reads one placement per match over the whole answer, or `null` when the book is read as tables.

## `internal Regex? LFanqieShapeSpelling { get; }`

Reads the 反切 spelling out of a table group as its first group, or `null` when the book prints none.

## `internal static LFanqieShape LFanqieShapeCreate(LFanqieBook book, string character)`

Compiles the book's patterns for the character.
The marker is required by the loader, and every other pattern compiles to `null` when the pack omits it.

## `private static Regex? LFanqieRegexCreate(string? pattern, string character)`

One pattern with the token replaced, dots crossing lines, or `null` for an absent pattern.
