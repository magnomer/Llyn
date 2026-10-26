# Core

The console's editor frame, which draws and routes keys for one `CEditor`.
It holds no entry rule, so every verdict comes from Conduct.
The Library and Favorite tabs use it in their scribe mode, and the Input tab uses it alone.

## `BEditor`

The frame is one list of rows lit one at a time.
The list is Conduct's `CEditorWalk` for the section in front.
Both faces read that walk, so the console lights the rows the window would show, in the same order.
Three head rows sit between the headword and the section tabs.
They are the language pill, the esteem strip and the part-of-speech row.
They belong to the entry, not to a section, so they stay while the section changes.
The walk holds none of them.
So the lit place counts three rows more than the walk, and `BEditorHeadSpan` is that offset.
The frame keeps only the lit place, the section in front, the open field and the scroll top.
The form has four sections, which are meanings, collocations, etymology and note.
Only the first two hold cards in the walk, so the other two show their tab line alone.
The tab line counts the cards of the first two sections, and brackets the section in front.
Keys are live only while the frame's owner has the focus and a draft is held.
So a hidden frame binds nothing and its keys fall through to the window.
The owner passes the shown and focused tests, and the frame never decides them.

Up and Down, the page keys and Control with Home or End move the lit row.
Control with the page keys jump to the previous or next card header.
They skip the head rows and the tabs.
A key with no card to reach is hidden, so the legend only offers a move that works.
Left and Right pick the section only while the tab line is lit.
Alt with a digit picks the section from anywhere.
Enter folds the lit card header through the walk's own fold gate.
A fresh card has nothing to fold and carries no gate, so Enter is not offered there.

An unfolded card has four chip fields under its header, which are Situation, Register, Tag and Translation.
`BCard` holds them, and the frame passes it the lit step and says whether a field may open.
The lit place is then a row and a part of that row, with part 0 the header.
Up and Down walk the parts of the lit card first.
They cross to the next row only past the last part.
Crossing upward lands on the last part of the card above, so the light retraces its steps.
The page keys, Control with Home or End, the card jumps and a section pick all land on a header.
Enter folds only while the header is the lit part.
Moving the light, the part or the section closes the open chip field and the marker's.
So no field outlives its light.
A printable key opens the lit chip field and types the key, as the headword does.
With no headword field open, a key reaches the marker, the cards and the language pill in that order.

The headword opens as a field on Enter, or on the first printable key while its row is lit.
So the user can type at once without a separate open step.
While the field is open, Up, Down, Enter and Escape close it.
The keys the field reads go to it.
Closing persists the draft, so a field edit is kept as one step.
Up and Down also move the lit row by one after the close.
Opening again starts from the draft's current headword, never from stale text.

F2 saves and is dim while the draft cannot be stored.
F7 undoes to the stored entry, and only an editor that owns its entries offers it.
F6 marks the entry as a favorite, and it has two faces.
While the frame edits, F6 goes to `BRegard`, and it is dim for a fresh draft or a halted tenure.
While the reading view shows, F6 goes to the lectern instead.
That one is built only when the owner passes a lectern, a reading test and a settle callback.
The settle callback lets the owner keep its list row steady while the mark moves the entry.
These rows are bound at the window focus, since they are not about the lit row.

The language pill opens a list of the loaded languages on Enter and picks the lit one on Enter again.
The list is loaded on demand through a loader the owner passes.
The languages are the workspace's and may change.
With no loader the list is empty.
Leaving the pill row, a new tenure or a close ends a load still running.
So a late answer never opens a list the user has left.
The esteem row takes Left and Right to move the grasp and the digits to set it.
Those keys are dim while no entry is stored, because Conduct keeps grasp and favorite only for a stored entry.
The part-of-speech row's keys come from `BMarker`, and the frame passes it the test that the row is lit.
Moving the light or choosing a section closes the marker's open list or field.
An open list takes the keys first.
So Up, Down, Enter and Escape reach the list and not the row beneath.
A printable key goes to an open list's letter seek.
It opens the speech field instead when that row is lit and its spot is the field.

Ctrl+Z undoes a step, and Ctrl+Y or Ctrl+Shift+Z redoes one.
They sit at the editor focus and show dim when nothing is left to step that way.
A halted tenure makes every key at the editor focus dim.
It also closes the open field, so a dead edit area takes no text.
The halt is the one Conduct stored at the last bulletin, so a key's state cannot flip between draws.
The frame also redraws on the walk's change, since a caret move rebuilds the walk without a desk event.

