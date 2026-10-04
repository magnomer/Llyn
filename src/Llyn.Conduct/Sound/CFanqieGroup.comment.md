# CFanqieGroup.cs
Hash: `e29afd04aaf88d65`

## `public sealed record CFanqieGroup(string CFanqieGroupHeading, string CFanqieGroupLabel, string CFanqieGroupSource, IReadOnlyList<string> CFanqieGroupStems, IReadOnlyList<CFanqieRow> CFanqieGroupRows)`

The fanqie readings of one character in one book, as the fanqie table groups them.

**Parameters**

- `CFanqieGroupHeading`: the character the group reads.
- `CFanqieGroupLabel`: the book's label.
- `CFanqieGroupSource`: the book's name.
- `CFanqieGroupStems`: the phonetic stems of the character, empty when it has none.
- `CFanqieGroupRows`: the readings, in the book's order.
