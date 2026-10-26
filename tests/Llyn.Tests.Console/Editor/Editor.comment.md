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
Three head rows lie between the headword and the tab line.
So a test presses Down past them to reach the tabs.
The window the case runs is wired as the real one is, with the loop's key and batch routes attached.
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
A side that fits its rows shows neither, and one row more shows both.
A headword longer than the side parks the cursor on the last cell of the bar.
This holds for narrow and wide characters alike.
The mark moves down after Control End.
An uncertain card title shows the unknown mark.
A save over an unreadable value is refused, naming the unreadable drop and then the save failure.
The draft stays Unsaved.
A refused card add after a typed headword shows the update failure each time and keeps the text.
Three cards overflow the side and two do not.
So the thumb and the count show only past the fit.

## `TEditorHead`

These cases pin the language pill and the esteem strip, the two head rows under the headword.
Enter on the pill opens the language list, and a letter lights the next language that starts with it.
Moving the light off the pill shuts the list.
So no list stays open on a row that is not lit.
A second open during the load loads once, and a close during the load leaves the list shut.
The reason is that the load is slow and the user can leave before it ends.
Enter on a lit language sets the draft's language and shuts the list, and Escape shuts only the list.
On the esteem row the arrows and the digits move, set and clear the grasp through Conduct.
F6 flips the favorite mark of a stored entry, drawn as a heart and the word.
A fresh draft has nothing stored, so the strip dims and the keys that need a stored entry are dimmed.

## `TEditorMarker`

These cases pin the part-of-speech row, with its unit chooser, freedom chooser and speech field.
Enter on a chooser opens its list, and a pick sets the draft through Conduct.
Picking the held freedom checks it first and then clears it, a rule the tenure keeps.
Typing on the speech spot opens the field and offers a menu, and a lit menu row becomes a chip.
Escape on the menu shuts only the menu and keeps the typing, and a second Escape closes the field.
A menu that matches nothing shows its hint as one muted line with no shadow.
No row can be lit there.
Moving the light off the marker row closes its open list.
A failed language load opens an empty list rather than none.

## `TEditorInlet`

These cases pin the chip fields under a card header, taking the Situation field as the model.
An empty field shows its hint, and the lit one also shows it on the foot.
A typed comma makes a bracketed chip and drops the hint.
A lit stored row becomes a chip and clears the entry.
Enter with no row lit settles the typed text.
Escape shuts the list first, and a second Escape closes the field and settles its text.
Backspace and Left on an empty entry remove a chip or step past it through Conduct's trail.
The light leaving a card closes its open field and settles the text.
The Register and Tag fields show braced and hashed chips.
Many chips wrap inside the card.
A refused settle on close keeps the typed text for the reopened field and draws it without the hint.
Moving the light off the card or saving drops that kept text, since it belongs to the field's card.
A halt closes an open field, and it takes no more keys.

## `TEditorLink`

These cases pin the Translation field, the fourth part of a card.
An empty field shows its hint, lit on the card.
A comma after a stored word makes an angled chip that names the language.
Enter on a whole match lights it, and a second Enter links it.
A lit fresh row hands the raw row on and links a tentative entry.
A headword that repeats in the list reads its number.
Closing the field links a word one entry answers and keeps the rest.
A failed resolve on close shows the translation failure and keeps the text.
Backspace on an empty entry removes the chip before it.

## `TEditorDrop`


These cases pin the drop-list box of `BDrop` as the editor uses it.
A letter lights the next row that starts with it, ignoring case.
The same letter again steps on and wraps.
A letter that matches the lit row's own start still moves on to the next match.
A control key or an empty list lights nothing and is not taken.
The box hangs under the field with a shadow when there is room.
It flips above the field when the window ends before the box.
When neither side fits it is clamped to the bounds.
So no cell of the box or its shadow leaves the part.

## Stamp

Commit: `f3a687d70b47`
- `TEditorChronicle.cs` `ed8d5edbb5c92302`
- `TEditorDraft.cs` `c85e82a5c120686d`
- `TEditorDrop.cs` `ba90316114a19401`
- `TEditorFrame.cs` `6b45be9c10b22e68`
- `TEditorHead.cs` `3115a20cd327ac3e`
- `TEditorInlet.cs` `15b3182018ef265b`
- `TEditorKey.cs` `a467932b90258d3e`
- `TEditorLink.cs` `c50b0e48d2673bea`
- `TEditorMarker.cs` `942a2bd8a8747e92`
- `TEditorSight.cs` `e24e48cce5d13ef6`
