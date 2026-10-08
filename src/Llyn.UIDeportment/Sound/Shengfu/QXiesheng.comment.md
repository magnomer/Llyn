# QXiesheng.cs
Hash: `1702227497750344`

## `internal sealed class QXiesheng`

The xiesheng panel browses the workspace by the phonetic series its characters belong to.
It is shown only while a loaded language pack declares a series source, since without one there is no series.
Every decision lives in [CXiesheng](../../../Llyn.Conduct/Panel/CXiesheng.comment.md), and this file writes controls on notice.
The series column, the reader and the editor are served from this file.
The entry list has its own driver, `QKindred`, and the series page has its own driver, `QStem`.

## `internal QXiesheng(UserControl surface)`

Takes the veneer page as its surface, builds the series page driver, and binds the print and export commands.
It hands `PXieshengRail` to a `QPanelRail`, with the bin, the new-record button and the export button.
It hands `PXieshengOrder` to a `QChoiceOrder`, whose menu hangs under the whole series search bar.
It hands the page to the entry list driver `QKindred`.
It attaches the series row fill and subscribes the series search field and the rail's notices.

## `private Border QRungBar`

Each named part of the page is pulled through `QContract.QContractFind`.

## `internal void QXieshengIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Builds the Conduct session and subscribes its notices.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The display view builds the lectern over the display that the session's entry list editor holds.
It hands the picker the series aperture, titled `Rung`, with the orders the session offers.
The column, the series page and the mode each answer the session's change with their own Refine.
The entry list driver is introduced with the session, so it subscribes its own rows.
It then attaches the reader, the series page and the editor.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.

## `private void QXieshengStoreRefine()`

Enables the rail's store button while the editor holds something storable.

## `internal void QXieshengVistaRefine()`

Answers the workspace's opening once the session restored its vistas and attached its observers.
It marks the picker's ordering and paints the mode and the entry list.
The column and the series page answer the same opening with their own Refines.

## `private async void QXieshengWorkspaceRefine()`

Answers the area's workspace change by drawing the flags of the languages again.
The area has already let the series and the chosen entry go.
Once the flags are in, it repaints the entry list, whose rows carry a flag.
So rows built while the load ran, after a series was chosen, gain their flags.
A failed flag load is reported by Conduct, and the rows are still painted without flags.
Its one request is the entry list's `CEntryListLoad`, which runs the flag fill and then answers the rows it paints.

## `private void QLodestarRefine()`

Answers the area's opening of a series a chip names by emptying the column's query field.
The area has already emptied the query, so the field only shows it.
The field's own handler still hears the change, and its gate finds the query already empty.

## `internal void QXieshengExitRefine()`

Releases the editor's recording player as the window exits.
The session's close already let the draft go and stopped the playback.

## `private bool QXieshengShownCheck()`

True while the panel is the visible tab.

## `internal void QGroveRefine()`

Copies the series column and its empty line again.

## `internal void QXieshengStemRefine()`

Hands the page of the chosen series to the series page driver.
It also answers the panel's clearing, while the lectern empties the reader itself.

## `private void QXieshengModeRefine()`

Shows the reader, the page or the editor.
The rail folds its own button pairs and enables the mode and bin buttons.

## `private void QLodestarObserve(object sender, TextChangedEventArgs e)`

Narrows the series column as the field is typed into.

## `private void QGroveObserve(object sender, RoutedEventArgs e)`

Chooses the series of the pressed row.

## `private void QXieshengFreshObserve()`

Starts a fresh entry in the editor.

## `private void QXieshengScribeObserve(bool scribe)`

Hears the rail's mode toggle and swaps the read view and the editor through the shared panel.
`scribe` is true for the editor and false for the read view.

## `private void QXieshengStoreObserve()`

Saves what the editor holds.

## `private void QXieshengBinObserve()`

Deletes the entry the panel holds.

## `private void QXieshengPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Allows printing and exporting only while an entry is read.
It answers no before the panel is attached, since the commands are bound at construction.

## `private async void QXieshengPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the read entry on the printer the gate asks for.

## `private async void QXieshengPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

Exports the read entry as a portrait file.

## `private void QXieshengChronicleRefine()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
