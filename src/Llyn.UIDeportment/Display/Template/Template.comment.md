# Template

Template holds the presentation items and fills for the reading card templates.
The items are plain copies of what a ready Conduct card prints.
The fills only paint them, so no template fill resolves lexical state or holds a Conduct record.
Texts are looked up under the keys Conduct chose, and a worded text shows its key's looked-up text.
Any other text shows as written.

## `QLeaf`

Named parts keep template presentation separate from Conduct gates.
Only ready items receive presentation.

### `QLeafCardRefine`

An untitled open card uses its kind caption, and an untitled folded card uses its peek.
`QCardFold` gives writing and reading cards one fold shape.
The heading collapses with the homed list when nothing homes on the card.
The "see" list collapses when the card has no "see" lines.

### `QLeafRowRefine`

A collocation row takes the same fold shape, so its hinge, stored fold and body follow the cards' rule.
The hinge stays the lectern's hinge gate, since the row's hinge carries the item like a card's.
A closed row cuts its meaning at the line end and shows only Conduct's filled markers.
An open row wraps its meaning and hides the markers, since its body then shows the fields they summarise.

### `QLeafBodyRefine`

A card and a row share one body fill.
A template without a contents part is left untouched.
Empty chip and example sections collapse rather than leaving blank rows.
Translations gain the chip margin when no situation row precedes them.

### `QLeafListRefine`

All chip lists, homed rows and "see" lines share one empty-row visibility policy.
Missing named lists are left untouched.

### `QLeafSeeRefine`

The line is a button whose item the lectern's see gate reads, so the fill adds no click.

### `QLeafSentenceRefine`

Sentence identity and mention pieces are kept so the click gate keeps its context.
A margin binding follows both the frame and sentence fonts, keeping their baseline relationship live.
The byline follows the sentence's margin and typography.
Gloss rows reuse the editor's Gloss filler.

### `QLeafLinkRefine`

Flag, headword and language arrive ready together, avoiding a target lookup during rendering.

## `QLeafItem`

This item stands for one meaning card or one collocation row.
Its constructor takes a `peek` that supplies the definition for a meaning card or the phrase for a collocation.
The lectern card picks it by the list the card comes in.
A muted title leaves the kind caption in its place, and a muted meaning line folds away.
Example, picture and film rows become their own items, so no fill holds a Conduct record.
Each collocation homed on a meaning card becomes a nested item.
A collocation item holds none, since the home rule gives a collocation no collocations.

### `QLeafItemId`

The id is handed back unread to the fold gate when the hinge is clicked.

### `QLeafItemStored`

It decides whether the hinge shows.

### `QLeafItemRank`

The badge formats the supplied position in the current culture and does not count the list.

### `QLeafItemMeaning`

The meaning is ready display text, independent of the folded header's selected peek.

### `QLeafItemTitle`

The title key is resolved once when the item is built.
A collocation row is built with its phrase as `peek`, since a row has no folded title place.

### `QLeafItemFlag`

One ensign per translation language of a closed row is mapped once when the item is built.
A language without an ensign gives an empty image, so the row never filters Conduct's list.

### `QLeafItemQuoted`

The example and "also" markers follow Conduct's verdict on whether the row has one.
They do not measure the text.

### `QLeafItemHome`

These are the collocations homed on this meaning or one below it, shown as folding rows beneath it.

### `QLeafItemSee`

These are "see" lines for collocations that also link here but live under another meaning.
Each carries the collocation's id for the click.

## `QLeafChip`

One situation, register or tag chip of a reading card.
It carries the ready chip for the open gate, and the plain text the chip prints.

## `QLeafImage`

One picture line, carrying the loading row and Conduct's empty verdict.
Each loading row is built as the vignette builds its own, and loads only once in view.
Lines keep the order Conduct gave them.

## `QLeafLine`

One example line, carrying plain copies so the fill never holds a Conduct record.
The sentence id is what a click hands back to the find gate.
Glosses are wrapped through `QGlossConverter`, so both modes share one flag resolver.
It only copies, so the driver neither cuts nor judges a line.

## `QLeafSee`

One "see" line under a meaning, naming a collocation homed under another meaning.
The id goes unread to the fold gate and the contents spotlight.
The text is the ready line Conduct worded.

## `QLeafVideo`

One film line, carrying the film row and Conduct's empty verdict.
Each film row is built as the vignette builds its own.

## Stamp

Commit: `2f9a06b8eb95`
- `QLeaf.cs` `28197bbac0cf4167`
- `QLeafChip.cs` `afe238a03dea2f47`
- `QLeafImage.cs` `ebc626cc614700b7`
- `QLeafItem.cs` `4a9ff3986e30777d`
- `QLeafLine.cs` `40a0d4aad94c015f`
- `QLeafSee.cs` `19c10c485fdc4d46`
- `QLeafVideo.cs` `84c2368cbae3d081`
