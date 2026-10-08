# QLecternFrequency.cs
Hash: `557735b484228775`

## `public sealed class QLecternFrequency`

The reading view's frequency section, drawing the chip the display's area answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID and subscribes its own events, so nothing is handed in late.

## `public QLecternFrequency(FrameworkElement surface, CDisplay display)`

Pulls the frequency section, its chip, and the name and band lines the chip shows from `surface`.
The chip redraws on both the area's open and close, and on the frequency notice marshalled through `QObserver`.
A closed area chooses no entry, so the chip collapses its section then.
