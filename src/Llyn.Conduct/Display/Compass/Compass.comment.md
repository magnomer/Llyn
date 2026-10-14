# Compass

This folder holds the reading view's floating contents, its sections and its rows.

## `CCompass`

The compass is the reading view's floating contents.
It reads the rows of the contents list and the place of a card on the page.
It holds no state, and only the display builds one over its rules.
A repaint reads it, so a lasting name fault shows once through the repaint memory.
`CCompassCardFind` answers which card list of the shown entry holds the card, and at which place.
The engine finds the card's owning list, and a switch maps that owner to the compass part.
A linked collocation answers the place of the top-level meaning holding its home, where the page shows its row.
Its home is its first link to a meaning the draft still holds.
The engine's collocation layout answers the home, so the compass only finds the meaning listing the card.
A collocation with no such link is placed among the Other collocations, in their own order.
The place is counted in the order `CFolio.CFolioOrderRead` sets, as the reading view's cards are.
So the spotlight lands on the card the page shows, even when the engine list is out of order.
It answers null when nothing is shown or the entry has no such card.

### `CCompassRead`

It gives one row per shown part and one per card under it.
The driver names the parts that stand on the page, in page order, with its own text lookup.
With nothing shown, every part still gives its title row.
The collocation part lists only the collocations with no live link.
With none, the part gives no row at all, as the page then hides its Other section.
The homes and the Other list come from one engine read of the collocation layout.
The compass shows no flags, so it hands that read an empty target map.
A meaning with collocations adds one muted heading line one level below it, carrying that meaning's card place.
The line is worded with `Display.CollocationHeading`, the caption the page shows over those rows, and counts nothing.
It carries no number, since it names no card.
Cards come in the order `CFolio.CFolioOrderRead` sets.
So a row's card place is the index of the same card on the page.
Conduct chooses every wording key and the driver's lookup turns it into text.
A card is named by its own title, else by its kind, and an uncertain title by the unknown mark.
An Other collocation's kind is `Display.CollocationSingle`.
A card's number is carried apart from its name, since a number is not part of a sentence.
A card's title goes through the one state map.
The full card map is not run, since the contents need no lists or links.
The engine then numbers the names two rows share, so the contents never show two rows alike.
Heading lines skip that rule, since a caption is not a name.
`LCompassKeyRead` maps each part to its wording key by name, so an unknown part throws.
`LCompassNameResolve` makes the wordings distinct by the engine's twin rule.
A refused lookup is shown once through the repaint memory and answers the plain wordings.

## `CCompassPart`

A section of the reading view that the floating contents can name.
The driver tells which parts stand on the page, in page order.
Conduct then chooses each part's wording and lists the cards under it.

## `CCompassRow`

One row of the floating contents, ready to show.
`CCompassRowDepth` is zero for a section, one for a card under it, two for the heading line under a meaning.
`CCompassRowCard` is null for the section itself, and a heading line carries the place of its meaning.
`CCompassRowNumber` is empty for a section and for a heading line.
`CCompassRowName` is numbered while another row carries the same wording.
`CCompassRowMuted` marks the heading line, which is never numbered as a twin.

## Stamp

Commit: `aae5349dbb94`
- `CCompass.cs` `79aeda0895b429aa`
- `CCompassPart.cs` `8ca6c8843e2b6a56`
- `CCompassRow.cs` `55e13f23dc73ca46`