The draw is `BEditorSight`'s.
The frame hands it the lit row and part, the open field and the walk.
It also hands it the three head parts and the card's chip fields.
Typing on the headword row opens the field only while the tenure runs.

A new tenure resets the frame to the first row and closes any field.
A draft that arrives moves the lit row back into range.
The walk may now be shorter.

## `BEditorChronicle`

The frame's undo and redo, over the desk's `CDeskChronicle`.
An open field holds typed text that is not yet a step.
So each undo or redo first applies that text, and the typed text counts as one step.
Then the step runs, and the field is reopened when the entry announces the new draft.
So the field shows the headword the step restored, never the text typed before it.
A new tenure forgets that pending reopen.
The state read is the chronicle's kept flags, so the legend and the status never ask the tenure.

## `BEditorSight`

The frame's draw, which lays out the form from the walk each time.
It keeps only the scroll top and the page height.
The headword row shows the placeholder in the muted role when the headword is empty.
Three lines follow it for the language pill, the esteem strip and the part-of-speech row, and a rule follows those.
Each head part draws its own line, and the frame only tells it the tone.
The tone is muted while halted, the lit role on the lit row, and nothing otherwise.
A tone paints the whole line in one role, and no tone lets each piece keep its own.
The pieces are cut to the line's width in order, so a narrow side loses the last piece first.
That drawing is `BEditorSightDraw`, which the head parts share.
A list a head part opens is drawn last, below its anchor line.
So it lies over the rows under it.
A language list hangs from the pill, and the marker's lists hang from the marker row.
A card header is a rule, its position, its caption and a fold glyph.
The chip fields of an unfolded card are drawn under it, one `BInlet` after another.
The field of the lit part takes the lit tone as one bar, and a halted tenure mutes them.
A field's list is drawn last, as the head parts' lists are, below the line that holds its entry.
The scroll top keeps the lit part on screen.
It keeps the rest of its card too, as far as a page allows.
The hint line shows the lit field's hint while that field is bare.
The glyph is drawn only for a card that carries a fold gate.
It is cut by cells to fit the width.
A muted caption means the card has no title yet, so the section's word stands in.
The lit row is padded to the full width, so the highlight is a bar.
It wears the other-focus role when the frame lacks the focus.
The caret is a zero-width cell at the end of the headword.
It shows while that row is lit and the frame has the focus.
A halted tenure draws every row muted and lights none, which matches the dead edit area.
The scroll top follows the lit row, so the lit row is always on screen.
A scroll mark shows the top's place when the lines pass one page.
The last row of the frame is a hint and a status.
It is dropped when the frame is one row high.
The hint is the halt notice while halted, else the headword prompt on the lit headword row.
A count of the rows below the page takes the left of that row.
The status says Unsaved while the draft differs from storage, and it is cleared while halted.
Undo and Redo show as muted when they would do nothing.
Only an editor that owns its entries shows them.

## `BRegard`

The esteem strip, which shows and edits the entry's favorite mark, grasp and frequency.
Conduct's `CEsteem` holds all three, and the strip keeps only a copy of what it last announced.
The copies are read again on each esteem change and on each new tenure.
So the strip never asks the gate while drawing.
The strip is dim for an entry not yet stored, since there is nothing to rate.
The favorite shows as a heart and a word, and the heart is blank while unmarked.
The blank is as wide as the heart, so the word does not move when the mark changes.
The grasp shows as a row of stars whose fills come from Conduct, then its label.
The grasp is muted while it is unrated.
The frequency is a word and a bar of the band against the spare, coloured by its tier.
It is shown only for a stored entry.
The keys act on the gate and draw nothing, so the next draw shows the answer.

## `BInput`

The Input tab's frame, which is a `BEditor` over a fresh input editor from the atelier.
It exists since the Input tab has no list beside it, so its editor takes the whole window.
`BInputWhole` is true whenever the tab is shown, and the window folds the index part by it.
The focus is the editor's while the tab is shown, else the window's.
It hands its editor a language loader over the atelier's catalog, since the tab has no owner to pass one.
Drawing returns nothing when the tab is not shown.

## Stamp

Commit: `f3a687d70b47`
- `BEditor.cs` `329eb9208769c6d5`
- `BEditorChronicle.cs` `9419b865bc578853`
- `BEditorSight.cs` `2af2f75320c44ca6`
- `BInput.cs` `21f0003272ef471e`
- `BRegard.cs` `749e910e9229c0b4`
