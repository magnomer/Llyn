# PLinkChip.cs

## `internal sealed class PLinkChip`

One committed link inside a card's Translation field.
The chip carries the id of another Entry and nothing else that is saved.
The headword, the language and the flag are read from that Entry to draw the chip.
They are display state, refreshed whenever the card is shown.
A link whose target should change is closed and written again rather than edited in place.
