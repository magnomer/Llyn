# Core

The console's editor frame, which draws and routes keys for one `CEditor`.
It holds no entry rule, so every verdict comes from Conduct.
The Library and Favorite tabs use it in their scribe mode, and the Input tab uses it alone.

## `BEditor`

The frame is one list of rows lit one at a time.
The list is Conduct's `CEditorWalk` for the section in front.
Both faces read that walk, so the console lights the rows the window would show, in the same order.
The frame keeps only the lit place, the section in front, the open field and the scroll top.
The form has four sections, which are meanings, collocations, etymology and note.
Only the first two hold cards in the walk, so the other two show their tab line alone.
The tab line counts the cards of the first two sections, and brackets the section in front.
Keys are live only while the frame's owner has the focus and a draft is held.
So a hidden frame binds nothing and its keys fall through to the window.
The owner passes the shown and focused tests, and the frame never decides them.

Up and Down, the page keys and Control with Home or End move the lit row.
Control with the page keys jump to the previous or next card header.
They skip the head and the tabs.
A key with no card to reach is hidden, so the legend only offers a move that works.
Left and Right pick the section only while the tab line is lit.
Alt with a digit picks the section from anywhere.
Enter folds the lit card header through the walk's own fold gate.
A fresh card has nothing to fold and carries no gate, so Enter is not offered there.

The headword opens as a field on Enter, or on the first printable key while its row is lit.
So the user can type at once without a separate open step.
While the field is open, Up, Down, Enter and Escape close it.
The keys the field reads go to it.
Closing persists the draft, so a field edit is kept as one step.
Up and Down also move the lit row by one after the close.
Opening again starts from the draft's current headword, never from stale text.

F2 saves and is dim while the draft cannot be stored.
F7 undoes to the stored entry, and only an editor that owns its entries offers it.
F6 marks the entry as a favorite.
It is built only when the owner passes a lectern, a reading test and a settle callback.
The settle callback lets the owner keep its list row steady while the mark moves the entry.
These rows are bound at the window focus, since they are not about the lit row.

Ctrl+Z undoes a step, and Ctrl+Y or Ctrl+Shift+Z redoes one.
They sit at the editor focus and show dim when nothing is left to step that way.
A halted tenure makes every key at the editor focus dim.
It also closes the open field, so a dead edit area takes no text.
The halt is the one Conduct stored at the last bulletin, so a key's state cannot flip between draws.
The frame also redraws on the walk's change, since a caret move rebuilds the walk without a desk event.

The draw is `BEditorSight`'s, and the frame hands it the lit place, the open field and the walk.
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
A rule follows the headword row.
A card header is a rule, its position, its caption and a fold glyph.
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

## `BInput`

The Input tab's frame, which is a `BEditor` over a fresh input editor from the atelier.
It exists since the Input tab has no list beside it, so its editor takes the whole window.
`BInputWhole` is true whenever the tab is shown, and the window folds the index part by it.
The focus is the editor's while the tab is shown, else the window's.
Drawing returns nothing when the tab is not shown.

## Stamp

Commit: `c05429890cec`
- `BEditor.cs` `d59ca099856b1c48`
- `BEditorChronicle.cs` `9419b865bc578853`
- `BEditorSight.cs` `47a46d31998783b0`
- `BInput.cs` `f561e7992d03a043`
