# Image

Image holds the console row of one picture on a card.
It stands apart from the card's inlets, because a picture row has a menu and a file browse.
Its Video twin shares its caption and its indent.

## `BImage`

The driver and the drawing of one picture row in the editor.
The card hands it a spot, which names the lit picture row or answers null.
Every key is live only while the spot answers, so a card with no lit picture row takes none.
The spot carries the collocation flag, the card id and the row.
So the gate addresses the row by kind, card and id.
Typing, the open menu and the calm row are three states.
Each keymap row is live in one of them.

### `BImageKeymap`

Enter on a calm row opens the Browse and Remove menu.
Insert adds a picture and Delete removes the lit one.
Ctrl+O browses at once, without the menu.
A typing field closes on Enter, Escape, Up or Down.
An open menu takes Up and Down to move, Enter to pick and Escape to shut.

### `BImageObserve`

A key goes to the menu first, which seeks a row by its typed letter and ignores control keys.
Otherwise a printable key on a calm row opens the location field and is typed into it.
Then the field, if open, takes the key.

### `BImageFieldOpen`

The field opens on the row's typed wording, and each edit goes to the gate as raw text.
The location is deferred like every typed field.

### `BImageSettle`

It closes the field and tells the host to keep the draft.
With no field open it only shuts the menu.

### `BImageMenuSelect`

The menu holds Browse first and Remove second.
The menu shuts before the pick runs.

### `BImageFileOpen`

It settles the field first, then asks the gate to browse.
The gate asks the desk's envoy, so the console and the window share one path.
A cancelled dialog sends nothing.

### `BImageDraw`

It answers the row's painter and the cell where the menu hangs.
The caption leads the row, then the location or the open field.
The menu hangs only under a lit row, and the caption width keeps picture and video rows aligned.

### `BImageLocationRead`

An unset location shows its hint in the muted role.
A location the engine cannot reach shows its text followed by the Display.Missing word, muted.
A long location is shortened with an ellipsis so the Display.Missing word still fits after it.
The word is dropped when even the file name would not fit beside it.
The text is then shown whole if it fits the cell, and shortened otherwise.
The role stays muted in every case.
Nothing is read from disk, because the draft states whether the address is reachable.

### `BImageCaptionRead`

The caption is padded to the widest of the two captions, in cells.
So a picture row and a video row start their value in one column.

### `BImageDropDraw`

It draws the menu below the anchor and nothing when no menu is open or no row is lit.

### `BImageClose`

It applies a pending edit, then resets.
The card calls it when it closes.

### `BImageReset`

It drops the field, the menu and the field's row id, and applies nothing.

## Stamp

Commit: `9377e657b395`
- `BImage.cs` `bbaff3796087106e`
