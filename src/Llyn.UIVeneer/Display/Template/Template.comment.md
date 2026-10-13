# Template

This folder holds the read-only card shapes of the entry page.
They live apart from `PDisplay.xaml` because the card is its own shape, not part of the page around it.
Every `Theme.` reference here is dynamic, since a loose dictionary is parsed before the theme is in reach.
[PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md) fills every named part, so no template here names a handler.
`QLookDisplay` holds the hover, press and empty states as rows, not triggers.

## `PDisplayCard.xaml`

It holds the shared parts of a read-only card and merges the body-row dictionaries.
The meaning card and the collocation row both read its text styles.
A card body is a stack of independent rows, so each row is written where it can be read alone.
`Display.Card.Body` fixes their displayed order.
The picture and video rows live with the other media markup, in `PDisplayImage.xaml`.

### `Display.Card.Body`

Situations, registers, translations, examples, tags, pictures and videos read the same on both cards.
So the shared tail is one template, handed to each card through a content control.
Each row is named, so the fill sets its items and folds it away when it has none.
A meaning card adds its definition above it, and a collocation row its phrase and meaning.

### `Display.Card.Title`

Its right margin is `Theme.Card.TitleMargin`, the one the writing card's title field carries.
So a long title is cut at the same width in both modes.
The margin keeps room for an eraser this card never shows, beside the hinge it does show.

### `Display.Card.Definition`

The definition is the sentence the card exists for.
No label stands over it, because a label naming what a reader can see only pushes the reading down.

### `Display.Card.Kind`

Untitled, unfolded cards retain a muted kind caption in the title slot.

## `PDisplayMeaning.xaml`

It draws the meaning card as the reading view shows it.
The header carries position and title, and the body opens with the definition.
The shared body follows, then the folding rows of homed collocations, then the "see" lines.

### `Display.Meaning.Card`

The header row is fixed at the height the writing card's header takes.
So the body starts at the same depth in both modes.
The reading view reserves the gutter strip the writing view puts its handles in, and leaves it empty.
A line then breaks at the same word in both modes.

### `PCardHeader`

The header is named so the card painter can close its corners while the card is folded.
The writing card's header carries the same name, so one painter serves both modes.

### `PCardPeek`

It shows the definition in the title's place while the card is folded and untitled.
The kind caption gives way to it then.

### `PCardHinge`

It sits at the right edge where the writing card's hinge stands, with no eraser to its left.
The reading driver hears its click as it bubbles from the card list.

### `PCardHeading`

The muted "Collocations · N" caption is worded by Conduct.
The fill shows it only when the meaning has homed collocations.

### `PCardHome`

It lists collocations homed on this meaning or one below it, one folding row each in one bordered list.
They use the collocation row template, so a homed row looks like one under Other collocations.

### `PCardSee`

It holds the "see" lines for collocations that link here but live under another meaning.
Each line is a muted link the reading driver hears to open the row.

### `PCardBody`

The body is named so the card painter can collapse it while the card is folded.

## `PDisplayCollocation.xaml`

It draws the collocation row under its home meaning and in the Other list.
It also holds the "see" line a meaning shows for a collocation homed elsewhere.
A closed row is one line of hinge, phrase, the meaning cut at the line end, and the filled markers.
An open row adds the reading card's own body beneath, so every filled field shows as on a card.

### `Display.Collocation.Row`

The header line and the body are two rows of one grid, so the body starts under the phrase.
The body's left margin lines the fields up under the phrase, not under the hinge.
Its content control reuses `Display.Card.Body` and the same fill, so no field is drawn twice.

### `PCardHinge`

The row's chevron is the card hinge, moved to the left edge.
The reading driver hears its click as it bubbles from the list.
It is named like a card's so one fold painter checks it and hides it on an unstored row.

### `PCardMeaning`

It takes the room left between the phrase and the markers.
The row fill turns wrapping off while the row is closed, so the trimming cuts it at the line end.

### `PCardFlag`

It holds one ensign per translation language, in a single horizontal line.
The ensign is sized like the translation chip's flag, so the open body's chips match.

### `PCardBody`

The body is named so the fold painter collapses it while the row is closed.

### `Display.Collocation.See`

It is one "see" line, a bare button, so the reading driver hears its click as it bubbles.
The button carries the item that names the collocation whose row the click opens.
The label stays muted at the definition's size, and the hand cursor marks it as a link.
Its background is transparent rather than empty, so the whole line takes the pointer.

### `Display.Collocation.Marker`

It is the muted look the example and "also" markers share.
It stays collapsed until the row fill shows a filled one.

## `PDisplayExample.xaml`

