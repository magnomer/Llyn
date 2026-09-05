# TMeaning.cs

## `public sealed class TMeaning`

Covers the engine's card seams.
A Meaning and a Collocation are created, read, rewritten, moved among siblings and deleted.
It is done one row at a time, not through the whole-form save the input panel uses.

## Inline notes

### `Assert.Throws<ArgumentOutOfRangeException>(() =>`

Meanings and Collocations hang from an Entry and from nothing else.
