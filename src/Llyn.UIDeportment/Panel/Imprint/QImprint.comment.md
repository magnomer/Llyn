# QImprint.cs
Hash: `6fcb0592b6d47f30`

## `internal sealed class QImprint : QChronicleHost`

The driver of the Source edit area, built by the Source panel over its veneer page.
It holds no state, since Conduct's `CImprint` holds the desk, the blank row and the byline.
`CImprint` also wires its desk's notices at build, so this driver attaches no observer.
Every handler hands what the page carries to one gate, and every update writes what Conduct answers.
It drives the four fields and the kind chip, and hands the credit rows and the byline to `QAuthor`.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `internal QImprint(UserControl surface)`

Takes the page and pulls each part through `QContract.QContractFind`.
Builds `QAuthor` over the `PAuthor` grid, which wires the credit rows and the byline itself.
Subscribes the four fields and ties the kind chip to its menu.

## `internal void QImprintIntroduce(CImprint imprint)`

Takes the source editor the shelf composed and listens to its Source notice.
It builds the kind menu once from the editor's `CImprintKindRead`, since the menu is read through the editor's port.
It hands the editor on to `QAuthorIntroduce`, so the author part listens to its own notices.
It calls no gate, so the panel's introduce hands the engine one request only.

## `internal void QImprintCloseRefine()`

Shuts the kind menu when the window closes.
The byline's close is the shelf's, run with the atelier's closures.

## `internal void QImprintClearRefine()`

Writes the editor's empty Source, so every field blanks and restores its placeholder.

## `internal void QImprintTallyRefine()`

Writes how many places cite the stored Source, as the editor answers it.
The Source panel also runs it whenever the shelf rows change.
A citation made elsewhere moves the count without a new draft.

## `public void QChronicleUndoObserve()`

Walks the desk back one step, with the caret kept where it was.

## `public void QChronicleRedoObserve()`

Walks the desk forward one step, the mirror of the undo.

## `private void QImprintDraftRefine(CReference reference)`

Writes the four fields, their placeholders, the kind chip and the tally from the Source the imprint announced.
A field whose text is unchanged keeps its caret.
The framework ignores a write of the same text.

## `private void QImprintTitleObserve(object sender, TextChangedEventArgs e)`

Hands the typed title to `CImprintTitleSet`.

## `private void QImprintYearObserve(object sender, TextChangedEventArgs e)`

Hands the typed year to `CImprintYearSet`.

## `private void QImprintUrlObserve(object sender, TextChangedEventArgs e)`

Hands the typed address to `CImprintUrlSet`.

## `private void QImprintNoteObserve(object sender, TextChangedEventArgs e)`

Hands the typed note to `CImprintNoteSet`.

## `private void QImprintKindObserve(object sender, RoutedEventArgs e)`

A picked kind option, whose tag goes raw to `CImprintKindSet`.
It then hands off to `QImprintKindRefine`, which shuts the chip.

## `private void QImprintKindRefine()`

Shuts the kind chip so its menu closes after a pick.
