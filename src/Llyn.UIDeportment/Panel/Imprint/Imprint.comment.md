# Imprint

The Source edit area of the library, plus the undo and redo chords every draft editor answers.
It stands apart because the Source form has credit rows with a byline dropdown, which no other form has.
Drivers here hold no state, since Conduct's `CImprint` holds the desk, the blank row and the byline.
Every handler hands what the page carries to one gate, and every update writes what Conduct answers.

## `QImprint`

The driver of the Source edit area, built by the Source panel over its veneer page.
`CImprint` wires its desk's notices at build, so this driver attaches no observer.
It drives the four fields and the kind chip, and hands the credit rows and the byline to `QAuthor`.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.
The constructor pulls each part through `QContract.QContractFind` and builds `QAuthor` over the `PAuthor` grid.
It subscribes the four fields and ties the kind chip to its menu.
`QImprintIntroduce` takes the source editor the shelf composed and listens to its Source notice.
It builds the kind menu once from `CImprintKindRead`, since the menu is read through the editor's port.
It hands the editor on to `QAuthorIntroduce`.
It calls no gate, so the panel's introduce hands the engine one request only.
`QImprintCloseRefine` shuts the kind menu when the window closes.
The byline's close is the shelf's, run with the atelier's closures.
`QImprintClearRefine` writes the editor's empty Source, so every field blanks and restores its placeholder.
`QImprintTallyRefine` writes how many places cite the stored Source, as the editor answers it.
The Source panel also runs it whenever the shelf rows change.
A citation made elsewhere moves the count without a new draft.
The undo and redo observers walk the desk one step back or forward, with the caret kept where it was.
`QImprintDraftRefine` writes the four fields, their placeholders, the kind chip and the tally from the announced Source.
A field whose text is unchanged keeps its caret, because the framework ignores a write of the same text.
The title, year, url and note observers hand the typed text to their `CImprint` setters.
The kind observer hands the option's tag raw to `CImprintKindSet`.
Then it shuts the chip so its menu closes after a pick.

## `QAuthor`

The credit rows and the byline of the Source edit area, built by `QImprint` over the `PAuthor` grid.
It holds no state beyond the spliced rows.
It subscribes what it paints itself, so the owner only builds and introduces it.
The byline popup sits inside the grid, so every part it drives is found from the grid.
The constructor subscribes the credit list's typing, keys, focus and hover, and attaches the credit and byline item fills.
The credit fill subscribes the row's four handles and is followed by the handle update.
The byline fill subscribes this driver's press handler on each row.
The byline popup places itself under its field through the shared field helper.
`QAuthorIntroduce` listens to the editor's credit, focus and revert notices and to its byline.
It calls no gate, so the panel's introduce still hands the engine one request only.
`QAuthorRefine` splices the credit rows, so a row that kept its Author and place keeps its field.
The unsaved notice shows while no draft is held.
`QAuthorFocusRefine` gives the keyboard to the blank credit row once the rows are spliced, when Conduct announces the focus.
`QAuthorShelfIntroduce` subscribes a realized row's four buttons, each to the handler of its own gate.
A row realized again is unsubscribed first, so no button fires twice.
`QAuthorRestoreRefine` writes the held name back into whichever credit field has the keyboard.
`QBylineRefine` reads the offered rows, targets the popup at the focused field, and opens or closes it.
The lit row follows the byline's index and is scrolled into view.
The frame takes the field's width as its least width, as the markup once bound it.
The add button goes to `CImprintAuthorAdd` with the row's place and Author.
No button hands the row's blank mark, so Conduct decides blankness from its own marker.
The remove button goes to `CImprintAuthorRemove`, and the earlier and later buttons go to `CImprintAuthorMove`, one place back or on.
Typing in a credit field is passed on with whether the field has the keyboard.
A field the edit area wrote has no keyboard, and the byline opens nothing for it.
A key in a credit field goes to the gate of its key with the row it came from.
Enter finishes the credit with the text and the lit byline row, and Escape cancels it.
Down and Up move the lit byline row.
The gate answers whether it took the key, and any other key is left to the field.
Setting `Handled` from that verdict is routing, not a look.
Leaving a credit field closes the byline through `CBylineClose`.
It then hands the field to the row's revert, which drops what was typed.
A press on an offered byline row is subscribed on each realized row.
`QAuthorShelfRefine` shows every credit row's handles while the pointer or the caret is in the list, and hides them otherwise.
It runs on the list's hover and focus changes and after every credit row fill.

## `QAuthorItem`

