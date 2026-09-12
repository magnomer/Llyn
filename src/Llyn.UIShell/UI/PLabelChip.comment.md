# PLabelChip.cs

## `internal sealed class PLabelChip`

One committed Tag inside a card's Tag field — the boxed label the user sees.
It holds the name shown and the id of the stored Tag the chip links.
The id is `0` when the chip is text the user typed.
The engine names it on the next draft save.
A Tag whose text should change is closed and written again rather than edited in place.
