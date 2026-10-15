# Menu

The word-pick menu and the drag band of the reading view.
It stands apart from Text, which draws sentences, because these drive popups and selection over the finished page.

## `QSwath`

The band of text a reader drags across in the reading view, highlighted so it can be copied.
A text block selects nothing on its own, and one block's selection could never reach the next.
So the band is kept as two positions in `QSwathText`'s item list and drawn over the page.
A picture, video, glyph shape or tone contour is an item too, taken whole or not at all.
Copying takes only the text today, and a richer export will read the same band later.
It lies inside the scroll viewer, so its highlight scrolls and clips with the text.
It never takes a hit, so every click still lands on the text and buttons under it.
A press on a button, link, text box, star row, slider or scroll bar is left to that control.
A drag past the viewer's top or bottom scrolls it a step at a time, so the band grows off-screen.

### `QSwathRowSlack`

It is internal so `QSwathText` joins copied rows by the same measure the band is drawn by.

### `QSwath`

It takes focus though it takes no hit, so Copy and Select All reach it from the keyboard.
Copy is offered only while a band is drawn.

### `QSwathAttach`

The viewer's tunnelling mouse events are listened to, so a press anywhere on the page is seen first.

### `QSwathClear`

A press, a new entry and an emptied view each drop the band.
The item list in `QSwathText` is dropped with it, since the page it described is about to change.

### `OnRender`

The band is the system highlight colour, thinned so the text stays readable through it.
Any item in range other than a text block is covered whole.

### `QSwathBandScan`

A position's rectangle is a zero-width edge, so each is joined with the next position's back edge.
Edges on one line merge into one rectangle.

### `QSwathMoveRefine`

Nothing starts until the pointer has moved a drag's distance.
A plain click therefore still opens a word on a sentence and reaches every control.
Items are gathered at that moment, in visual order, which is reading order.
The viewer then captures the mouse, so the band follows the pointer past the window's edge.

### `QSwathControlCheck`

A link is a content element, so the logical parent is followed until a visual is reached.

### `QSwathTextCheck`

It answers true only for text under no control.
So the beam cursor shows over prose alone.

## `QSwathSeam`

One end of the band, holding a page item index and, for a text block, the position inside it.
An item that is not text carries no position.
It stays a value, so an end is copied rather than shared between the anchor, head and tail.

### `QSwathSeamStart`

The head's own block starts at its position, and any later block starts at its beginning.

### `QSwathSeamFinish`

The tail's own block ends at its position, and any earlier block ends at its end.

## `QSwathText`

The page items a band runs across, and the text the band covers.
It stands apart from `QSwath`, which keeps the drag and the drawing.
Every bound is measured against the owning swath, so the items and the highlight share one frame.
A tone contour is bounded by what it draws, not by its slot.
Its slot is the pronunciation's column and its box may hang past it, so the highlight covers the box.

### `QSwathTextScan`

Buttons are entered, since chip text is part of the page though a press on it stays a click.
A video is one item, so the controls inside it are not walked.
A video is known as a surface that `QScreen` drives, since Deportment never names the Veneer's screen type.
A collapsed section is skipped whole.

### `QSwathTextFind`

The item nearest the point wins, with vertical distance counting far more than horizontal.
A point above or left of a block maps to its start.
A point below or right of it maps to its end.
A block asked while its layout is stale refuses, and its start stands in.

### `QSwathTextRead`

The text is empty while no band is drawn.
Only text blocks contribute, so pictures and videos in the band add nothing yet.

### `QSwathTextFormat`

Blocks on one row read as one line, touching ones joined and spaced ones separated by a space.
A block on a lower row starts a new line.

## `QMentionAsk`

One open request for the sense menu, answered by `QMentionMenu`.
The area that opened the menu subscribes this ask alone, so it hears only its own pick.
So no area compares anchors to tell its pick from another's.
A new ask stands for every opening, and the menu forgets the old one on every hide.

### `QMentionAskChosen`

Zero as the sense id means the whole Entry.

## `QMentionMenu`

The menu that opens at a clicked word when the word could mean more than one thing.
It is one popup of the loaded window, because every panel that draws a sentence answers a click through it.
Entry mode lists the Entries an unlinked word may mean, and picking one opens it.
Sense mode lists the Meanings of one Entry, and picking one settles the `QMentionAsk` that opened it.
The menu never writes and never reads the engine.
It shows the rows it is given and reports the one picked.

### `QMentionMenu`

It sets itself as the mention attached property, so every mention block below inherits its host.
However the popup closes, the menu hides, so a closed menu keeps no rows for the keys.
The window builds it before its own wiring, so its preview key runs ahead of the chronicle's.
Deactivation and every navigation landing close it too.
A landing only closes the menu, since the tab's area opens the record itself.

### `QMentionPopup`

The popup is declared once in the window markup beside the panels.

### `QMentionOfferRefine`

Every panel that draws a sentence takes this one path, the reading display and the corpus excerpt alike.
The word's place is read off the control that drew it, from the offer's unit in the whole text.
That is a ready surface value, so the menu calls no gate here.
A null offer shows nothing, and an empty one only closes any open menu.

### `QMentionEntryRefine`

The menu is already hidden by the pick, and a landing hides it too.

### `QMentionMeaningRefine`

It answers a fresh `QMentionAsk`, and the caller subscribes that ask alone.
So only the asker hears its pick.
The card row and the corpus scribe call it directly, each with its own area's ready read.

### `QMentionMenuObserve`

The row is a plain surface rather than a button, so the list beneath it keeps its own selection.

### `QMentionMenuHide`

It forgets the rows, the lit row and the sense ask.
It is safe to call when the menu is already closed, which is how every navigation calls it.

### `QMentionMenuRefine`

It hides first, so entry mode always opens with no sense ask.
The popup hangs under the clicked word and the line it sits on.
So a wrapped sentence opens the menu under the word and not under the block's last line.
The header is bound to a localization key, so a language change renames it in place.
The first row starts selected, so Enter alone picks the likeliest answer.
An empty list opens nothing.

### `QMentionKeyObserve`

It is hooked into the window's preview key event, so the keys reach the menu wherever focus sits.
The rows are not focusable, and the menu takes no focus of its own.
A closed menu holds no rows, so the lantern gate answers null and the arrow passes through.
Escape closes the menu only while it stands open.
Every other key passes through, so a shortcut still works while the menu is open.

### `QMentionMenuSelect`

It captures the sense ask, then hides before it settles, so a subscriber sees a closed menu.
A subscriber that opens another panel does not then close a menu that has already moved on.
With no ask the pick is an Entry, opened through the navigation's gate.

### `_qMentionMeaningAsk`

Null means entry mode, and every hide clears it.

### `QMentionLeaveRefine`

A popup that stays open over another window would answer keys meant for that window.

## Stamp

Commit: `2f9a06b8eb95`
- `QMentionAsk.cs` `99ebaa29f9e3be85`
- `QMentionMenu.cs` `f861653f93f5f1b7`
- `QSwath.cs` `14d3c994f95140d1`
- `QSwathSeam.cs` `2df105d4cda9a5d4`
- `QSwathText.cs` `cb11efaee0d0b0bc`
