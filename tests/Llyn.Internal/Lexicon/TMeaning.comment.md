# TMeaning.cs

## `public sealed class TMeaning`

Covers how the Meanings of an entry read back.
Sub-senses saved under a parent read back with that parent and their own positions.
The engine walks them into reading order for the word menu, each with its depth.
A Meaning with no title or definition takes the worded fallback.

## Inline notes

### `Assert.Throws<ArgumentOutOfRangeException>(() =>`

Meanings hang from an Entry and from nothing else.
