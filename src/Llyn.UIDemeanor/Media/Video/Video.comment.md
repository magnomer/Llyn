# Video

Video holds the console row of one video on a card.
It stands apart from Image, because a video row has two fields and one lit slot.
It reuses Image's caption, indent and gap.

## `BVideo`

The driver and the drawing of one video row in the editor.
The card hands it a spot, which names the lit video row or answers null.
Every key is live only while the spot answers, so a card with no lit video row takes none.
The spot carries the collocation flag, the card id and the row.
So the gate addresses the row by kind, card and id.
Typing, the open menu and the calm row are three states.
Each keymap row is live in one of them.

### `BVideoKeymap`

Enter on a calm row opens the Browse and Remove menu.
Insert adds a video and Delete removes the lit one.
Ctrl+O browses at once, without the menu.
Left and Right light the location or the span, and each is live only from the other slot.
A typing field closes on Enter, Escape, Up or Down.
An open menu takes Up and Down to move, Enter to pick and Escape to shut.

### `BVideoObserve`

A key goes to the menu first, which seeks a row by its typed letter and ignores control keys.
Otherwise a printable key on a calm row opens the field of the lit slot and is typed into it.
Then the field, if open, takes the key.

### `BVideoFieldOpen`

The field opens on the lit slot's typed text, and each edit goes to the gate as raw text.
The slot is remembered, so the field is drawn in its own cell.
The location and the span are deferred like every typed field.

### `BVideoSettle`

It closes the field and tells the host to keep the draft.
With no field open it only shuts the menu.

### `BVideoFileOpen`

It settles the field first, then asks the gate to browse.
The gate asks the desk's envoy, so the console and the window share one path.
A cancelled dialog sends nothing.
Browsing sets the location whichever slot is lit.

### `BVideoDraw`

The row shows the caption, then the location cell, then the span cell.
The span cell is as wide as the span text or its hint, whichever is wider.
The location takes the rest, and a cell that does not fit is dropped.
Text too long for its cell is cut with an ellipsis.
Only the lit slot takes the lit tone.

### `BVideoValueRead`

An empty value shows its hint in the muted role.
A location the engine cannot reach shows its text followed by the Display.Missing word, muted.
A long location is shortened with an ellipsis so the Display.Missing word still fits after it.
The word is dropped when even the file name would not fit beside it.
The text is then shown whole if it fits the cell, and shortened otherwise.
The role stays muted in every case.

### `BVideoDropDraw`

It draws the menu below the anchor and nothing when no menu is open or no row is lit.

### `BVideoClose`

It applies a pending edit, then drops the field, the menu and the field's row id.
The lit slot stays.

### `BVideoReset`

It drops the field and the menu without applying, and lights the location again.

## Stamp

Commit: `9377e657b395`
- `BVideo.cs` `dea606065fcd5639`
