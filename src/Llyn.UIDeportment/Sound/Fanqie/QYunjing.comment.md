# QYunjing.cs
Hash: `0893207efc19de56`

## `internal sealed class QYunjing`

The yunjing panel's driver, which browses the workspace as a rime table by onset and rime.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [CYunjing](../../../Llyn.Conduct/Panel/CYunjing.comment.md), and this file writes controls on notice.
It drives the Veneer page `PYunjing`, so the page holds only markup.
The category page, the reader and the editor are served from this file.
The two columns have their own driver, `QDiweiIndex`, and the entry list has its own driver, `QXiaoyun`.

## `internal QYunjing(UserControl surface)`

Takes the Veneer page the main window places and builds the category page driver.
It hands `PYunjingRail` to a `QPanelRail`, with the bin, the new-record button and the export button.
It hands `PYunjingOrder` and `PYunmuOrder` to two `QChoiceOrder`, whose menus hang under the onset and rime search bars.
It hands the page to the column driver `QDiweiIndex` and the entry list driver `QXiaoyun`.
It then subscribes the rail's notices.

## `private Border QLadder`

Each named part of the page is pulled through `QContract` under the page's own `x:Name`.

## `internal void QYunjingIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Builds the Conduct session, wraps its entry list's editor, and subscribes the notices.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
It hands the pickers the onset and rime apertures, titled `Ladder` and `Stair`, with the orders `CYunjingOrderRead` offers.
The column and entry list drivers are introduced with the session, so they subscribe their own rows.
The category page and the mode answer the area's change with their own Refines.
It then attaches the reader, the category page and the editor.
The rail is introduced with the atelier's navigation for its trail and the editor for its chronicle.
The print and portrait command bindings are added last, so no can-execute query meets a session not yet built.

## `private void QYunjingStoreRefine()`

Enables the rail's store button while the editor holds something storable.
It runs whenever the desk reports its state again.

## `internal void QYunjingVistaRefine()`

Answers the workspace's opening once the session restored its vistas and attached its observers.
It marks the ordering of both pickers and paints the mode and the entry list.
The category page and both columns answer the same opening with their own Refines.

## `private async void QYunjingWorkspaceRefine()`

Answers the area's workspace change by drawing the flags of the languages again.
The area has already let both columns and the chosen entry go.
Once the flags are in, it repaints the entry list, the only list whose rows carry a flag.
So rows built while the load ran, after a cell was chosen, gain their flags.
A failed flag load is reported by Conduct, and the rows are still painted without flags.
Its one request is the entry list's `CEntryListLoad`, which runs the flag fill and then answers the rows it paints.

## `internal void QYunjingExitRefine()`

Releases the editor's player as the window closes.
The area's own close, run by the atelier, lets the draft go and stops the playback.

## `private bool QYunjingShownCheck()`

Tells the session whether the page is on screen, since the session cannot see the window.

## `internal void QYunjingDiweiRefine()`

Hands the page its composed content, blank while it is hidden.
It answers the area's change, a cleared entry list and the workspace's opening.

## `private void QYunjingModeRefine()`

Writes the reader, editor and page visibility off the session's verdicts.
It hands the rail the scribe, mode and bin states.

## `private void QYunjingFreshObserve()`

The rail's new-record notice asks the panel for a new entry.

## `private void QYunjingScribeObserve(bool scribe)`

The rail's toggle hands its side to the scribe gate.

## `private void QYunjingStoreObserve()`

The rail's store notice asks the editor to save the entry.

## `private void QYunjingBinObserve()`

The rail's bin notice asks the panel to delete the entry.

## `private void QYunjingPressRefine(object sender, CanExecuteRoutedEventArgs e)`

Tells the print and portrait commands whether the panel allows them now.

## `private async void QYunjingPressObserve(object sender, ExecutedRoutedEventArgs e)`

The print command asks the session to print the portrait.

## `private async void QYunjingPortraitObserve(object sender, ExecutedRoutedEventArgs e)`

The portrait command asks the session to export the portrait.

## `private void QYunjingChronicleRefine()`

Lights the rail's two chronicle buttons only while the editor has a step to walk.
It runs whenever the editor reports its state again.
