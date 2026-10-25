# Compass

This folder holds the reading view's floating contents, its sections and its rows.

## `CCompass`

The compass is the reading view's floating contents.
It reads the rows of the contents list and the place of a card on the page.
Only the display builds one.
It hands over the rules, the card area, and reads of the shown header and the frequency chip.
Its one state is the list of parts that stand on the page, `CCompassShown`.
A repaint reads it, so a lasting name fault shows once through the repaint memory.
`CCompassCardFind` answers which card list of the shown entry holds the card, and at which place.
The card is named by a `collocated` flag and its id.
The card port finds the card's owning list under that kind.
A switch maps that owner to the compass part.
A linked collocation answers the place of the top-level meaning holding its home, where the page shows its row.
Its home is its first link to a meaning the draft still holds.
The engine's collocation layout answers the home, so the compass only finds the meaning listing the card.
A collocation with no such link is placed among the Other collocations, in their own order.
The place is counted in the order `CFolio.CFolioOrderRead` sets, as the reading view's cards are.
So the spotlight lands on the card the page shows, even when the engine list is out of order.
It answers null when nothing is shown or the entry has no such card.

### `CCompassSought`

The event says a card was sought, with its part and its place on the page.
The card area raises it when a "see" line opens a row.
The Library raises it for a meaning named by id.
It answers nothing when `CCompassCardFind` finds no card, so a stale id moves no view.
Both presentations scroll to the place, so the sought card lands where the page draws it.

### `CCompassLitRead`

It names the contents row that stands for the reading position.
Each row gives whether it is muted and whether it has reached the top of the page.
The last unmuted row to reach the top is lit, else the first unmuted row, else none.
A muted heading line is never lit, since it names no card.
The page-scroll code of both presentations calls it, so the two light the same row.

### `CCompassRead`

It gives one row per shown part and one per card under it.
The parts are `CCompassShown`, in the page order of `CCompassOrder`, and the driver gives only its text lookup.
With nothing shown, `CCompassShown` is empty and so are the rows.
The collocation part lists only the collocations with no live link.
A part with no wording key, the etymology and the stamp, gives no row, since the contents never name them.
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
`CCompassOrder` is the page order of every part, the one list the display and the driver share.
`LCompassShownRefine` recomputes `CCompassShown` from the shown header and the card area's kept answers.
The speech part shows when the entry is marked, and the frequency part when a chip is held.
The note and stamp parts show as the header says.
The meaning and collocation parts follow the cards, and the incoming part follows the usages.
The etymology part needs an ancestor.
The display calls it on every open, close, card, usage, etymology and frequency read, so the parts stay current.
`LCompassKeyRead` maps each part to its wording key by name, so an unknown part throws.
The etymology and the stamp map to no key.
`LCompassNameResolve` makes the wordings distinct by the engine's twin rule.
A refused lookup is shown once through the repaint memory and answers the plain wordings.

## `CCompassPart`

A section of the reading view that the floating contents can name.
The etymology and the stamp are parts of the page.
The contents list neither.
Conduct then chooses each part's wording and lists the cards under it.

## `CCompassRow`

One row of the floating contents, ready to show.
`CCompassRowDepth` is zero for a section, one for a card under it, two for the heading line under a meaning.
`CCompassRowCard` is null for the section itself, and a heading line carries the place of its meaning.
`CCompassRowNumber` is empty for a section and for a heading line.
`CCompassRowName` is numbered while another row carries the same wording.
`CCompassRowMuted` marks the heading line, which is never numbered as a twin.

## Stamp

Commit: `36e31eb71c98`
- `CCompass.cs` `f31dcfa5b66fbf17`
- `CCompassPart.cs` `f480a648a7e56468`
- `CCompassRow.cs` `55e13f23dc73ca46`
