# QGuild.cs
Hash: `b3ddca943a3a02d3`

## `internal sealed class QGuild : QChronicleHost`

The driver of the authors panel: the workspace browsed by the people its Sources credit.
It is the sources panel's shape read through a different question, so it holds the same three columns.
Every decision lives in Conduct's `CGuild`, and this driver writes controls on notice.
The roll is served here, and the oeuvre, the vita and the autograph have drivers of their own.
The rail, the order picker and the kind filter are shared drivers, built here and fed from Conduct.
It is the chronicle host its own rail walks, so the rail's undo and redo reach the session.
It registers no surface with `QChronicle`, because only its rail asks it to walk.

## `internal QGuild(UserControl surface)`

Takes the guild page as its surface and builds a driver for each nested page.
The nested pages are the vita, the autograph, the colophon and the oeuvre.
It builds the rail with Fresh shown and Portrait hidden, since no Entry is read here.
The order picker hangs its menu under the whole `PEchelon` bar.
It subscribes the roll search field, the rail's four notices and the roll's clicks.
It attaches the roll fill.

## `private Border QEchelon`

Each part of the page is pulled by its contract ID through `QContract.QContractFind`.

## `internal void QGuildIntroduce(CAtelier atelier, CEnvoy envoy)`

Builds the panel's Conduct with the window's envoy, subscribes its notices, and wires the roll.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the Conduct guild to the vita, the autograph and the oeuvre drivers, which answer their own notices.
The vita also takes the atelier's navigation, which its citation click opens a usage through.
The panel itself attaches its autograph desk's bulletins and its vista observers.
The rail gets the window's navigation for its trail and this driver as its chronicle host.
The order menu is built here once, from the orderings Conduct offers.
The roll's aperture feeds both the order picker and the kind filter.
The print command binding is added last, so no can-execute query ever meets a panel not yet built.

## `internal void QGuildVistaRefine()`

Answers the workspace opening, after Conduct has restored the vistas and their observers.
It only paints.
It marks the held order, builds the kind menu from the held filter, and shows the filter mark.
It reads the roll once, and that answer carries the kind menu too.
It then lists the roll from the same answer.
The oeuvre answers the same opening with its own Refine.
The window subscribes the colophon's tally to the same opening after it.
The search fields keep their text, since Conduct carried the queries into the fresh vistas.

## `internal void QGuildClose()`

Closes both pickers' menus, so nothing stays open over a window that is going.

## `private bool QGuildShownCheck()`

Tells the panel whether the guild page is on screen, so a bulletin is acted on only while shown.

## `private void QRollRefine()`

Reads the roll answer once and hands it to `QRollShow`.
It answers the roll's rows notice, so it takes the `Refine` ending.

## `private void QRollShow(CGuildRoll roll)`

Paints the one roll answer: the rows, the empty notice, the vita and the autograph's count chips.
The vita comes with the rows, so the same Author feeds both without a second read.
The workspace opening hands it the roll it already read.

## `internal void QTallyRefine()`

Refreshes the colophon's tally whenever the oeuvre rows change.
It also answers the workspace opening, so a fresh workspace shows no stale tally.

## `private void QGuildModeUpdate()`

Writes every visibility and enablement off the panel's verdicts.
The two nested pages are pulled by contract ID, since their drivers own only what lies inside them.
The rail takes the scribe verdict, the mode and bin enablement, and the store verdict.

## `private void QMusterObserve(object sender, TextChangedEventArgs e)`

Hands the typed roll search to the query gate.

## `private void QRollObserve(object sender, RoutedEventArgs e)`

Hands the clicked Author's id to the gate, which records the station.

## `private void QGuildFreshObserve()`

Asks the gate for a new Author when the rail's Fresh is clicked.

## `private void QGuildScribeObserve(bool scribe)`

Asks the mode gate for the side the rail's toggle names.

## `private void QGuildStoreObserve()`

Asks the session to save.

## `private void QGuildBinObserve()`

Asks the gate to delete the chosen Author.

## `private void QGuildPressCheck(object sender, CanExecuteRoutedEventArgs e)`

Allows the print command only while the gate says the colophon side is shown.

## `private async void QGuildPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the Source chosen in the oeuvre on the printer the gate asks for.
The gate words the page with the Source legend.

## `public void QChronicleUndoObserve()`

Walks the autograph session back one step, with the caret kept where it was.
The rail's backward button calls it through `QChronicleHost`.

## `public void QChronicleRedoObserve()`

Walks the autograph session forward one step, the mirror of the undo.

## `private void QGuildChronicleUpdate()`

Lights the rail's two chronicle buttons only while the session has a step to walk.
It runs whenever the desk reports its state again.
