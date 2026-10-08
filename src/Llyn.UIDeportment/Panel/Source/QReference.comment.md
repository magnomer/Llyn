# QReference.cs
Hash: `53c16810e2c9f1c7`

## `internal sealed class QReference : QChronicleHost`

The driver of the Source panel.
It names what the panel is made of and what it forwards.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
Every branch it once carried lives in the Conduct shelf `CShelf` and the two `CPanel` it holds.
The driver forwards each handler to the shelf and writes the page's parts when a notice arrives.
The Conduct shelf, the read areas and the two edit areas are wired here.
The Source list and the entry list have drivers of their own, `QShelf` and `QFootnote`.
The rail, the order picker and the language filter are shared drivers, built here and fed from Conduct.
It is the chronicle host its own rail walks, so the rail's undo and redo reach the session.
The driver picks the area in front by the shelf's side verdict.

## `internal QReference(UserControl surface)`

Takes the veneer page as its surface and builds the drivers of the nested imprint, the colophon and both lists.
It builds the rail with Fresh and Portrait shown, since an Entry is read and exported here.
The order picker hangs its menu under the whole `PGrade` bar.
It subscribes the rail's four notices.

## `private Border QGrade`

Each named part of the page is found through `QContract.QContractFind` under its markup name.

## `internal void QReferenceIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the driver to work on the Conduct shelf it builds, and the shelf builds its own editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the shelf the atelier, the shown seam, the envoy and that marshal.
It hands the shelf to both list drivers, which answer their own rows notices.
The Source list driver also gets the colophon, whose tally it repaints from each Source answer.
The Source rows notice also repaints the imprint's tally, since a stored entry may have moved its count.
The rail gets the window's navigation for its trail and this driver as its chronicle host.
The grade menu is built here once, from the orders `CShelf.CShelfOrderRead` offers.
The Source list's aperture feeds both the order picker and the language filter.
The display view builds the lectern over the editor's display, which Conduct attached to the footnote panel.
The shelf's change notice and both panels' notices repaint the rail, undo and redo included.
The print and portrait command bindings are added last, so no can-execute query meets a shelf not yet built.

## `internal async void QReferenceVistaRefine()`

Answers the workspace opening, after the shelf has restored its vistas and attached its own observers.
It ticks the grade menu and paints the filter mark.
The entry list answers the same opening with its own load, so both loads run side by side.
The flags are loaded before the first rows are built, because an entry row reads its flag at construction.
The filter menu is built from the languages that load answers.
The Source list driver paints the rows of that same answer last.
Its one request is `CShelfRollLoad`, which runs the flag fill and then answers the rows it paints.

## `private bool QReferenceShownCheck()`

Tells the shelf whether the Source page is on screen, so a bulletin is acted on only while shown.

## `internal void QReferenceExitRefine()`

Releases the entry player and closes both pickers' menus when the window exits.
The imprint's kind menu closes with them.

## `private void QReferenceModeRefine()`

Writes which of the four areas is in front from the shelf's side verdicts.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.

## `private void QReferenceChronicleRefine()`

Reads the chronicle of the area in front and writes the rail's undo and redo.

## `private void QReferenceFreshObserve()`

Asks the gate for a new Source when the rail's Fresh is clicked.

## `private void QReferenceScribeObserve(bool scribe)`

Asks the shelf for the side the rail's toggle names.

## `private void QReferenceStoreObserve()`

Asks the session to save.

## `public void QChronicleUndoObserve()`

Walks the session back one step, with the caret kept where it was.
The rail's backward button calls it through `QChronicleHost`.

## `public void QChronicleRedoObserve()`

Walks the session forward one step, the mirror of the undo.

## `private void QReferenceBinObserve()`

Asks the gate to delete the chosen Source.

## `private void QReferencePressRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the print command from the shelf's one press verdict.

## `private async void QReferencePressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the source being read, as the engine portrays it.
The shelf asks for the ticket through the envoy, picks the vista and words the page.
Nothing is read back from the screen.

## `private void QReferencePortraitRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the export command from the shelf's portrait verdict, which holds only while an entry is read.

## `private async void QReferencePortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
