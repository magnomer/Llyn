# QTaxonomy.cs
Hash: `1afc33e14b053fa9`

## `internal sealed class QTaxonomy`

Drives the taxonomy panel.
It owns what the panel is made of, and when it starts and stops.
The tag catalog is painted here, and the entry list beside it by its own driver `QMembership`.
The reader and editor stand beside them.
The panel itself is the veneer's `PTaxonomy` page, which the window places.

## `internal QTaxonomy(UserControl surface)`

Takes the page the window pulled under the contract ID `PTaxonomy`.
It adds the print and export command bindings to the page.
The rail's print and export buttons sit inside the page, so their commands reach these bindings.
It hands the rail `PTaxonomyRail` to a `QPanelRail`, with the bin, the new-record button and the export button.
It hands `PTaxonomyOrder` to a `QChoiceOrder`, whose menu hangs under the whole tag search bar.
It hands `PTaxonomyFilter` to a `QChoiceFilter`, and the page to the entry list driver `QMembership`.
It subscribes the tag search field and the rail's notices.

## `private Border QFunnel`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QTaxonomyIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the panel to work through the Conduct taxonomy it builds, which builds its own editor.
It keeps the atelier and envoy for the catalog load, and hands the volume and mention menu on.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the picker and the filter the tag aperture, titled `Funnel` and `Lattice`.
The ordering menu is built once, here.
The view wraps that editor for its editor page.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
The entry list driver is introduced with the area, so it subscribes its own rows and opening.
The tag catalog gets its row fill here, where its source is set.
The area's rows, opening and workspace events are subscribed here, each to its own Refine.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.
The window fills the panel when the workspace opens, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.

## `private void QTaxonomyStoreRefine()`

Enables the rail's store button only while the editor's draft can be stored.

## `internal void QTaxonomyExitRefine()`

Releases the editor's player, so none outlives the window.
It calls no gate.
The window's exit gate `CAtelierClose` closes the editor and stops its playback in Conduct.

## `private void QTaxonomyPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live: exactly when an entry is read in the display.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QTaxonomyPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The panel names nothing, and the engine builds the page from the vista and stored rows.
Nothing is read back from the screen.

## `private async void QTaxonomyPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
Nothing is read back from the screen.

## `private void QTaxonomyChronicleRefine()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private bool QTaxonomyShownCheck()`

Tells the panel whether the taxonomy page is on screen, so a bulletin is acted on only while shown.

## `private void QTaxonomyModeRefine()`

Paints the mode from the shared panel state.
No control's visibility stands in for the mode any more.
The rail folds its own button pairs from the editing flag it is handed.

## `private async void QTaxonomyWorkspaceRefine()`

Answers the area's workspace event, which has already closed the entry, cleared the chosen Tag and raised the rows.
It loads the flags of the workspace that moved.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `private void QExplorationObserve(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement rebuilds the catalog.

## `internal async void QTaxonomyVistaRefine()`

Answers the workspace opening, after `CTaxonomy` has restored its vistas and attached its observers.
The picker's checked row and the filter mark are drawn from the vista first.
The flags are loaded before any row is built.
The filter's language menu is then built from the languages that load answers.
The catalog is then listed once, so a row keeps the flag it was built with.
Its read raises the entry rows, so the entry list is painted after the flags too.
Its one request is `CTaxonomyRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QDirectoryRefine()`

Answers the area's rows event and lists the Tags the area reads, already in the vista's ordering.
The chosen Tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A failed read has already been shown by the area, which then answers no rows.
A read that succeeds makes the area raise the entry rows, so the entry list follows.

## `private void QDirectoryRefine(IReadOnlyList<CCatalogTag> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QDirectoryObserve(object sender, RoutedEventArgs e)`

A clicked row hands its Tag's id to the toggle gate, and a click that carries no item is ignored.
The gate records the station and toggles, so clicking the chosen tag lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.
The gate raises the rows, so the view rebuilds nothing itself.

## `private void QDirectoryItemRefine(FrameworkElement container, object item, string? _)`

Fills one tag row from its item, the work its bindings did before.
The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is subscribed once per row, removed first so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.
The chosen tag stays visible while the eye is on the entries beside it.
Without it the membership list would show a filtered set with nothing on screen saying which filter.

## `private void QDirectoryTagRefine()`

Answers the area's opening event after a chip's arrival or a coinage.
The area has already emptied both queries, so the tag field only shows it.
`QMembership` empties the entry field from the same event.
The field's own handler still hears the change, and its gate finds the query already empty.
The area raises the rows right after, so the catalog is rebuilt without the view asking.

## `private void QTaxonomyFreshObserve()`

Hears the rail's new-record notice.
New makes a Tag while none is chosen and an Entry under the chosen Tag otherwise.
The one fresh gate decides, and it asks the leave question and any wording through the envoy.

## `private void QTaxonomyScribeObserve(bool scribe)`

Hears the rail's mode toggle and swaps the read view and the editor through the shared panel.
`scribe` is true for the editor and false for the read view.

## `private void QTaxonomyStoreObserve()`

Hands the rail's store notice to the editor's save gate.

## `private void QTaxonomyBinObserve()`

Hands the rail's delete notice to the panel's delete gate.

## Inline notes

### `private CAtelier _cAtelier = null!;`

The atelier whose catalog the flag load needs.
The kept envoy goes with that load, so a failure is shown.
