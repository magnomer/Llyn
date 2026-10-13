# Field

The text-input machinery of the deportment: field templates, the caret, chip entries and scrollbars.
It stands apart from the panels because every editor shares these helpers and framework contract names.
Framework part names such as `PART_ContentHost` live here in code, never in the audited XAML surface.

## `QBerth`

The wrap panel of a chip field, which seats the typing entry at the caret among the chips.
The chips are the field's items, and the entry is not one of them.
So moving the caret never touches the order of the rows the engine shows.
Children flow left to right and wrap onto a new line when the width runs out.
`QBerthAnchorProperty` is the chip the entry stands before.
`QBerthEntryProperty` is the entry element the panel seats.
Both are set on the field and bound by its own items panel alone.
They are not inherited, so a chip field nested inside a chip never takes over the outer one.
No anchor, or an anchor no item carries, seats the entry after the last chip.
`QBerthBuild` builds the field's entry once, as a one-item list showing the caret through the field's own selector.
It binds the field's anchor to the caret's anchor property, so a caret step reseats the entry.
It answers the entry, so the driver fills it as it fills the chips.
The visual and logical children are the chips plus the entry once seated, so resources and inherited values reach it.
Visual children come in layout order, so focus moves through them as they are seen.
Measure binds to the field once, seats a waiting entry, then measures line by line.
Arrange places the children line by line, each line as tall as its tallest child.
`QBerthEntryRefine` releases the seated entry and keeps the new one waiting.
An items panel must start with no child of its own.
So the entry is seated only at the next measure.
`QBerthOwnerAttach` binds to the field that owns the panel as its items panel, once.
A panel not yet hosted by a field waits for a later measure.
`QBerthEntryAttach` first releases an entry still held by a panel the field built earlier.

## `QCaret`

The text cursor every input field is typed against.
WPF draws its own, but only its color can be set.
Its width comes from a Windows-wide setting and its blink is a hard on-off.
So the framework caret is made transparent and this one is drawn over the field instead.
It is at least two pixels wide with rounded ends, painted in the theme accent.
It fades rather than snaps.
It also holds the caret rules every chip entry shares, kept where a test can reach them without a window.
`QCaretHook` is one class handler serving every text box, including those inside control templates.
A field is adorned the first time it takes keyboard focus, so a field never opened costs nothing.
`QCaretEdgeApply` decides the erase keys every chip caret answers the same way.
The key arrives as its WPF name, such as `Back` or `Delete`.
A selection leaves every key to the text box, so ordinary editing is untouched.
Backspace at the start of the entry hands the step before it to `remove`.
Delete at the end of the entry hands the step after it to `remove`.
Each chip entry's erase Observe calls it and ends in its remove gate.
`QCaretStepApply` takes the arrow keys alone.
An empty entry is walked past a chip, and `place` puts the caret back.
So a chip is crossed the way a character is, in either direction.
Each chip entry's caret Refine calls it after the erase Observe, which ignores the arrows.
The answer says whether the key was taken.

## `QEnvelope`

Builds the reusable text-input control templates in code.
The content host must be named `PART_ContentHost`, because WPF's TextBox looks that exact part up.
The name is a string literal here, so the contract is honored without entering the audited XAML surface.
The placeholder inset, its fade, the hint and the surface name are read from `QField`.
`QEnvelopeIntroduce` registers the three templates under the keys their styles bind a Template setter to.
The framed one goes under `Theme.Input.Field.Template`.
The bare template is the field a card uses while being edited, under `Theme.Input.Field.Bare`.
A card must read the same in both modes, so the text sits where the reading side draws it.
Padding would move it, so the frame is a sibling drawn behind with a negative margin instead.
The frame therefore grows outward on hover and focus and never shifts the text by a pixel.
How far outward is measured by `QFieldConverter`, since a card writes its fields at several faces.
A fixed measure framed each face at its own height, and the card showed a different box on every line.
The plain template is the field a search bar writes into, under `Theme.Input.Field.Plain`.
A search bar already draws its own frame around the dropper, the divider and the field together.
The bare template would draw a second frame inside it on hover and focus, read as two nested controls.
So the plain template carries the placeholder and the text host alone.

