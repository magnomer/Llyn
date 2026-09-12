# TMeaning.cs

## `public sealed class TMeaning`

Covers the engine's card seams.
A Meaning and a Collocation are created, read, rewritten, moved among siblings and deleted.
It is done one row at a time, not through the whole-form save the input panel uses.
Sub-senses created under a parent read back with that parent and their own positions.
The word menu walks them into reading order.

## Inline notes

### `Assert.Throws<ArgumentOutOfRangeException>(() =>`

Meanings and Collocations hang from an Entry and from nothing else.
