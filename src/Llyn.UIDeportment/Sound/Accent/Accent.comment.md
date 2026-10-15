# Accent

The pronunciation rows after the primary, and the tone contour box drawn under a tonal reading.
It stands apart from `Sound` because it holds a whole editor block and its own custom-drawn element.

## `QAccent`

The further pronunciation rows of the input panel, rendered from the draft the engine holds.
The primary pronunciation keeps its own field above them.
Each later row has its variety, its IPA and the same controls the primary wears.
The controls are play when the row has audio, lookup, download, and the plus and minus pair.
The rows are rendered as a diff on each draft bulletin, keyed by pronunciation id.
The primary field also wears a variety chip, drawn from the same draft row in the same pass.
A row edit goes to the accent gate with that row's id and shows the text it answers.
A plus or minus goes to the add or remove gate with that row's id.
A null id is Conduct's word for the primary, so no magic id is sent.
The remove gate is always called, and Conduct removes nothing on null.
The engine answers with a draft bulletin, and the render takes the row off the screen.
Lookup and download open the editor's one menu under the row's own button, not under the list.
The menu refine is subscribed before the observe, so the old search closes first.
Shutting the popup cancels the search, so the clip binding carries its refine and then its observe.
The five row commands are bound on the sound panel, as the markup had them.
So a row command reaches this editor.
Playback plays on the editor's player, handed in at build.
A null address plays nothing, since the engine cleared the gone file and the bulletin repaints the row.
A file the player refuses reaches the user through the editor sound's one failure subscription.
A changed variety or mark rebuilds the row, because its label, flag and brackets are fixed at creation.
The audio is always taken from the sheet, because the row never edits it.
Variety flags load in their own subscriber after the sheet, so each refine asks Conduct once.
A null flag answer paints nothing, since the load failed or another language took the editor.

## `QAccentCommand`

The routed commands a pronunciation row raises.
They are bound by the editor or the reading view that hosts the rows.
Each carries the row as its parameter, because the row template has no code of its own.
The commands are addition, removal, notation menu, clip menu and playback.

## `QAccentControl`

The hover handles of a row list, built for a row only once the pointer or the focus reaches it.
Every row used to carry its plus, minus and star buttons from the start, invisible until hovered.
Three buttons per row meant three control templates and three accessibility peers nobody asked for.
A row now carries an empty slot, and the handles load into it on first hover or focus.
The slot is a content control tagged with the template key, so the row template says which handles it grows.
A row never hovered draws nothing there, and nothing after the slot moves, since it ends the row.
Keyboard focus in a cell editor shows the same handles a hovered row does.
The row last served is remembered on the list, because a pointer raises moves by the dozen.
That property is attached only in code and needs no accessors.
A row already holding its handles, or whose slot names no template, is left as it is.
The slot's content is bound to its own data, so the handles follow the row when its data is replaced.

## `QContour`

The tone contour box drawn under an IPA reading of a tonal language.
It draws one cell per syllable, on one guide line per level of the scale it is handed.
Each cell carries the syllable's pitch line and the syllable as written beneath.
A level tone is a flat line and a contour tone bends at every level the marks spell.
A line between two levels fades from one colour to the other.
Each turning point pins its brush's colour at its horizontal position in a gradient in absolute coordinates.
Every level arrives with its theme brush, resolved by `QContourInk` from the role Core picked.
The box hides itself while the language is not tonal or the reading carries no tone.
It starts collapsed and never takes the pointer, so a press lands on what lies beneath.
Its brushes and font are theme references, so a theme change repaints it.
The cells are sized so a contour is read at a glance, never as a glyph.
So it draws its full width and never shrinks to fit.
It asks for its plot's height and no width.
Its width is the slot it is handed, and it draws the plot centred on that slot.
A plot wider than the slot hangs over both sides by the same amount.
So the screens hand it the column of the delimited pronunciation, and the plot centres under it.
The pronunciation keeps its place, and its buttons do not widen the slot.
It draws nothing until both syllables and scale are present.
The frame goes first and the guides next, so every pitch line sits on top.
The whole drawing is shifted by the plot's origin in the slot, so frame, guides and cells move as one.
The frame brush also rims every dot, so a dot stands off the line it sits on.
The scale is highest first, the first level is the top line and each next lies one gap below.

## `QContourInk`

Turns Conduct's ready contour into the box's own items, so the box names no Conduct type.
Each level's brush key, chosen in Conduct, is looked up from the scope.
A brush missing from the theme falls back to gray, as the box's own brushes do.

## `QContourItem`

One syllable of the box in Deportment's own shape.
It holds the written text, the pitch levels and a theme brush per level in the same order.
It also holds whether a pitch line is drawn.

## `QContourPlot`

The geometry of the box, kept apart from the inks and text it paints with.
It places guides, cells, pitch points and labels from the scale and the syllable count.
The box builds one per measure and per drawing, so a plot never outlives the values it read.
The cell width caps the syllable text, so a long syllable is trimmed inside its own cell.
The top padding is wider than the sides, because the dots and axis digits rise above the top line.
The bottom padding is narrower.
The label row is 20 pixels high and holds one line of 15-pixel text.
The size has room for at least one cell.
The origin is the plot's left edge in its slot, half the width the plot lacks or spares.
It is negative when the plot is wider than the slot.
It is rounded to a whole pixel, so the frame and guides keep their half-pixel snap.
A line's height follows how many levels lie above it, read from the scale's own order.
So the plot assumes no direction and no range, and a scale in another order draws as handed.
Turning points spread evenly across the cell, inset from both edges.
A single level yields two points at one height.
So a level tone is a flat line across the same span.

## Stamp

Commit: `2f9a06b8eb95`
- `QAccent.cs` `171a48d1f55c07b3`
- `QAccentCommand.cs` `a63197563f21dece`
- `QAccentControl.cs` `12df01bae289db01`
- `QContour.cs` `8ddceb0e2b0610c7`
- `QContourInk.cs` `89725f62a4ae583b`
- `QContourItem.cs` `819e805c0669a089`
- `QContourPlot.cs` `2e5337eba06fc119`