## `QField`

The shared helpers every text field leans on.
It owns the hint property, the placeholder dress, the popup place and the caret walk.
The field templates live in `QEnvelope`, and the editor a row cell grows lives in `QQuill`.
`QFieldHintProperty` is the placeholder text a field shows while empty.
The deportment sets it by resource key, so it follows a language switch.
The templates and the bare choice read it, so no hint travels through `Tag`.
`QFieldGhostAttach` binds a measuring ghost to its field through `QFieldGhost`.
The ghost follows every text and hint change by itself.
`QFieldPopupPlace` places a popup under its field, or above it when the screen ends.
It overlaps the shade the popup frame paints.
`QFieldSurfaceFind` finds the surface border of a field's template, or the field itself without one.
Anything but a field answers null, so a handler may pass an event source through unchecked.
`QFieldSurfaceName` is shared because the slate, proffer and drawer dropdowns are placed against the frame.
`QFieldTextShow` writes a fact into a box, and an unchanged text is no change to the property.
So WPF leaves the caret of a box being typed into alone.
The draft echoes each deferred request back, so that write must stay a no-op.
`QFieldFocusDefer` puts the caret at the end of the field inside a row once its container exists.
The container is generated after layout, so the focus waits for the loaded pass.
`QFieldPlaceholderInset` is the room a placeholder is held off the caret by.
It is the same in a bare field as in a framed one.
A placeholder starting where the text starts is written under the caret.
A writer then reads the caret as part of the word.
Every `QEnvelope` template holds its placeholder off by this much, so a card field sits as the headword does.
`QFieldPlaceholderShow` dresses a reading-side text block as the placeholder its edit-side field would show.
The inset, the muted colour and the fade are taken from here rather than guessed there.
Then the edit side and the reading side put the same word in the same place.
`QFieldCaretApply` is the shared way the chip fields reach the entry they are typed into.
The Context, Label, Link and Register fields are each a run of chips with one open entry among them.
No entry can be bound by name, because it is one item of a templated collection.
Moving the entry rebuilds its item, so the focused box is gone by the time the move lands.
The caret is put back on the newly drawn entry once the field is laid out again.
A caret past the end of the text lands at the end, since the text box clamps it itself.
Without it a step across a chip would drop the user out of the field.
`QFieldHostFind` finds the field the moved entry will be redrawn in, from the box being left behind.
`QFieldCaretFind` takes the first text box under the field as the entry.
A committed chip carries a button and a label, never a field.

## `QFieldConverter`

Measures the frame a written field is drawn in so every field carries the same frame.
A card writes its fields at the sizes and faces the reading view draws them in.
A frame hugging each line would stand at a different height on every field.
So the frame is measured outward from the line instead, to one height for the whole card.
The frame is a sibling drawn behind the text with a negative margin, never padding.
A field therefore keeps the text where the reading view puts it whatever the frame does.
The bound values arrive as the face, the size and the padding, in that order.
Until all three are settled it answers no margin, so a half-built field draws no frame rather than failing.

## `QFieldGhost`

The text a measuring ghost shows, which is the field's own text, else its hint.
A ghost sizes its field, so an empty field still takes the width of its hint.
The choice sits in a converter, so the driver never branches on what the user typed.
The values hold the text first and the hint second, and a missing hint reads as empty.
A ghost only shows, so nothing is ever written back.

## `QGauge`

The two-child panel an edit field stands in, so the field lands where the read view puts its text.
It is public because the editor markup in the Veneer places it.
The first child is the mark, an unseen text block holding the same words in the read view's own style.
Both children are measured, but only the mark's size is reported.
So the cell closes on the plain text width rather than on the field's own.
The second child is the field, arranged at its own width inside the mark's slot.
Room is left for its caret.
The panel is stretched to the row and both children are centered in it.
The read view centers its text block the same way.
A field wrapped in a centered box is rounded twice against a fractional text height, and lands a pixel off.
One stretched slot with one centered child is rounded once, on both sides alike.
The mark is given the whole slot, so its own alignment places it.
The field is given its own width instead of the slot's.
A caret at the end of the text is then not cut off.

