# LPortraitLegend.cs

## `public sealed record LPortraitLegend`

The localized words a page likeness is written with.
The engine holds no language facts, so the shell hands these in and the engine only places them.
The entry likeness has its own label record, because it names different things.

**Parameters**

- `LPortraitLegendUnknown` - the mark shown for a field whose value is unknown.
- `LPortraitLegendUntitled` - the title shown when the view has none.
- `LPortraitLegendUnwritten` - the head shown when an example's sentence was never written.
- `LPortraitLegendUnused` - the tally when nothing uses the view.
- `LPortraitLegendOnce` - the tally when one place uses it.
- `LPortraitLegendUses` - the tally word after a count of two or more.
- `LPortraitLegendTranslation` - the heading over an example's glosses.
- `LPortraitLegendSource` - the heading over an example's cited source.
- `LPortraitLegendAuthor` - the heading over a source's credited authors.
- `LPortraitLegendYear` - the heading over a source's year.
- `LPortraitLegendUrl` - the heading over a source's address.
- `LPortraitLegendNote` - the heading over a source's note.
- `LPortraitLegendDescription` - the heading over a situation's description.
- `LPortraitLegendKind` - the localized name of each source kind, falling back to the stored word.

## `public string LPortraitTallyFormat(int count)`

The usage chip for `count` places, worded as the display words it.

## `public string LPortraitKindFormat(LReferenceKind kind)`

The chip text for a source kind, in the reader's language when one was handed in.
