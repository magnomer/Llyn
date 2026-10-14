# Card

The card theme holds the shared looks of the entry card and the rows it carries.
They serve writing and reading mode alike.
The two modes are drawn by dictionaries that never meet, so their shared numbers and styles live here.
Neither mode can then drift from the other by an edit to one file.
Hover, press and chosen looks are switched by `QLook` rows in the deportment.

## `PThemeCard` measurements

A card must read the same whether it is being written or being read.
`Theme.Card.TitleMargin` is the room a title leaves at its right, the same in both modes.
It covers the eraser of 18, a gap of 2, the hinge of 26 and 16 of air.
The reading card has no eraser but keeps the room anyway.
So a long title is cut at the same width in both modes.
The peek and the reading caption take the same margin, so each stands where the title stands.
`Theme.Card.EraserMargin` moves the writing card's eraser left of the hinge, with a gap of 2 between them.
The eraser keeps this place while the hinge is collapsed on an unsaved card.
`Theme.Card.FoldRadius` and `Theme.Card.FoldEdge` shape the header of a folded card.
The body is gone then, so the header rounds all four corners and drops its bottom line.
The card painter sets both keys on `PCardHeader` and clears them when the card unfolds.
Open headers in `Theme.Card.Header` round only their top corners and keep a bottom separator.
`Theme.Card.ExampleSize`, `Theme.Card.ExampleFamily`, the Gloss family, size and slant are fallbacks only.
`QFontFace` sets them on each view's resources from the language pack.
Every dynamic reference then takes the pack's values, and a pack that declares none leaves the fallbacks in force.
`Theme.Card.Instance` uses dynamic typography so pack resource overrides can reach example text.
The remaining margin, padding and size keys are shared across modes so both lay out the same.
`Theme.Card.FrameSize` is shared by frame punctuation and its invisible measuring text.

## Card shell and body

`Theme.Card.Shell` is the card's outer box, so a card keeps its outline when the mode under it changes.
`Theme.Card.Gutter` is the strip kept clear down the right of every card body.
Writing needs handles beside each line and reading needs none.
Reserving the strip in both modes makes a line wrap identically whether or not the handles are there.
`Theme.Card.Reach` is a row reaching back out into that strip.
So its text keeps the body's width and its handles hang in the gutter.
Its transparent background makes the whole row hit-testable, so hovering the gutter reveals the handles.
`Theme.Card.Control` starts a row's add and remove buttons invisible and pointer-free.
The row driver shows both while its list is hovered or holds focus.
It then clears its values so this style rules again.
`Theme.Card.Handle` is the small add and drop mark, sized to sit in the gutter without pushing the line down.
`Theme.Card.Crown` and `Theme.Card.Body` give both modes the same header-content and body inset.
`Theme.Card.Position` is the circular number badge, reserving identical space in both modes.

## Chips

`Theme.Card.Pellet` is a translation as a chip tinted in the soft accent.
A link takes the same chip.
`Theme.Card.Badge` is a tag as an outlined chip with no fill.
A label and a mention take the same outline.
Beside the filled translation and situation chips, its outline marks a different kind.
`Theme.Card.Situation` uses the dedicated situation palette in both modes, while translations use the accent palette.
`Theme.Card.Register` chips use helper colours rather than situation or translation colours.
So a row of chips is never read as the wrong kind.

## Written fields

`Theme.Card.Title` sets each written field at the size, weight and face the reading side gives it.
A writer therefore sees the card as it will be read.
Handles are added rather than a form put in its place.
`Theme.Card.Meaning` and `Theme.Card.Expression` inherit bare-input behaviour, and the expression adds semibold emphasis.
`Theme.Card.Ordinal` is the number in a card's badge, written into rather than only read.
It stands read-only and untouched by the pointer until a double click opens it.
The badge above it therefore keeps taking the drag that moves the card.
The badge takes an accent ring while open, because a number being written must read as one.
The card driver paints the ring and enables number editing when the position opens.
`Theme.Card.Ghost` is a copy of what a field holds, drawn invisibly behind it.
So the field is exactly as wide as its text.
An inline field has to be measured this way.
That is because a drop-down left to itself takes the width of its widest offer.

