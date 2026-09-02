# PTagChip.cs

## `internal sealed class PTagChip`

One committed Tag inside a card's Tag field — the boxed label the user sees.
It holds nothing but its name, because a Tag is nothing but its name.
There is no id to carry.
A Tag whose text should change is closed and written again rather than edited in place.
