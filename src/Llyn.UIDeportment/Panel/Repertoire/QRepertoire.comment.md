# QRepertoire.cs
Hash: `f145a14ec587ce1f`

## `internal sealed partial class QRepertoire : QChronicleHost`

The Repertoire panel's driver, the view of the shared stock of usage contexts itself.
A Situation is independent data owned by nothing, so this panel is not a view of one Entry's contexts.
It holds the surface, the host and the repertoire Conduct the browsing side calls, and nothing else.
The browsing behavior lives in `QRepertoireBrowse.cs` and the editing in `QRepertoireEditor.cs`, one file per responsibility.
The held draft the editor writes into lives in `QRepertoireHold.cs`, apart from the controls it reads.
`QRepertoireDialog.cs` answers the picture and video row clicks.
It answers the window's undo and redo keys as `QChronicleHost`, attached to its surface since the surface is no driver.

## `internal QRepertoire(UserControl surface)`

Takes the veneer's page as its surface, which the window pulls by contract ID.
The page's local styles are handed to the look sheet, which otherwise knows only the application's.
The driver attaches itself to the page as the host of the undo and redo keys.
It adds the print and export command bindings and points the two buttons at them.
It ties both droppers to their popups, sets every icon, and subscribes every click and text field.

## `private Border QTier`

Each named part of the page is pulled through `QContract.QContractFind` by its contract ID.

## `private CRepertoire _cRepertoire = null!;`

The repertoire Conduct, holding the atlas, the occurrence list, the desk, the session and the panel's mode.
The atlas's vista carries the order, the inquest, and the languages hidden from the entry column.
The panel keeps no copy of any of them and asks the Conduct for each where it needs it.
It is null until `QRepertoireIntroduce` builds it, so only the print and portrait gates guard against that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `internal void QRepertoireIntroduce(QWindow host)`

Builds the repertoire Conduct, which builds its editor, and wires the desk's notices.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The lectern follows the occurrence panel, whose loads and clears reach the display's area, never the veneer.
Binds the panel to the window it asks for confirmations and panel switches through.
It binds its lists, the editor's media rows among them, and subscribes to the engine, and reads nothing yet.
The atlas and occurrence lists get their row fills through `QLookItemAttach`.
The picture and video drivers are handed the repertoire's own image and video gates.
It attaches the entry display and editor to the same host and engine, so an Entry is read and written.
The engine's change notices drive the mode, and its row notices drive the two lists.
Its scenario and situation notices paint the sheet.
Its failures reach the window through the envoy, which the Conduct asks directly.
A dropped inquest empties the inquest box and the language menu.
A workspace change reloads the flags.
The ordering menu is built once from the Conduct's fixed list of orderings.
The window fills the situation catalog and the entry column when the workspace opens.
Every change after that arrives as an announcement.
The panel is current whether or not its tab is in front.

## `private bool QRepertoireShownCheck()`

Whether the panel is on screen, so the engine knows when a notice needs painting.

## `private void QRepertoireModeRefine()`

Paints the mode the engine decides.
It shows the page, checks the toggle and lights the buttons the mode names.
The scenario is live only while its desk runs.
It folds the trail pair and the chronicle pair by the same mode as the toggle.
It ends by refreshing the rail's undo and redo.

## `internal void QRepertoireExitRefine()`

The Veneer half of the window's exit, since the editor's stop and the playback cancel run in Conduct.
Releases the editor's recording player and closes the popups the panel owns, so neither outlives the window.

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