## Hinge and peek

`Theme.Card.Hinge` is the fold button at the right of every card header, where checked means folded.
The hit area is 26 square, wider than the 12 pixel chevron, so it is easy to strike.
The chevron is a `QIconImage`, so it keeps the icon's own colours like every other fold control.
Deportment gives the mark its icon, turn and tooltip through `QLook` rows on `PSurface` and `PSurfaceMark`.
The hand cursor overrides the header's drag cursor.
The button takes the press before the header sees it, so a click on the hinge never starts a drag.
`Theme.Card.HingeFocus` is the ring the hinge shows while it holds keyboard focus.
Its fixed six-pixel radius matches the hinge's hover surface, and a mouse click leaves no ring behind.
`Theme.Card.Peek` is the card's main text shown in the title's place while the card is folded and has no title.
It is muted, one line and cut with an ellipsis, so it reads as a glimpse and not a title.
On the writing card it stands behind the title box as a placeholder and takes no pointer.
So a click still reaches the title box.
It starts collapsed, and the card painter shows it.

## Example rows

`Theme.Card.Switch` is the mark that opens a frame on an Example row that carries none.
It carries a glyph rather than a drawing and holds no size of its own.
So it stands on the line's baseline as the text does.
A drawn mark of a fixed size sits where the row puts it.
It is never quite where the eye reads the line.
This style supplies padding, and `QLook` rows project it onto `PCardSurface` and paint the hover and checked grounds.
`Theme.Card.Dot` is the mark an example line opens with, in the interface face it shares a baseline with.
It is not hit tested, because it is punctuation and not a place to write.
`Theme.Card.Frame` is the parentheses a frame is written inside, bold in the interface face and the accent colour.
No room is kept for the ring a written field is framed by, because the reading view keeps none either.
A frame the writing view widened would set the sentence further along than the card reads it.
The example keeps the reading face and its own weight.
The frame is never read as part of the sentence.
The invisible copies behind the written fields carry the same weight.
Or a bold field would outgrow the room measured for it.

## `Theme.Card.Spotlight`

One dip and return of a card's opacity, played when a click on a word lands on that card.
The reader's eye is led to the card without any colour the card does not already wear.
The display begins it on the card container after the card is scrolled into view.

## Caption styles

`Theme.Caption.Button` gives the window caption buttons their shared dimensions.
Deportment switches their hover and press through `QLook` rows on the named surface.
`Theme.Caption.Close` names the close button so `QLook` gives it the warning colour on hover and press.

## Category styles

`Theme.Category.Item` stretches category content across a rounded row.
Deportment lights hover and press and sets the padding through `QLook` rows on the named parts.

## Etymology styles

The etymology map is a tree of boxes, drawn the same way in the editor and the reading view.
Each box is a centred column of its tongue, its word and its gloss.
The boxes it descends from stand in a row above it, joined to it by a trunk rule.
Under the box a stem carries its relation word down to the fork of its siblings.
`Theme.Etymology.Sources` lays the source boxes out as a centred row aligned to the foot of their line.
`Theme.Etymology.Item` is the editor's box.
It holds a tongue choice, the word field, a gloss field and the box tools.
It also holds a restore note and a relation choice.
`Theme.Etymology.Reading` is the reading view's box, with the same parts as plain text and no tools.
Both templates name their parts alike, so each driver fills the same names.
Each template nests its own sources list, so the tree needs no flattening in markup.
`PEtymologyTrunk` is the rule between a box and its sources, and a driver hides it while there are none.
`PEtymologyPrior` and `PEtymologyNext` are the left and right half-bars along the foot of a box.
A driver hides the left half on the first sibling and the right half on the last.
So a row of siblings draws one bar from the first box's centre to the last box's centre.
`Theme.Etymology.Bar` fills those half-bars in the line colour.
`Theme.Etymology.Frame` rings a box that is its own, and `Theme.Etymology.Trace` is the dashed ring of a borrowed box.
The trace starts collapsed, so a driver shows it only when a box is borrowed from another entry.
`Theme.Etymology.Stem` is the short upright rule of the trunk and of the relation's two links.
`Theme.Etymology.Tool` is the small square button for insert, add and remove, and it is never focusable.
`Theme.Etymology.Note` is the link-like restore button, centred under the tools.
`Theme.Etymology.Root` is the accent-ringed box the entry's headword stands in at the foot of the map.
`Theme.Etymology.Headword` is that headword, centred and in the accent colour.
The part rests on the palette and the input part.

