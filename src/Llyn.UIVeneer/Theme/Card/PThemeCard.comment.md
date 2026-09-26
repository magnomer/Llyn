# PThemeCard.xaml

## `<sys:Double x:Key="Theme.Card.TitleSize">`

The measurements a card is drawn to, held here rather than in either card dictionary.
A card must read the same whether it is being written or being read.
The two are drawn by dictionaries that never meet.
So the numbers live where both can reach them.
Neither can drift from the other by an edit to one file.

## `<Style x:Key="Theme.Card.Shell" TargetType="Border">`

The card itself, its heading strip, its number and the box its fields stand in.
Both modes take these, so a card keeps its outline when the mode under it changes.

## `<Style x:Key="Theme.Card.Gutter" TargetType="ColumnDefinition">`

The strip kept clear down the right of every card body.
Writing a card needs handles beside each line, and reading it needs none.
Reserving the strip in both modes makes a line wrap identically whether or not the handles are there.

## `<Style x:Key="Theme.Card.Reach" TargetType="Grid">`

A row inside the body reaching back out into that strip.
The row's text keeps the body's width and its handles hang in the gutter beside it.
The transparent background makes the whole row hit-testable so hovering the gutter reveals the handles.

## `<Style x:Key="Theme.Card.Pellet" TargetType="Border">`

A translation, a situation and a tag as chips.
A translation is tinted blue, a situation tinted amber and a tag outlined.
A row of chips is therefore never read as the wrong kind.

## `<Style x:Key="Theme.Card.Situation" TargetType="Border">`

A situation chip in its own amber, on the reading and the writing side alike.
Amber is warm against the blue a translation takes and carries none of the meaning green or red would.

## `<Style x:Key="Theme.Card.Title" TargetType="TextBox">`

Each of a card's written fields at the size, weight and face the reading side gives that same field.
A writer therefore sees the card as it will be read.
Handles are added rather than a form put in its place.

## `<Style x:Key="Theme.Card.Ordinal" TargetType="TextBox">`

The number in a card's badge, written into rather than only read.
It stands read-only and untouched by the pointer until a double click opens it.
The badge above it therefore keeps taking the drag that moves the card.
The badge takes an accent ring while it is open, because a number being written must read as one.
The card templates that draw the badge add the ring and the opening, since both follow the card.

## `<Style x:Key="Theme.Card.Handle" TargetType="Button">`

The small marks that add and drop a row, sized to sit in the gutter without pushing the line down.

## `<Style x:Key="Theme.Card.Switch" TargetType="ToggleButton">`

The mark that opens a frame on an Example row that carries none.
It carries a glyph rather than a drawing and holds no size of its own.
It stands on the line's baseline as the text beside it does.
A drawn mark of a fixed size sits where the row puts it.
That is never quite where the eye reads the line.
Deportment sets its padding and its hover and checked grounds through `PLook` rows on `PCardSurface`.

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
