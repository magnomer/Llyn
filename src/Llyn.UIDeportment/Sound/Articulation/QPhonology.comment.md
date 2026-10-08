# QPhonology.cs
Hash: `a36e6314213cc2e1`

## `internal sealed class QPhonology`

Drives the phonology panel and forwards what the user asks of it.
Every branch it once carried lives in Conduct's `CPhonology` and the `CPanel` it holds.
The search, the inventory, the read-only display and the editor are wired here.
The ordering, the filter and the command row each have their own shared driver.
The panel itself is the veneer's `PPhonology` page, which the window places.
It names no Core type, since its rows arrive as `CCatalogPronunciation`.

## `internal QPhonology(UserControl surface)`

Takes the page the window pulled under the contract ID `PPhonology`.
It builds the articulation driver over the nested `PArticulation` page, as a parent driver builds a nested one.
That driver is handed the fold toggle and the aid's seam, since folding the aid is its own concern.
It adds the print and export command bindings to the page.
The rail's print and export buttons sit inside the page, so their commands reach these bindings.
It hands the rail `PPhonologyRail` to a `QPanelRail`, with the bin, the new-record button and the export button.
It hands `PPhonologyOrder` to a `QChoiceOrder`, whose menu hangs under the whole `PSequence` bar.
It hands `PPhonologyFilter` to a `QChoiceFilter`.
Row clicks are taken on the inventory, and the search field and the rail's notices are subscribed here.

## `private Border QSequence`

Each named part is pulled from the page by its contract ID on every read.

## `internal void QPhonologyIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Puts the panel to work through the Conduct phonology panel it builds, which builds its own editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The view wraps that editor for its editor page.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
It hands the picker and the filter the panel's aperture, titled `Sequence` and `Lens`.
The sequence menu is built once from the orders Conduct offers.
The inventory repaints whenever the panel raises its rows.
The filter mark repaints in its own driver, when the reader ticks a language and when the vista opens.
The window fills it when it restores the stored ordering, and every change after that arrives as an announcement.
So the panel is current whether or not its tab is the one in front.
The rail is introduced with the atelier's navigation for its trail and the editor for its chronicle.

## `internal async void QPhonologyVistaRefine()`

Answers the workspace opening, after the Conduct panel restored its vista and attached its own observers.
It marks the chosen order and the filter, then awaits the flags.
The flags are loaded before the first rows are built, because a row reads its flag at construction.
The filter menu lists the languages the flag load answers.
Its one request is `CPhonologyRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QPhonologyStoreRefine()`

Enables the rail's store button only while the editor's desk can store.

## `private async void QPhonologyWorkspaceRefine()`

Answers `CPhonologyWorkspaceChanged` by drawing the flags of the new workspace's languages.
Once the flags are in, it repaints the inventory, whose rows carry a flag.
A failed flag load is reported by Conduct, and the rows are still painted without flags.
Its one request is `CPhonologyRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `private bool QPhonologyShownCheck()`

The shown seam.
It answers whether this tab is the one in front, which only the page knows.

## `internal void QPhonologyExitRefine()`

Releases the editor's recording player when the window closes.
The editor and the playback stop in Conduct, through the close the panel registered with the workspace.

## `private void QInventoryRefine()`

Refills the inventory from the rows the deportment reads, spliced so the list keeps its scroll position.

## `private void QInventoryRefine(IReadOnlyList<CCatalogPronunciation> rows)`

Paints `rows` the area answered ready, so the paint itself asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

## `private void QPhonologyModeRefine()`

Writes the mode and the enablement the deportment holds into the editor, the display and the rail.
The rail folds its own button pairs from the scribe flag it is handed.

## `private void QPhonologyPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live.
It is live exactly when an entry is read in the display.
The deportment answers, so no control state is read.
The binding can be asked before the deportment is built, and then answers false.

## `private async void QPhonologyPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, as the engine portrays it.
The gate asks for the ticket through the envoy and names the vista.
Nothing is read back from the screen.

## `private async void QPhonologyPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.

## `private void QPhonologyChronicleRefine()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.

## `private void QPhonologyScribeObserve(bool scribe)`

Hears the rail's mode toggle and hands the mode to the scribe gate.
`scribe` is true for the editor and false for the read view.

## `private void QProbeObserve(object sender, TextChangedEventArgs e)`

Hands the typed search to the query gate.

## `private void QInventoryObserve(object sender, RoutedEventArgs e)`

Hands the clicked row's id to the panel's row select.

## `private void QPhonologyFreshObserve()`

Hears the rail's new-record notice and asks the panel for a new entry.

## `private void QPhonologyStoreObserve()`

Hands the rail's store notice to the editor's save gate.

## `private void QPhonologyBinObserve()`

Hands the rail's delete notice to the panel's delete gate.

## Inline notes

### `_qArticulation.QArticulationIntroduce(`

The aid is given both fields a phonetic character is typed into.
It is given the atelier's catalog too, which reads both charts the aid lays out.
The search comes first, because a reader who opens the charts with nothing focused is looking a pronunciation up.
The editor's field takes over as soon as the reader focuses it.
The editor's field is pulled from the editor's Veneer by its contract ID `PPronunciationField`.
It is named once here rather than looked up whenever a character is chosen.
