# QTenor.cs
Hash: `31b39569554a49d3`

## `internal sealed class QTenor`

The tenor panel browses the workspace by the Registers its cards carry.
It is the taxonomy panel's shape read through a different question, so it holds the same three columns.
It owns a reader and an editor over one Entry, and answers the engine rather than its own visibility.
The panel itself is the veneer's `PTenor` page, which the window places.
The register catalog is painted here, and the entry list beside it by its own driver `QCohort`.

## `internal QTenor(UserControl surface)`

Takes the page the window pulled under the contract ID `PTenor`.
It adds the print and export command bindings to the page.
The rail's print and export buttons sit inside the page, so their commands reach these bindings.
It hands the rail `PTenorRail` to a `QPanelRail`, with the bin, the new-record button and the export button.
It hands `PTenorOrder` to a `QChoiceOrder`, whose menu hangs under the whole register search bar.
It hands `PTenorFilter` to a `QChoiceFilter`, and the page to the entry list driver `QCohort`.
It subscribes the register search field and the rail's notices.

## `private Border QDegree`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QTenorIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the panel to work through the Conduct tenor panel it builds, which builds its own editor.
It keeps the atelier and envoy for the catalog load, and hands the volume and mention menu on.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the picker and the filter the register aperture, titled `Degree` and `Grille`.
The ordering menu is built once, with the third ordering that only a register catalog offers.
The view wraps that editor for its editor page.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
The entry list driver is introduced with the area, so it subscribes its own rows and opening.
The register catalog gets its row fill here, where its source is set.
The area's rows, opening and workspace events are subscribed here, each to its own Refine.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.

## `private void QTenorStoreRefine()`

Enables the rail's store button only while the editor's draft can be stored.

## `internal void QTenorExitRefine()`

Releases the editor's player, so none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` closes the editor and stops its playback in Conduct.

## `private void QTenorPressRefine(object sender, CanExecuteRoutedEventArgs e)`

The print and export buttons are live exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The buttons follow this answer on their own, so no panel state has to switch them.

## `private async void QTenorPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names nothing, and the engine builds the page from the vista and stored rows.
Nothing is read back from the screen.

## `private async void QTenorPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
Nothing is read back from the screen.

## `private void QTenorChronicleRefine()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private bool QTenorShownCheck()`

Tells the panel whether the tenor page is on screen, so a bulletin is acted on only while shown.

## `private void QTenorModeRefine()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The rail folds its own button pairs from the editing flag it is handed.

## `private CTenor _cTenor = null!;`

The panel's Conduct, holding the register vista and the cohort vista it restored.
The vista carries the order, the query, the chosen Register, and the languages hidden from its entries.
The panel keeps no copy of any of the four and asks Conduct for each where it needs it.
The Register is held by its id rather than its name, since a name may be rewritten under the panel.
A null Register is not an absence to be corrected.
It is the whole workspace, which is what the panel shows first.
The Conduct tenor is null until the window introduces the panel, so only the print gate checks for it.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private async void QTenorWorkspaceRefine()`

Answers the area's workspace event, which has already closed the entry, cleared the chosen Register and raised the rows.
It loads the flags of the workspace that moved.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `private void QSoundingObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement rebuilds the catalog.

## `internal async void QTenorVistaRefine()`

Answers the workspace opening, after `CTenor` has restored its vistas and attached its observers.
The picker's checked row and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The filter's language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.
Its one request is `CTenorRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QTenorFreshObserve()`

Hears the rail's new-record notice.
New makes a Register while none is chosen and an Entry under the chosen Register otherwise.
The one fresh gate decides, and it asks the leave question and any wording through the envoy.

## `private void QTenorScribeObserve(bool scribe)`

Hears the rail's mode toggle and swaps the read view and the editor through the shared panel.
`scribe` is true for the editor and false for the read view.

## `private void QTenorStoreObserve()`

Hands the rail's store notice to the editor's save gate.

## `private void QTenorBinObserve()`

Hands the rail's delete notice to the panel's delete gate.

## `private void QGamutRefine()`

Answers the area's rows event and lists the Registers the area reads, already counted and in the vista's ordering.
The chosen Register is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.

## `private void QGamutRefine(IReadOnlyList<CCatalogRegister> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QGamutRegisterRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the register field only shows it.
`QCohort` empties the entry field from the same event.
The field's own handler still hears the change, and its gate finds the query already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

## `private void QGamutObserve(object sender, RoutedEventArgs e)`

A clicked row hands its Register id to the toggle gate, and a click that carries no item is ignored.
The gate records the station, toggles and raises the rows, so clicking the chosen Register lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

## `private void QGamutItemRefine(FrameworkElement container, object item, string? _)`

Fills one register row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen Register stays visible while the eye is on the Entries beside it.
Without it the entry list would show a filtered set with nothing on screen saying which filter.