Presentation item for one credit row, copied from the source editor's row.
The list is spliced on each notice.
So a row that keeps its id and place keeps its field and caret.
The constructor takes each value apart, so the row's changing fields are written by the shell alone.
`QAuthorItemBlank` copies Conduct's `CAuthorRowBlank` mark, so no id value has to mean blank.
An engine row is thus never taken for the blank row, whatever its id.
`QAuthorItemName` raises on change, so a rename elsewhere reaches a kept row.
`QAuthorItemEarlier` and `QAuthorItemLater` say whether a row above or below exists.
So the move control is offered only where it can act.
`QAuthorItemBuild` makes one item per credit row, the blank row already placed among them.
`QAuthorItemMatch` calls two rows the same when they credit the same Author at the same place, blank or not.
The name is not part of it, so a rename is synced rather than rebuilt.
`QAuthorItemSync` copies the name and both order flags onto the held row that `QSplice` kept.
The id and place are left, since the match already found them equal.
Each setter raises only on a change, so an untouched row repaints nothing.
`QAuthorItemFind` finds the blank row, which the caret goes to after an add.
The source form of `QAuthorItemRefine` writes the held name back into the field the event came from.
Anything but a credit field is left alone, so a handler may pass an event source through unchecked.
The container form fills one credit row: its name, the handles' enablement and the four handle icons.
The name is refilled only when the row's name changed, so typing in a kept row is never overwritten.

## `QBylineItem`

One stored Author offered in the dropdown, holding the Author's id and the name it reads.
Choosing a row credits that Author by id, so a rename in the workspace follows the credit.
`QBylineItemLead` is the first of three name pieces: what stands before what was typed, what was typed, and what follows.
The row draws the middle piece in weight, so a reader sees why the row is offered.
The engine split the name where it matched, so the item only copies the pieces.
`QBylineItemRead` answers the Author a lit row names, or null while no row is lit.
The veneer thus passes a list's choice through unshaped.
`QBylineItemBuild` makes one item per Author the engine offered.
`QBylineItemRefine` fills the three runs of one row.
It subscribes the press handed in once, removing it first so a refill never doubles it.

## `QChronicle`

The one wrap every host puts around its engine step.
It also keeps the hosts that drive a veneer surface, since such a surface is not a host itself.
It owns the key chords that walk a draft's chronicle: Ctrl+Z back, Ctrl+Y or Ctrl+Shift+Z forward.
The window subscribes the chords once, so every draft editor answers them the same way.
Which editor answers is decided by where the keyboard is, not by which panel is showing.
`QChronicleHold` holds each surface's driver weakly, so a dropped surface frees its entry.
`QChronicleIntroduce` names the driver that answers the keys for everything inside a surface.
`QChronicleRead` answers the driver attached to an element, or null.
`QChronicleCaretRefine` runs the step and keeps the caret at the end of the focused text box.
Restoring a field's text through a bulletin drops the caret to zero.
Bulletins fire synchronously on the calling thread, so the restore has happened when the step returns.
The caret is moved only when the same box still holds the keyboard.
`QChronicleFocusRead` climbs from the focused element to the first ancestor that answers undo and redo.
The situation and example forms each hold an entry form inside them.
Climbing meets the nearer of the two first, which is the one the user is typing in.
A focused element outside the visual tree climbs the logical tree instead, so a text run still finds its form.
Nothing focused, or nothing above it that answers, says the chord is not ours.
`QChronicleKeyObserve` turns the chord into an undo or a redo on the host found.
It swallows the key when one was.
The window subscribes it on preview, so it beats the text box's own undo.
The theme has switched that undo off anyway.
A chord with no host under it falls through untouched.

## `QChronicleHost`

What the window's undo and redo keys ask of whichever draft editor holds the keyboard.
Five drivers answer: the entry form, the source form, the situation form, the example form and the guild.
Each asks the engine to step its chronicle, and the engine flushes the pending request first.
The engine's draft bulletin brings the restored fields back through the same path a reload uses.
So nothing here draws anything by hand.
The desk's own state notice settles the undo and redo buttons after every step.

## Stamp

Commit: `c2f335c331fe`
- `QAuthor.cs` `b182e1ab73d67219`
- `QAuthorItem.cs` `44206716aeb026c4`
- `QBylineItem.cs` `306f99953f0cd7a3`
- `QChronicle.cs` `66432adee340042e`
- `QChronicleHost.cs` `d93626efbc3308d1`
- `QImprint.cs` `6fcb0592b6d47f30`
