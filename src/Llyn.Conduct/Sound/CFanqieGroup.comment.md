# CFanqieGroup.cs

## `public sealed record CFanqieGroup(`

The fanqie readings of one character in one book, as the fanqie table groups them.

**Parameters**

- `CFanqieGroupHeading`: the character the group reads.
- `CFanqieGroupLabel`: the book's label.
- `CFanqieGroupSource`: the book's name.
- `CFanqieGroupStems`: the phonetic stems of the character, empty when it has none.
- `CFanqieGroupRows`: the readings, in the book's order.
