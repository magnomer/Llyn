# Editor

Tests of the console editor frame, mirroring `src/Llyn.UIDemeanor/Editor/`.
Each test opens an entry in the Library tab's scribe mode, or runs a whole window on the Input tab.
The tests read the frame from the cells and key rows it drew.
So they pin what a user sees and what the legend offers.

## `TEditorFrame`

The Library cases start from a stored entry named fire and press F4 to enter the scribe mode.
They pin that the right side is titled with Editing and the headword.
It also shows the section tabs and the first card header.
Enter on the headword opens its field and offers Escape as the close.
The first typed letter reaches the draft.
Closing the field leaves the draft Unsaved and offers Enter as the open again.
A printable key on the lit headword opens the field and types the key itself, so no key is lost.
Left and Right choose the section only while the tab line is lit.
Alt with a digit chooses the section from anywhere.
Control with the page keys light only card headers.
Neither is offered where no card lies in that direction.
Control with Home and End reach the first and last row.
The Enter label tells which kind of row is lit.
Enter on a stored card header folds it through Conduct, and the header glyph turns from down to right.
F3 widens the editor over the list.
The first Escape closes an open field, and the next Escape gives the list back.
Tab hands the keys back to the list.
The Input tab case pins that the editor takes the whole window and F7 starts a blank entry.
It also pins that Unsaved shows after a typed headword.
A draft with steps lights Undo at its first draw.
The chronicle's kept state was filled before any draw.
A caret moved without a draft change still asks for a redraw of the face.

## `TEditorChronicle`

These cases pin Undo and Redo as the user meets them.
Control Z with a step undoes it and keeps the field open.
The caret sits at the end of the text.
On a dim row the key leaves the caret where it was typed.
Control Y and Control Shift Z both redo the step.
The right side of the navigation shows Undo and Redo, dimmed by the chronicle's kept state.
Leaving the editor for the list clears that state.
A halted chronicle is the failure case.
The rows dim, the field refuses to open and the hold failure is named once.
Typing after the halt changes nothing.

## `TEditorDraft`

These cases pin what leaving an altered draft asks.
A typed key and a leave key in one batch raise the Leave question and keep the window.
A save with an open field closes the field and stores its text.
A refused save on another editor shows its refusal once and keeps the draft and the lit bar.
On a browse tab the leave keys ask only over an altered draft.
Off the Input tab, only quit and a workspace change ask.
The three answers differ: Save stores, Discard drops, and Cancel stays.
F2 reopens a stored entry, or lights the stored entry of a fresh draft.
A cancelled leave on another row lights the entry still being edited.

## `TEditorKey`

These cases pin the keys that move or leave the editor.
Control Page Up on a later card lights the previous header.
F8 while editing asks before deleting on Library, works on Favorite, and leaves the draft alone on Input.
Tab from the list goes back to the editor.
Escape on an open field closes it and keeps the headword lit.
Up and Down on an open field close it and move the light.
Alt with one to four chooses each stack, and a narrow width keeps the chosen tab whole.

## `TEditorSight`

These cases pin what the user sees around the field.
A closed lit headword parks the cursor at the end of its text.
A side taller than its rows shows a position mark and the count of hidden rows.
The mark moves down after Control End.
An uncertain card title shows the unknown mark.
A save over an unreadable value is refused, naming the unreadable drop and then the save failure.
The draft stays Unsaved.
A refused card add after a typed headword shows the update failure each time and keeps the text.

## Stamp

Commit: `c05429890cec`
- `TEditorChronicle.cs` `ed8d5edbb5c92302`
- `TEditorDraft.cs` `22dac0cedec4ef0a`
- `TEditorFrame.cs` `e8d403015d267be2`
- `TEditorKey.cs` `535ad3c2582640bd`
- `TEditorSight.cs` `fef009fc81ed449d`