## Gloss styles

The look of a Gloss row, shared by the card editor and the corpus panel.
The input part is merged.
So the remove control and the boxed field stand on the same styles as every other input.
`Theme.Gloss.Line` is the list of rows, folded away by a `QLook` row while it holds none.
A bare field draws its hover frame past its own bounds.
So the margin above the list keeps the first row's frame clear of the sentence's.
The margin under each row keeps two neighbouring frames apart for the same reason.
`Theme.Gloss.Choice` and `Theme.Gloss.Option` draw one language of the picker as its flag and name, lit by `QLook` rows.
`Theme.Gloss.Globe` is the ring drawn in place of a flag while the row has no language yet.
`Theme.Gloss.Flag` is hidden while there is no language.
`Theme.Gloss.Name` shows the language name, or the muted language label while none is chosen.
`Theme.Gloss.Remove` is the cross that drops the row.
Its command is a `QLook` row, and the row fill gives the row as its parameter and the icon.
`Theme.Gloss.Row` is the inline row under a card's sentence.
It holds the flag that opens the picker and the bare text field.
The flag toggle is pulled left by its own padding, so the flag starts where the sentence above starts.
The text takes the family, size and slant the entry's language pack declares for a Gloss.
They come through the card resources the panel sets.
`PGloss.PGlossRowApply` fills every named part, wires the picker and sets its value path.
The text and the placeholder come from the state converter's logic.
What is typed leaves through the editor's own handler, so the row holds no copy of it.
`Theme.Gloss.Field` is the boxed row of the corpus edit area, with the language toggle, the boxed field and the cross.
`Theme.Gloss.Display` is the read-only line of the corpus read area.
That is where the unknown mark stands in when the text is not known.

## Glyph styles

The look of the glyph row in the editor and of the character chips in the reading view.
The transcription styles are merged, so the editor row reads as a transcription row without its dropdown.
The reading view draws the same row with one chip per character in the field's place.
Each Han chip links into its entry.
Both panels read the glyph face and size from `Theme.Glyph.Family` and `Theme.Glyph.Size`.
The display and the editor put those keys on their lists.
`Theme.Glyph.Family` is the fallback face when the pack declares none, the same face `Theme.Phonetic` gives the other reading rows.
It is spelled out rather than aliased, because a `StaticResource` entry in a deferred dictionary broke the styles after it.
`Theme.Glyph.Phonetician` raises the glyph notation command so the menu opens in the glyph scheme.
It hides when the section declares no sources, as a Japanese pack does, since a search would find nothing.
The command is a `QLook` row, and the editor sets the visibility from the list tag.
`Theme.Glyph.Field` is drawn as a pronunciation field but in the glyph face and size.
The phonetic face is meant for IPA and Latin schemes, and a Han character reads wrong in it.
`Theme.Glyph.Measure` is the unseen measuring copy of the field, sized by its text or by the glyph placeholder when blank.
It takes the same face and size as the field, so the two measure alike.
`Theme.Glyph.Chip` is one character of the reading view, inked like the field's text and turned accent and underlined on hover.
It carries the field's medium weight and no margin between chips.
That is because the field runs its characters together and the row must match it.
Its parameter is the character item, and the display resolves the entry when the command fires.
A chip whose item carries no language is inert, with no hand cursor, no tooltip and no hit test.
The command and the hover are `QLook` rows, and the display fills the character and the parameter.
`Theme.Glyph.Line` is the chip run standing in the field's place.
It has the field's vertical margin, so the chips sit on its line.
`Theme.Glyph.Row` holds the scheme label in the shared label column, then the field, its measuring copy and the lookup button.
It has no plus or minus, because a language has one glyph row at most.
`QGlyph.QGlyphItemRefine` fills the named parts, the scheme label included, and observes the typed text.
`Theme.Glyph.Display` is one chip, filled through `QLookItemAttach` and `QGlyphItem.QGlyphItemRefine`.

