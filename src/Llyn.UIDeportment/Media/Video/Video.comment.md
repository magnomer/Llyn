# Video

Video rows for the editor's cards and the repertoire's situations, and the reading view's film frames.
It stands apart from Image because a video is never kept or copied, and its preview plays on a screen.

## `PMedia`

The reveal of a media list's row handles, shared by the editor and the reading view.
Attaching twice is harmless, since each handler is removed before it is added.
Hiding clears the values, so the theme's hidden base shows again.

## `PVideoFrame`

The element that unwraps a reading view's film line into the row the Screen plays.
A line arriving as the element's data is unwrapped here, and any other data is left alone.
A line Conduct calls empty collapses the element.
Otherwise the film row becomes the element's data, so the Screen binds as it does in the editor.
It is public because the Veneer's display markup names it.

## `QVideo`

The driver for Video rows in a card and in a situation.
The rows sit in a template, so the card list hands their buttons here rather than to the card.
Each click hands one raw value to one video gate, and the engine finds the card a row belongs to.
Each owner builds its own driver and hands it its own gate.

### `QVideoRowIntroduce`

The rows are drawn through `QVideoApply`, which hooks the location and span observers only after the fill.

### `QVideoRowRefine`

A new row holds the engine's location and span, and nothing typed.

### `QVideoFreshObserve`

A Situation has no card, so the add names a null card, and the draft routes it.

### `QVideoOpenObserve`

The chosen path goes raw to the file gate, which sends it as the row's location at once.
A cancelled dialog hands null, and the gate does nothing.
The field takes a web address as well.
Nothing is copied into the workspace, because a Video is never kept.

### `QVideoApply`

The location and span boxes are hooked after the fill.
So a write from the fill echoes only where the handlers were already hooked, as in the editor.

### `QVideoLocationObserve`

The location box is hooked to this observer alone, so no box name decides the gate.
Only a box with the keyboard in it reports, since a write from the draft echoes through the same event.

## `QVideoItem`

One Video row on a card.
It carries where the video plays from, the span worth watching, and the address the screen plays.
The location and the span are kept with the card.
The address is not kept, and arrives ready on the draft.

An empty location is not one thing.
It may never have been written, or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds the engine's values and nothing typed.

The timestamp is written the way it is read, as `00:30 - 04:20`.
The draft carries its moments ready, so the row parses nothing.

### `QVideoItemShow`

The new draft is taken whole, so the id is always taken.
Only the fields whose value differs from the held draft are announced.

### `QVideoItemPreview`

A null screen hides the frame, so no empty player stands on the card.

### `QVideoItemUntil`

Null when the timestamp named no end and the video plays out.

### `QVideoItemPlaying`

It is the row's own state rather than the player's.
So it survives the preview being taken down and put back as the card scrolls.
A row begins stopped, in both modes.
Opening a card is not asking to hear it, and several videos would otherwise all speak at once.

### `QVideoItemRefine`

The shared fill of a written video row, used by the entry and situation editors alike.
Each field's text is rewritten only on a full fill or when its own property changed.
So a change to the other property leaves a field being typed in alone.

### `QVideoItemAttach`

The values are attached properties of `QScreen`, whose driver is built first.
The film id goes first, because a changed address reloads the screen at once.
The volume follows the shared catalog live, so every screen answers the one slider.
The playing flag is bound both ways, since the screen's own switch changes it.

## Stamp

Commit: `c2f335c331fe`
- `PMedia.cs` `5fa76a2c6ba83acb`
- `PVideoFrame.cs` `669bc595be841abb`
- `QVideo.cs` `587915ceeca724ef`
- `QVideoItem.cs` `21134000039ae78c`
