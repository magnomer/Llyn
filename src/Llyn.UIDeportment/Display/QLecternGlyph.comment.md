# QLecternGlyph.cs
Hash: `6b0375222c9ae963`

## `public sealed class QLecternGlyph`

The reading view's glyph section, drawing the glyph row [CDisplaySound](../../Llyn.Conduct/Display/CDisplaySound.comment.md) answers.
[QLectern](QLectern.comment.md) builds it once in its constructor over the view's page.
It pulls its own parts by contract ID, and the lectern subscribes its redraw to the display's open and close.

## `public QLecternGlyph(FrameworkElement surface, CDisplaySound area)`

Binds the chip list to its rows and holds the heading, all pulled from `surface`.
`area` is the display's sound area, the only part it reads.
The section and its label column go to a [QLecternLead](QLecternLead.comment.md), which shows them together.
The chip list is attached to `QGlyphItem.QGlyphItemRefine`, which fills each chip.
The chip command is bound here, so the sound strip holds no adapter for it.

## `public void QLecternGlyphRefine()`

Rebuilds the chips from the ready glyph row, and hides the section while it has no cell.
The lectern subscribes it to both open and close, since a closed display answers a blank row.
The glyph font goes into the list's resources, so the chips take it and the label does not.
The heading is looked up from the key Conduct chose, with the scheme's name as the fallback.

## `private void QLecternGlyphObserve(object sender, ExecutedRoutedEventArgs e)`

Hears a chip's command and hands the chip's character and language to the glyph gate.
The gate resolves the character and raises a row choice for the library tab.
An inert chip or anything else is ignored, since only a linked chip opens an entry.
