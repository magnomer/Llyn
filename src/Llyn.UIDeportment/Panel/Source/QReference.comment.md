# QReference.cs
Hash: `9976e0477e1cb062`

## `internal sealed class QReference`

The driver of the Source panel.
It names what the panel is made of and what it forwards.
A Source is independent data owned by nothing, so this panel is not a view of one citation of it.
The visible label is Source and the internal base is Reference, because Source already names a pronunciation source.
Every branch it once carried lives in the Conduct shelf `CShelf` and the two `CPanel` it holds.
The driver forwards each handler to the shelf and writes the page's parts when a notice arrives.
The shelf, the entry list, the read areas, the two edit areas and the shared rail are all wired here.
The driver picks the area in front by the shelf's side verdict.

## `internal QReference(UserControl surface)`

Takes the veneer page as its surface and builds the drivers of the nested imprint and colophon pages.
It points the export and print buttons at their commands, ties both droppers, and sets every icon.
It subscribes every click, both search fields and both list clicks, and attaches the two item fills.

## `private Border QGrade`

Each named part of the page is found through `QContract.QContractFind` under its markup name.

## `internal void QReferenceIntroduce(QWindow host)`

Puts the driver to work on the Conduct shelf it builds, and the shelf builds its own editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the shelf only the shown seam, and subscribes to the notices of both lists.
The Source rows notice also repaints the imprint's tally, since a stored entry may have moved its count.
The grade menu is built here once, from the orders `CShelf.CShelfOrderRead` offers.
The lectern follows the entry list's panel, as the other panel drivers build it.
The shelf's change notice and both panels' notices repaint the rail, undo and redo included.
The print and portrait command bindings are added last, so no can-execute query meets a shelf not yet built.

## `internal async void QReferenceVistaRefine()`

Answers the workspace opening, after the shelf has restored its vistas and attached its own observers.
It ticks the grade menu and paints the filter mark.
The flags are loaded before the first rows are built, because an entry row reads its flag at construction.
The filter menu is built from the languages that load answers, and the Source rows are painted last.
Its one request is `CShelfRollLoad`, which runs the flag fill and then answers the rows it paints.

## `internal async void QFootnoteVistaRefine()`

Answers the workspace opening for the entry list, which has its own first paint.
It awaits the same flag load, then paints the entry rows.
Its one request is `CFootnoteRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private bool QReferenceShownCheck()`

Tells the shelf whether the Source page is on screen, so a bulletin is acted on only while shown.

## `internal void QReferenceExitRefine()`

Releases the entry player and closes both dropdowns when the window exits.
The imprint's kind menu closes with them.

## `private void QShelfRefine()`

Paints the rows, the empty notice and the colophon tally from one shelf answer.
The rows are spliced, so the list keeps its scroll position.

## `private void QShelfRefine(CShelfRoll roll)`

Paints `roll` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QFootnoteRefine()`

Refills the entry list, and shows the empty text while no row stands.
The aperture chooses the text's key by whether the list is being searched.

## `private void QFootnoteRefine(IReadOnlyList<CVistaRow> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QReferenceModeRefine()`

Writes which of the four areas is in front and the enablement the shelf holds into the rail.
The voyage and chronicle pairs follow the viewer and scribe verdicts.

## `private void QReferenceChronicleRefine()`

Reads the chronicle of the area in front and writes the rail's undo and redo.

## `private void QTrellisListRefine(IReadOnlyList<string> languages)`

Builds the filter menu from the languages it is handed, ticking the languages the vista shows.

## `private void QGradeObserve(object sender, RoutedEventArgs e)`

Hands the chosen order to the shelf, then closes the grade dropper.

## `private void QReferenceViewerObserve(object sender, RoutedEventArgs e)`

Asks the shelf to leave edit mode, since the viewer button was pressed.

## `private void QReferenceScribeObserve(object sender, RoutedEventArgs e)`

Asks the shelf to enter edit mode, since the scribe button was pressed.

## `private void QReferencePressRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the print command from the shelf's one press verdict.

## `private async void QReferencePressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the source being read, as the engine portrays it.
The shelf asks for the ticket through the envoy, picks the vista and words the page.
Nothing is read back from the screen.

## `internal void QReferenceVoyageRefine(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the driver only shows what it is told.

## `private void QReferenceRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QReferenceAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QTrellisRefine()`

Shows the filter mark while the vista hides any language.

## `private void QSurveyObserve(object sender, TextChangedEventArgs e)`

Hands the typed Source search to the query gate.

## `private void QRummageObserve(object sender, TextChangedEventArgs e)`

Hands the typed entry search to the entry list's query gate.

## `private void QTrellisObserve(object sender, RoutedEventArgs e)`

Hands the ticked languages to the filter gate, then shows the filter mark.

## `private void QGradeRefine()`

Unchecks the grade dropper so its popup closes after a pick.

## `private void QShelfObserve(object sender, RoutedEventArgs e)`

Hands the clicked Source's id to the shelf's select gate.

## `private void QFootnoteObserve(object sender, RoutedEventArgs e)`

Hands the clicked entry's id to the shelf's entry gate.

## `private void QReferenceFreshObserve(object sender, RoutedEventArgs e)`

Asks the gate for a new Source.

## `private void QReferenceStoreObserve(object sender, RoutedEventArgs e)`

Asks the session to save.

## `private void QReferenceUndoObserve(object sender, RoutedEventArgs e)`

Walks the session back one step, with the caret kept where it was.

## `private void QReferenceRedoObserve(object sender, RoutedEventArgs e)`

Walks the session forward one step, the mirror of the undo.

## `private void QReferenceBinObserve(object sender, RoutedEventArgs e)`

Asks the gate to delete the chosen Source.

## `private void QReferencePortraitRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the export command from the shelf's portrait verdict, which holds only while an entry is read.

## `private async void QReferencePortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