## `QIndicator`

Builds the scrollbar templates in code.
WPF requires the track to be named `PART_Track`, kept here for the same reason as `PART_ContentHost`.
It prevents framework names from entering the audited XAML naming surface.
The rail is one design, laid along whichever axis it was asked for.
It is ten pixels of track with a thumb of the same ten pixels.
A thumb narrower than its track reads as a hairline lost inside an empty groove.
The four pixels of inset are the gap between the reading surface and the rail.
They belong to the rail rather than the page, so every scroll viewer keeps the same gap.
The gutter template holds a fourteen pixel lane beside the content, never over it.
Nothing the page draws is ever covered by the rail.
The lane is zero until the rail is actually shown, and only on the axis showing it.
An empty lane beside a short popup would read as a margin nobody asked for.
The computed visibility opens the lane, so a disabled or unneeded axis stays closed.
Each rail is given the lane size and the matching rail template here rather than by a style.
A style written beside the scroll viewer never reaches a rail the template makes, which would keep its default thickness.
WPF requires the parts `PART_ScrollContentPresenter`, `PART_VerticalScrollBar` and `PART_HorizontalScrollBar`.

## `QQuill`

Grows the editor of a row cell only while it is being edited.
A row list once drew one text box per cell.
A Han character with twenty-six readings therefore built over a hundred editors.
Each editor is a control template, a text editor with its undo stack, and an accessibility peer that reports itself.
Nobody edits more than one cell at a time.
So the rows draw the same text blocks the reading view draws.
A click on a cell puts one text box over that block, and the box leaves when focus does.
The editor and the reading view therefore share the cell's styles, and the two modes match to the pixel.
A cell declares its editor through the attached `QQuillCellProperty`.
`QQuillCell` is the cell's editor order, holding the box style key, the bound property and an optional placeholder key.
The row fill is the one place it is written.
`QQuillIntroduce` watches the list for presses, so every cell beneath it grows its editor on demand.
`QQuillPressRefine` finds the ordered cell under the press and puts an editor into it, unless one already stands there.
The press then goes no further, since the box is given focus and the caret at the pressed glyph.
A box that cannot take focus is taken out again at once, so no editor is left standing unfocused.
A cell without a text block, a known box style or a resolvable style key is left alone.
The box takes the text block's tooltip, so a reflex language still shows its region while edited.
`QQuillTrailDraw` measures whether the press landed past the middle of the glyph, so the caret goes after it.
`QQuillOpenIntroduce` builds the editor with a two-way binding that updates on every keystroke.
The row is bound as an explicit source, so the text is settled before the box joins the tree.
A text change raised while joining would otherwise reach the editor as an edit the user never made.
The placeholder resource becomes its hint through `QFieldHintProperty`.
The editor listens for its own loss of keyboard focus, which is when it leaves.
`QQuillCloseTeardown` unwires the editor and takes it out of its cell.
The box leaves the tree before its binding is cleared, so the text change that clearing raises reaches nobody.
`QQuillBlockRefine` clears the text block's visibility rather than setting it.
So a style that hides an empty block keeps ruling it.
`QQuillCellFind` finds the nearest ordered grid above the pressed element, or null outside every cell.
The walk stops at the host, so an order on the host's own ancestors is never taken for a cell.
A text run is not a visual, so the walk climbs the logical tree until it reaches one.

## `QVeilPeer`