It draws the example block of a read-only card.
Examples are indented as a block, because they are evidence for the definition rather than a field.
Each line opens with a dot, so a reader counts examples without reading them.

### `PExampleFrame`

The frame is drawn in the interface face and the accent colour the example never takes.
A reader tells at a glance what the card writes and what the card only marks.
It stands beside the example, as on the writing side.
A row carrying no frame drops it whole.

### `PExampleText`

The control that knows its words draws the sentence, so a reader can click one.
[PLeaf](../../../Llyn.UIDeportment/Display/Template/PLeaf.comment.md) sets its text, language and Mentions and keeps the frame out of the sentence.
Its language is the Example's own, because the sentence may be written in a language the entry is not.
Examples use the typography the language pack declares for them.
The fallback for a pack that declares none is written once, with the rest of a card's measurements.

### `Display.Card.ExampleGloss`

It draws one Gloss under the sentence as the flag of its language and the text.
The unknown mark stands in when the text is not known.
It takes family, size and slant from the entry's language pack and uses the muted colour.
The flag and globe come from the shared Gloss theme, so both modes draw the same mark.
Its top margin equals the bottom margin of the writing side's sentence field.

### `PExampleGloss`

The Glosses align with the sentence's first character rather than with the frame.
The margin below keeps the last Gloss nearer its own sentence than the next one.
Its parts carry the editor's Gloss names, so the editor's row fill sets them.
The list folds away when the Example carries none.

### Byline column

The byline of the cited Source stands at the right end of the example line, as dictionaries print quotations.
An example citing nothing collapses the column, so an uncited sentence runs to the edge.
The fill copies the sentence element's font, size and baseline drop, so the byline sits on its baseline.

## `PDisplayRegister.xaml`

`Display.Card.RegisterLink` draws a register as a chip, because it names the tone an entry is used in.
It takes the helper colour rather than the situation colour, so the two rows are never read as one.

## `PDisplaySituation.xaml`

### `Display.Card.SituationChip`

A situation names where or when the entry is used, so it is a chip.
It is the same chip the writing side is typed into, so the field does not change shape between modes.
The reading chip is a button, and the list the card sits in catches its click.
The chip carries no id, because what was clicked already stands for the situation drawn.
Translations and tags are read the same way, so no chip on a card is a dead end.

### `Display.Card.SituationLink`

It wears the chip border of the writing side.
The edge lights on hover and the chip dims while pressed, both switched by `QLookDisplay`.
That is the only sign the reading side gives that a chip is a link.
The translation and tag chips take the same treatment, so the three read as one kind of link.

## `PDisplayTag.xaml`

`Display.Card.TagLink` draws a tag, which is its own text and holds no id.
The chip is what the taxonomy is later asked to browse by.

## `PDisplayTranslation.xaml`

### `Display.Card.TranslationLink`

The pellet carries no edge of its own, so hovering draws one rather than repainting the fill.
Repainting would put the chip in the colour a selected chip wears elsewhere.
The edge is there in every state and only its colour changes.
An edge appearing on hover would widen the chip and shove its row.

### `Display.Card.TranslationChip`

A translation is a word in another language, drawn as a chip.
Tags take an outlined chip instead, so the two rows of chips are never read as one kind.
A translation chip opens the Entry it points at, and a tag chip opens the taxonomy browsing by that tag.

## `PDisplayUsage.xaml`

It holds the shapes of the page around the card.
They are the incoming rows above it and the stamps beneath it.
Nothing here answers an event, so the dictionary is loose and merged beside the card shapes.

### `Theme.Usage.Row`

It draws one entry whose sentence mentions the entry being read, as a card lighting under the pointer.
It is a button, because the row opens the entry it names.

### `Theme.Usage.Detail`

The sentence under the headword is trimmed to one line.
It collapses when the row has no sentence, so the headword stands alone.

### `Theme.Stamp.Label`

The stamp label is muted and small, because the stamps are a footnote.
`Theme.Stamp.Time` dresses the time beside it in the same face.

## Stamp

Commit: `c2f335c331fe`
- `PDisplayCard.xaml` `9189158db876e2c7`
- `PDisplayCollocation.xaml` `3319fa918b8df9eb`
- `PDisplayExample.xaml` `c3a66be3055d55b1`
- `PDisplayMeaning.xaml` `f965dfe515ea48aa`
- `PDisplayRegister.xaml` `d128fa6b868d9cb7`
- `PDisplaySituation.xaml` `da80ba1cee6b4e0f`
- `PDisplayTag.xaml` `ba5fabbaeee382f4`
- `PDisplayTranslation.xaml` `d881e278e4be7660`
- `PDisplayUsage.xaml` `2aef77ba042119d7`
