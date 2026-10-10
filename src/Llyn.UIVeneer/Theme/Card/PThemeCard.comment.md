# PThemeCard.xaml
Hash: `17b0761c837fc22b`

## `<sys:Double x:Key="Theme.Card.TitleSize">`

The measurements a card is drawn to, held here rather than in either card dictionary.
A card must read the same whether it is being written or being read.
The two are drawn by dictionaries that never meet.
So the numbers live where both can reach them.
Neither can drift from the other by an edit to one file.

## `<sys:Double x:Key="Theme.Card.ExampleSize">`

This size and `Theme.Card.ExampleFamily` are fallbacks only.
`QFontFace` sets both on each view's resources from the language pack.
Every dynamic reference to the keys then takes the pack's values.
A pack that declares none leaves these values in force.

## `<sys:Double x:Key="Theme.Card.GlossSize">`

The Gloss family, size and slant are fallbacks in the same way.
`QFontFace` sets them from the pack's Gloss typography.

## `<Thickness x:Key="Theme.Card.TitleMargin">`

The room a card title leaves at its right, the same in both modes.
It covers the eraser of 18, a gap of 2, the hinge of 26 and 16 of air.
The reading card has no eraser but keeps the room anyway.
So a long title is cut at the same width in both modes.
The peek and the reading caption take the same margin, so each stands where the title stands.

## `<Thickness x:Key="Theme.Card.EraserMargin">`

Moves the writing card's eraser left of the hinge, leaving a gap of 2 between them.
The eraser keeps this place while the hinge is collapsed on an unsaved card.

## `<CornerRadius x:Key="Theme.Card.FoldRadius">`

The header's corners and lower edge while its card is folded.
The body is gone then, so the header rounds all four corners and drops its bottom line.
The folded card reads as one closed bar inside the unchanged shell.
The card painter sets both keys on `PCardHeader` and clears them when the card unfolds.

## `<Style x:Key="Theme.Card.Shell" TargetType="Border">`

The card's outer box, the shared input card with a gap beneath it.
Both modes take it, so a card keeps its outline when the mode under it changes.

## `<Style x:Key="Theme.Card.Gutter" TargetType="ColumnDefinition">`

The strip kept clear down the right of every card body.
Writing a card needs handles beside each line, and reading it needs none.
Reserving the strip in both modes makes a line wrap identically whether or not the handles are there.

## `<Style x:Key="Theme.Card.Reach" TargetType="Grid">`

A row inside the body reaching back out into that strip.
The row's text keeps the body's width and its handles hang in the gutter beside it.
The transparent background makes the whole row hit-testable so hovering the gutter reveals the handles.

## `<Style x:Key="Theme.Card.Control" TargetType="FrameworkElement">`

A row's add and remove buttons start invisible and take no pointer.
The row driver shows both while its list is hovered or holds focus.
It then clears its values, so this style rules again.

## `<Style x:Key="Theme.Card.Pellet" TargetType="Border">`

A translation as a chip tinted in the soft accent.
A link and an etymon take the same chip.
A situation and a tag take styles of their own.
So a row of chips is never read as the wrong kind.

## `<Style x:Key="Theme.Card.Badge" TargetType="Border">`

A tag as an outlined chip with no fill of its own.
A label and a mention take the same outline.
Beside filled translation and situation chips, its outline marks a different kind.

## `<Style x:Key="Theme.Card.Situation" TargetType="Border">`

Situation chips use the dedicated situation palette in both modes.
Translations instead use the accent palette.

## `<Style x:Key="Theme.Card.Title" TargetType="TextBox">`

Each of a card's written fields at the size, weight and face the reading side gives that same field.
A writer therefore sees the card as it will be read.
Handles are added rather than a form put in its place.

## `<Style x:Key="Theme.Card.Ordinal" TargetType="TextBox">`

The number in a card's badge, written into rather than only read.
It stands read-only and untouched by the pointer until a double click opens it.
The badge above it therefore keeps taking the drag that moves the card.
The badge takes an accent ring while it is open, because a number being written must read as one.
The card driver paints the ring and enables number editing when the position opens.

## `<Style x:Key="Theme.Card.Handle" TargetType="Button">`

The small marks that add and drop a row, sized to sit in the gutter without pushing the line down.

## `<Style x:Key="Theme.Card.HingeFocus" TargetType="Control">`

The ring a hinge shows while it holds keyboard focus.
Its fixed six-pixel radius matches the hinge's hover surface.
A focus visual appears only for keyboard focus, so a mouse click leaves no ring behind.

## `<Style x:Key="Theme.Card.Hinge" TargetType="ToggleButton">`

The fold button at the right of every card header, the same in both modes.
Checked means folded.
The hit area is 26 square, wider than the 12 pixel chevron, so it is easy to strike.
The chevron is a `QIconImage`, so it keeps the icon's own colours like every other fold control.
Deportment gives the mark its icon, turn and tooltip through `QLook` rows on `PSurface` and `PSurfaceMark`.
The hand cursor overrides the header's drag cursor.
The button takes the press before the header sees it, so a click on the hinge never starts a drag.

## `<Style x:Key="Theme.Card.Peek" TargetType="TextBlock">`

The card's main text shown in the title's place while the card is folded and has no title.
It is muted, one line and cut with an ellipsis, so it reads as a glimpse and not a title.
On the writing card it stands behind the title box as a placeholder.
It takes no pointer, so a click there still reaches the title box.
It starts collapsed, and the card painter shows it.