The accessibility peer a text-heavy list surface hands the framework instead of the default one.
The framework otherwise makes a peer for every text block in the surface and reports each to any subscriber.
A subscriber is any process that asked the system for property changes, and several common utilities do.
Each report is one cross-process call, so a surface of four hundred text blocks costs half a second per entry.
This peer reports only the controls a person can act on, the fields and buttons.
A screen reader therefore still reaches every field and button, and the text rows are read from the window's text.
The text input service talks to a field directly and never through this peer, so typing and the IME stand.
The surface reads as a plain group.
The class name is the owner's type name, so an inspector still tells surfaces apart.
Children are the peers of the focusable controls beneath the surface, found by walking the visual tree.
Nothing below a focusable control is walked, since that control's own peer answers for its parts.
A decorator owner is walked from its child, so a framed list speaks through this peer and not its own.

## Inline notes

### QCaret.cs: `field.CaretBrush = Brushes.Transparent;`

The framework caret is hidden, not removed.
The real caret still holds the position the IME composes at.
Japanese, Mandarin and Cantonese input would otherwise place their candidate window wrongly.

### QCaret.cs: `AdornerLayer? layer = AdornerLayer.GetAdornerLayer(field);`

A field outside any adorner layer keeps the framework caret, because nothing was made transparent yet.

### QCaret.cs: `private void QCaretLayoutRefine(object? sender, EventArgs e)`

Scrolling, resizing and font changes all move the caret without changing the text.
Layout is the one signal that catches every such move.
It fires often, so the position is compared first and nothing is redrawn when it stands still.

### QCaret.cs: `private void QCaretOffsetApply(double offset)`

A caret that jumps is hard to follow, so a move along one line is slid instead.
The offset starts at where the caret was and eases to zero, which is where it now belongs.
A move to another line, or a long jump, is not slid.
The eye loses a caret that travels too far.

### QCaret.cs: `if (typed || moved || !_qCaretBlink)`

Typing holds the caret solid, because a caret that blinks mid-word is read as a stutter.
Each keystroke restarts the cycle, so the blink resumes only once typing stops.

### QCaret.cs: `Brush face = _qCaretField.TryFindResource("Theme.Accent") as Brush ?? Brushes.Black;`

The accent is read at each draw, so a theme change is picked up without rebuilding the caret.

### QEnvelope.cs: `var pContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");`

No template binds a Margin to the host, since WPF already offsets the editable text internally.
Binding the host's Margin to Padding too would apply it twice.
The caret would be pushed right and down while the placeholder, padded once, stayed put.

### QEnvelope.cs: `pSurface.SetValue(UIElement.SnapsToDevicePixelsProperty, true);`

The measured margin is seldom a whole pixel, since a face's line spacing is not.
A hairline drawn at a half pixel is smeared over two rows and reads as a faint grey.
Snapping the frame lands each edge on one row so all four sides carry the same colour.

### QEnvelope.cs: `pContent.SetValue(FrameworkElement.MarginProperty, new Thickness(-2, 0, 0, 0));`

WPF insets an editable text host two pixels from the left even when the padding is zero.
A reading view draws the same words with a TextBlock, which has no such inset.
Pulling the host back by those two pixels lands the written word on the read word exactly.

### QFieldConverter.cs: `private const double QFieldConverterHeight = 31;`

One height for every written field, and the height the card's own buttons stand at.

### QFieldConverter.cs: `var pLine = pFamily.LineSpacing * pSize;`

A face is asked for its own line spacing rather than assumed, so a change of face still measures true.

### QFieldConverter.cs: `var pVertical = Math.Max(0, pRoom);`

A field whose line already fills the frame is left hugged rather than pushed inward.
A frame is never drawn inside the text it frames.

## Stamp

Commit: `c2f335c331fe`
- `QBerth.cs` `296b6bc885b1b87e`
- `QCaret.cs` `9944fb03dcc1c00e`
- `QEnvelope.cs` `3fa4b64ee223d726`
- `QField.cs` `a821f672842cf9dc`
- `QFieldConverter.cs` `492cd567c2fe7f08`
- `QFieldGhost.cs` `77453066f2eefc7f`
- `QGauge.cs` `8314a21d3efc20db`
- `QIndicator.cs` `e233e4df0680962e`
- `QQuill.cs` `f75f8cd176f99399`
- `QVeilPeer.cs` `8a1908a13cbe1c41`