## Marker styles

`Theme.Marker.Surface` is the editor's part-of-speech chip, at the height, margin and radius of `Theme.Speech.Chip`.
So a part of speech keeps its box between reading and writing.
Its edge is painted in its fill, so the accent edge on hover widens nothing.
`Theme.Marker.Field` is the pill a new part of speech is typed into.
It has the height and radius of the chips beside it.
The row then reads as chips ending in an empty one.
The border is a hairline until pointed at or typed in.
Deportment switches the border and the ground through `QLook` rows.
`Theme.Marker.Text` is the typing inside that pill, sized and coloured as a chip's name, so committing moves nothing.
`Theme.Marker.Switch` is the chevron that opens the presets, held inside the pill, and it also heads a folded sound part.
The chevron turns over while the switch stands open, so it points the way the next click goes.
The ground behind it warms to the soft accent and never the accent itself.
That is because the mark carries its own blue.
Deportment turns the chevron and warms the ground through `QLook` rows on `PSurface` and `PSurfaceMark`.
`Theme.Marker.Menu` is the presets menu, lifted off the form by a faint shadow.
The heavier popup shadow belongs to windows that cover their page, which this one does not.
`Theme.Unit.Dropper` is the unit chooser, a chip outlined rather than filled.
The outline tells a choice of one apart from the filled chips of the parts of speech.
`Theme.Freedom.Dropper` is outlined the same way, since it too is a choice of one.
`Theme.Unit.Label` and `Theme.Freedom.Label` write the chosen name as a chip's name is written.

## Mention styles

`Theme.Mention.Linked` underlines a word that stands for an Entry in the accent colour.
So a reader sees which words lead somewhere.
The cursor turns to a hand over it, so the run itself answers the pointer and no code searches pieces.
Anywhere else the cursor is left to whoever answers next, which in the reading view is the band's beam.
An unlinked word may open an Entry too, but only the engine knows that, and the cursor does not ask.
`Theme.Mention.Silent` draws a word marked as standing for nothing plain.
It is still a piece of its own, so a click on it is answered without asking the engine.
`Theme.Mention.Menu` is the menu the linking gesture is asked through, built once and attached to every sentence field by key.
Link, choose, silence and unlink each carry one of the four Mention commands, as `QLook` rows on the named items.
The command routes from the field that holds the focus, so it reaches the field's host.
Which items are enabled follows the selection, answered by the host's command checks when the menu opens.
Cut, copy and paste stand below a separator, because a custom menu replaces the field's own.
`Theme.Mention.Line` is the wrapped chip line under a sentence field, collapsed by a `QLook` row while empty.
A row without Mentions is as tall as it was before the line existed.
`Theme.Mention.Sense` is the Meaning title on a chip.
It is hidden while empty, which is when the Mention stands for the whole Entry.
`Theme.Mention.Chip` shows the word, an arrow, the headword it stands for, and the Meaning when narrowed.
A word standing for nothing shows the silent mark in place of a headword.
The remove button asks the unlink command with the chip as its parameter.
Deportment fills the named parts, the command and the icon through `QLookItemAttach`.
The chip is drawn as a badge rather than a pellet.
That is because it reports a link and is not the link itself.

## Stamp

Commit: `c6e60141a172`
- `PThemeCaption.xaml` `3e87659ad411aeea`
- `PThemeCard.xaml` `17b0761c837fc22b`
- `PThemeCategory.xaml` `331d3745f7a12209`
- `PThemeEtymology.xaml` `419b0cb30ad13b63`
- `PThemeGloss.xaml` `7069508ab866d557`
- `PThemeGlyph.xaml` `340069918a46ca84`
- `PThemeMarker.xaml` `9d4e0f67fd3de81e`
- `PThemeMention.xaml` `29c5f6b673dfd8e0`