## `<Style x:Key="Theme.Card.Switch" TargetType="ToggleButton">`

The mark that opens a frame on an Example row that carries none.
It carries a glyph rather than a drawing and holds no size of its own.
It stands on the line's baseline as the text beside it does.
A drawn mark of a fixed size sits where the row puts it.
That is never quite where the eye reads the line.
This style supplies padding.
`QLook` rows project that padding onto `PCardSurface` and paint its hover and checked grounds.

## `<Style x:Key="Theme.Card.Ghost" TargetType="TextBlock">`

A copy of what a field holds, drawn invisibly behind it.
The field is then exactly as wide as its text.
An inline field has to be measured this way.
Left to itself a drop-down takes the width of its widest offer.

## `<Style x:Key="Theme.Card.Dot" TargetType="TextBlock">`

The mark an example line opens with, drawn in the interface face it shares a baseline with.
It is not hit tested, because it is punctuation and not a place to write.

## `<Style x:Key="Theme.Card.Frame" TargetType="TextBlock">`

The parentheses a frame is written inside, set bold in the interface face and the accent colour.
No room is kept for the ring a written field is framed by, because the reading view keeps none either.
A frame the writing view widened would set the sentence further along than the card reads it.
The example keeps the reading face and its own weight.
The frame is never read as part of the sentence.
The invisible copies behind the written fields carry the same weight.
A bold field would otherwise outgrow the room measured for it.

## `<Storyboard x:Key="Theme.Card.Spotlight">`

One dip and return of a card's opacity, played when a click on a word lands on that card.
The reader's eye is led to the card without any colour the card does not already wear.
The display begins it on the card container after the card is scrolled into view.

## `<sys:Double x:Key="Theme.Card.DefinitionSize">`

Meaning and expression fields share a definition-size resource across modes.

## `<sys:Double x:Key="Theme.Card.SituationSize">`

Situation captions have a shared size independent of title text.

## `<sys:Double x:Key="Theme.Card.RegisterSize">`

Register captions have a shared size independent of title text.

## `<FontStyle x:Key="Theme.Card.GlossStyle">`

The normal gloss slant is a fallback for pack typography.

## `<sys:Double x:Key="Theme.Card.FrameSize">`

Frame punctuation and its invisible measuring text share this size.

## `<Thickness x:Key="Theme.Card.FrameMargin">`

Frame spacing separates grammatical context from the example text.

## `<FontFamily x:Key="Theme.Card.ExampleFamily">`

Example typography has a readable fallback when no pack override is available.

## `<FontFamily x:Key="Theme.Card.GlossFamily">`

Gloss typography has a readable fallback when no pack override is available.

## `<Thickness x:Key="Theme.Card.ExpressionMargin">`

A collocation expression is separated from its following definition.

## `<Thickness x:Key="Theme.Card.SituationMargin">`

Situation rows are spaced independently from their internal chips.

## `<Thickness x:Key="Theme.Card.RegisterMargin">`

Register rows retain their own spacing beneath preceding content.

## `<Thickness x:Key="Theme.Card.ChipMargin">`

The normal chip-row spacing is shared across card layouts.

## `<Thickness x:Key="Theme.Card.ChipTightMargin">`

Tighter chip-row spacing remains a separate resource rather than an inline exception.

## `<Thickness x:Key="Theme.Card.TagMargin">`

Tags remain visually separated from preceding card content.

## `<Thickness x:Key="Theme.Card.ExampleMargin">`

Example blocks share their outer spacing.

## `<Thickness x:Key="Theme.Card.ExamplePadding">`

Example indentation belongs to the common card theme.

## `<Thickness x:Key="Theme.Card.ExampleLineMargin">`

Example lines share separation from following content.

## `<Thickness x:Key="Theme.Card.MediaMargin">`

Media rows share indentation and outer spacing across modes.

## `<Thickness x:Key="Theme.Card.FoldEdge">`

A folded header loses its bottom line because no body follows it.

## `<Style x:Key="Theme.Card.Header" TargetType="Border">`

Open headers round only their top corners and retain a bottom separator.

## `<Style x:Key="Theme.Card.Crown" TargetType="Grid">`

Both card modes share the same header-content inset.

## `<Style x:Key="Theme.Card.Position" TargetType="Border">`

The circular number badge reserves identical space in reading and writing modes.

## `<Style x:Key="Theme.Card.Body" TargetType="Grid">`

Both card modes share the same body inset.

## `<Style x:Key="Theme.Card.Example" TargetType="Border">`

Example blocks use the shared margin and indentation resources.

## `<Style x:Key="Theme.Card.Media" TargetType="ItemsControl">`

Media lists use the shared media-row spacing.

## `<Style x:Key="Theme.Card.Preview" TargetType="Border">`

Media previews remain compact, outlined surfaces aligned to the left.

## `<Style x:Key="Theme.Card.Register" TargetType="Border">`

Register chips use helper colors rather than situation or translation colors.

## `<Style x:Key="Theme.Card.Meaning" TargetType="TextBox" BasedOn="{StaticResource Theme.Input.Bare}">`

Definition fields inherit bare-input behavior without title emphasis.

## `<Style x:Key="Theme.Card.Expression" TargetType="TextBox" BasedOn="{StaticResource Theme.Card.Meaning}">`

Expression fields inherit definition sizing but add semibold emphasis.

## `<Style x:Key="Theme.Card.Instance" TargetType="TextBox" BasedOn="{StaticResource Theme.Input.Bare}">`

Example text uses dynamic typography so pack resource overrides can reach it.
