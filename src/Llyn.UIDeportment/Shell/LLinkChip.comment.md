# LLinkChip.cs

## `public sealed class LLinkChip`

One committed link inside a card's Translation field.
The chip carries the id of another Entry and nothing else that is saved.
The headword and the language are read from that Entry to draw the chip.
They are display state, refreshed whenever the card is shown.
A link whose target should change is closed and written again rather than edited in place.

## `public LLinkChip(long id, string headword, string language)`

Builds the chip and finds its flag for the language through `LEnsignImage`.
The flag is fixed once made, so the card builds again a chip made before its flag loaded.
