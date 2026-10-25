# Display

This folder holds the reading view's coordinator, entry section drivers, label layout helper and grasp star renderer.
The coordinator wires the section drivers to Conduct notices, including drivers in neighboring folders.
Reading interactions still call Conduct gates, so presentation does not decide whether a change is accepted.

## `QLectern`

Subscription order is the redraw order, so the order of building matters.
The reflex section precedes the sound section, so a fanqie notice rewrites the anchors first.
The accent section draws its rows before it loads their flags, so late flags land on drawn rows.
The card section redraws before the compass, so contents measurements follow the section updates.
The compass hears the sought event itself, so the lectern wires no spotlight between the card section and the compass.
`QRoster` is built after the compass because contents clicks need the compass instance.
Fold notices refresh the card section, sound boxes and reflex hinge together.
The empty notice and the contents swap on the open and the close alike.
The swap follows whether the display holds the blank header.
So an open that leaves the blank header shows the empty notice.
Bulletin subscriptions use `QObserver`, keeping their delivery separate from the direct open and close callbacks.

## `QGraspStar`

The host supplies dependency properties and ready star tables, keeping rating rules outside the renderer.
A missing star image throws in the constructor, so a broken asset fails before the row draws.
A disabled row is dimmed rather than hidden, so the header keeps its shape.
Empty hit tables still reserve the renderer's slack border.

### `QGraspSizeDraw`

A slack border gives the row room beyond the star ink.

### `QGraspPressRefine`

A press always reports the pressed step to the host callback, even on the standing step.
The renderer never writes the step itself.
The meaning of a repeated press stays with the subscriber.

### `QGraspKeyRefine`

A step key reports a relative move to the host callback and marks the key handled.
Home and End report the whole limit as a move, so the renderer clamps nothing.
Other keys stay unhandled.

### `QGraspDraw`

A transparent rectangle is drawn first, so the pointer hits the row and not only the ink.
The star image is only a mask, so the themed brushes give each star its colour.
An unrated row fades further, so it reads apart from a low rating.
While the pointer hovers, the preview brush replaces the fill brush.

### `QGraspStepDraw`

Gaps and slack fold into the neighboring hit regions, so the renderer leaves no gap between stars.
The supplied hit table chooses the step for each half.

## `QLecternCard`

The card driver consumes only the card, route and sound areas, keeping engine records outside presentation.
Word offers leave through a notice, so the driver needs no window or contents reference.
Bubbling click and mention handlers cover cards generated inside templates.
The word event of a sentence starts the keyboard word cursor, and the lectern needs no window for it.

### `QLecternWordStart`

The event starts the route's word cursor on the sentence and listens for keys on that line.
The cursor ends when the line loses keyboard focus or Escape is pressed.
The event is marked handled, so no outer element starts a second cursor.

### `QLecternWordObserve`

Enter asks the route for the mention at the cursor and raises the mention notice.
The notice carries the unit of the word's start, so the menu stands at that word.
Left and Right move by word, and with Control they jump to the next stop.
After each move the line redraws its lit word from the route, which holds the cursor.
Any other key is left alone.
A click raises the same notice with no unit, so the menu stands at the click.

### `QLecternLeafRefine`

Clearing each item source before replacement forces templates to redraw even when the length is unchanged.

### `QLecternChipObserve`

The route gate decides whether the click is handled.

### `QLecternHingeObserve`

The item says whether it is a collocation row.
The gate is given that flag with the id.
So the engine looks the card up by kind and id.
The gate verdict goes to `QLook.QLookCheckedRefine`, which puts a refused hinge back.

### `QLecternSeeObserve`

The line always names a collocation, so the gate opens it as one.
The gate unfolds the row and itself announces the card as sought.
So the click raises no notice of its own.
A refused unfold answers false and leaves the click unhandled, so no spotlight is requested.

## `QLecternEntry`

Entry fields redraw on open and close, while the note redraws only on open.
The note section's visibility follows the shown entry, keeping its content refresh separate from hiding it.
An unchosen unit or freedom hides its chip, so a bare entry shows no empty chip.
The note is Markdown, drawn as blocks by `QMarkdownFace`.

## `QLecternFrequency`

Open, close and frequency bulletins share one redraw path, keeping the chip consistent across those updates.

## `QLecternGrasp`

The star driver receives its ready tables from the grasp area, keeping rating decisions below presentation.
A press reports the pressed step and a key reports a relative move, so the area decides each outcome.
The returned grasp replaces the step and label, so the gate's answer remains authoritative.
A hover with no step reads the standing grasp label, so the label returns when the pointer leaves.

## `QLecternHeader`

Text, font, flag and favorite state have separate redraw callbacks, keeping their updates independent.
A favorite click redraws from the gate's returned state rather than trusting the toggle.
The gate flips the stored mark itself, so the click sends no checked state.
The header takes the favourite gate and its change event from the display's grasp area, not from the display.

### `QLecternFlagRefine`

The flag is drawn for the language the area shows after loading, not a language captured before the await.
The initial empty-language draw clears the previous flag before loading starts.

## `QLecternEtymology`

The etymology section draws the map of boxes the shown entry descends from.
It raises no notice, and nothing else reaches for it, so `QLectern` keeps no handle to it.
The lectern builds it and wires its repaint to the open and close callbacks.
A repaint reads one state from the card area and shows the section only when a box exists.
The headword stands at the foot of the map, taken from the state.
The tree is built by the editor's `QEtymologyItem`, so both maps nest and match boxes alike.
A box hides its tongue and its gloss while they are empty.
A box with an entry link draws its word as a hyperlink in the accent colour.
A click on it opens that entry through the route, so the map is a way to travel.
A borrowed box draws a dashed trace in place of its frame, and a linked box adds a mark.
The relation word is the text of the choice the box's relation id selects.
It hides when no choice matches.
Each trunk rule is bound to `HasItems` of the sources list above it, so a box without sources draws none.
A box hides its left half-bar when it is the first of its siblings.
It hides its right half-bar when it is the last.
So the half-bars of a sibling row join into one fork.
The reading map shows no tools, since only the editor changes boxes.
A wheel turn over the map is raised again on the section.
So the page scrolls and the map does not.

## `QLecternIncoming`

One handler on the list hears every row, since the click bubbles from the row button.
The row is read from the click's original source.
The section collapses when its usage list is empty.

## `QLecternLead`

The lead couples a section's shown state with its label column.
The label column joins the shared `PReadingLabel` width only while its section shows.

## Stamp

Commit: `36e31eb71c98`
- `QGraspStar.cs` `b96249bbb783b9db`
- `QLectern.cs` `722bac2f594d8991`
- `QLecternCard.cs` `8d3780431c60b38b`
- `QLecternEntry.cs` `f13d612b00f01fd0`
- `QLecternEtymology.cs` `4eac208cc78d309e`
- `QLecternFrequency.cs` `557735b484228775`
- `QLecternGrasp.cs` `b3a3e33c2b56fe4c`
- `QLecternHeader.cs` `099a9cf062743bd5`
- `QLecternIncoming.cs` `2013dfbcd1cfc565`
- `QLecternLead.cs` `f744babc1ea40def`
