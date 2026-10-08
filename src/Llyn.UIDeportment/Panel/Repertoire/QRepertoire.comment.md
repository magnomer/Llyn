# QRepertoire.cs
Hash: `e3253b9e05bcee03`

## `internal sealed class QRepertoire : QChronicleHost`

The Repertoire panel's driver, the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the surface, the atelier and envoy, the entry editor and display drivers, and the rail.
It also holds the two pickers and the repertoire Conduct.
The catalog, the entry column, the Situation reading and its editing are the sub-drivers `QAtlas`, `QOccurrence`, `QVignette` and `QScenario`.
The panel answers two questions rather than one.
It shows what this Situation is, and where it is used.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `internal QRepertoire(UserControl surface)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
The page's local styles are handed to the look sheet, which otherwise knows only the application's.
The driver attaches itself to the page as the host of the undo and redo keys.
It builds the rail with Fresh and Portrait shown, and the two pickers over their placed controls.
The order picker hangs its menu under the whole `PTier` bar.
It builds the four sub-drivers over the same page, each finding its own parts.
It adds the print and export command bindings, which the rail's buttons reach.
It subscribes the rail's four notices.

## `private Border QTier`

Each named part of the page is pulled through `QContract.QContractFind` by its contract ID.

## `private CRepertoire _cRepertoire = null!;`

The repertoire Conduct, holding the atlas, the occurrence list, the playwright, the session and the panel's mode.
The atlas's vista carries the order, the inquest, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until `QRepertoireIntroduce` builds it, so only the print and portrait gates guard against that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `internal void QRepertoireIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Builds the repertoire Conduct, which builds its editor.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The display view builds the lectern over the editor's display, which Conduct attached to the occurrence panel.
Keeps the atelier and envoy, which the catalog load runs through.
It introduces `QScenario`, `QVignette`, `QAtlas` and `QOccurrence`, which subscribe the notices they paint themselves.
The occurrence and vignette drivers take the window's atelier, never the window.
It attaches the entry display and editor to the same atelier and facets, so an Entry is read and written.
The engine's change notices drive the mode.
Its failures reach the window through the envoy, which the Conduct asks directly.
A dropped inquest rebuilds the language menu through `QRepertoireClearRefine`, while `QAtlas` empties the box.
A workspace change reloads the flags.
It hands the rail the window's navigation and itself as the chronicle host.
It hands both pickers the atlas's aperture, and the order picker the Conduct's fixed list of orderings.
The window fills the situation catalog when the workspace opens, and `QOccurrence` fills the entry column on the same notice.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `internal async void QRepertoireVistaRefine()`

Answers the workspace opening, after the Conduct has started the fresh vistas and attached their observers.
The two pickers tick the ordering from the atlas and draw the filter mark.
The flags are loaded before any row is built.
The filter picker then builds its language menu from the languages that load answers.
The catalog is then listed once through `QAtlas`.
The Conduct carried any search text held into the fresh vistas, so the boxes need no re-send.
Its one request is `CRepertoireRowsLoad`, which runs the flag fill and then answers the rows it paints.

## `internal void QRepertoireExitRefine()`

The Veneer half of the window's exit, since the editor's stop and the playback cancel run in Conduct.
Releases the editor's recording player and closes both pickers' menus, so neither outlives the window.

## `public void QChronicleUndoObserve()`

Steps whichever draft is in front one snapshot back, through the Conduct.
The step runs inside `QChronicle.QChronicleCaretRefine`, so the caret stays at the end of the focused box.
The draft bulletin the engine raises brings the older fields back through the ordinary restore.

## `public void QChronicleRedoObserve()`

Steps whichever draft is in front one snapshot forward again.
The inverse of the undo above, through the same bulletin.

## `private bool QRepertoireShownCheck()`

Whether the panel is on screen, so the engine knows when a notice needs painting.

## `private void QRepertoireModeRefine()`

Paints the mode the engine decides.
`QScenarioVisibleRefine` lights the writing side, and `QVignetteVisibleRefine` the reading side.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.
It ends by refreshing the rail's undo and redo.

## `private void QRepertoireChronicleRefine()`

Lights the rail's two chronicle buttons only when the draft in front has a step to walk.
The Conduct reads the entry editor's chronicle while `PEditor` is in front, and the held Situation's otherwise.

## `private void QRepertoireClearRefine()`

Has the filter picker rebuild its language menu after an arrival dropped the query on the vista.
The menu is read back from the vista's cleared filter, and the filter mark follows.

## `private async void QRepertoireWorkspaceRefine()`

Answers the Conduct's workspace change by reloading the flags, since they do not belong to the old folder's rows.
The Conduct has already closed the shown Situation and dropped the old selection.
The window's envoy goes with the load, so the catalog reports a failed load and answers no languages.

## `private void QRepertoireFreshObserve()`

The rail's Fresh is answered by the Conduct, which decides what is made.
With no Situation chosen and no Entry shown, it opens the editor on a Situation nothing has stored yet.
With a Situation chosen, or an Entry shown, it starts a new Entry carrying that Situation instead.

## `private void QRepertoireBinObserve()`

Hands the rail's delete to the Conduct, which acts only while a Situation and not an Entry is shown.
The envoy asks the user first, and a Situation something references is asked about with its usage count.

## `private void QRepertoireStoreObserve()`

The rail's save, standing for whichever editor is in front.
The Conduct saves an open Entry through the entry editor, and otherwise commits the held Situation.

## `private void QRepertoireScribeObserve(bool scribe)`

Asks for the side the rail's toggle names, on whichever side the Conduct stands.
The Conduct asks before leaving an editor, so unsaved wording is never lost silently.

## `private void QRepertoirePressRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the print button is live, which holds while an entry or a situation is read.
It answers no before attach, because the command binding exists from the constructor on.
An editor on screen prints nothing, because what is printed is what is read.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QRepertoirePressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the entry being read, or else the situation being read, as the engine portrays it.
The gate asks for the ticket through the envoy and words the page through the engine.
The Conduct picks the page from the side it shows, and the engine builds it from stored rows.
Nothing is read back from the screen.

## `private void QRepertoirePortraitRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the export button is live, exactly when an entry is read in the display.
It answers no before attach, as the print check does.
Print may also act on the other page this panel reads, but export acts on entries alone.
The button follows this answer on its own, so no panel state has to switch it.

## `private async void QRepertoirePortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the entry being read, as the engine portrays it.
The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.
Nothing is read back from the screen.
