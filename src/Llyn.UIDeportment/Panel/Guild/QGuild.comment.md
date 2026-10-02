# QGuild.cs

## `internal sealed class QGuild`

The driver of the authors panel: the workspace browsed by the people its Sources credit.
It is the sources panel's shape read through a different question, so it holds the same three columns.
Every decision lives in Conduct's `CGuild`, and this driver writes controls on notice.
The roll and the oeuvre are served here, and the vita and the autograph have drivers of their own.

## `internal QGuild(UserControl surface)`

Takes the guild page as its surface and builds a driver for each of its two nested pages.
It points the print button at its command, ties both droppers to their popups, and sets every icon.
It subscribes every click, both search fields and both list clicks, and attaches the roll and oeuvre fills.

## `private Border QEchelon`

Each part of the page is pulled by its contract ID through `QContract.QContractFind`.

## `internal void QGuildIntroduce(QWindow host)`

Builds the panel's Conduct with the window's envoy, subscribes its notices, and wires the two lists.
Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
It hands the panel to the vita and the autograph drivers, which answer their own notices.
The panel itself attaches its autograph desk's bulletins and its vista observers.
The order menu is built here once, from the orderings Conduct offers.
The print command binding is added last, so no can-execute query ever meets a panel not yet built.

## `internal void QGuildVistaRefine()`

Answers the workspace opening, after Conduct has restored the vistas and their observers.
It only paints.
It marks the held order, builds the kind menu from the held filter, and shows the filter mark.
It then lists the roll once.
The window subscribes the oeuvre's rows and the colophon's tally to the same opening, one member per list.
The search fields keep their text, since Conduct carried the queries into the fresh vistas.

## `internal void QGuildClose()`

Closes the dropdowns, so nothing stays open over a window that is going.

## `private void QLouverBuild()`

Builds the kind menu from the filter the roll now holds, since a fresh vista holds none.

## `private void QLouverRefine()`

Shows the filter mark while the roll is filtered.

## `private void QEchelonDropperRefine()`

Unchecks the order dropper so its popup closes after a pick.

## `private void QRollRefine()`

Paints the one roll answer: the rows, the empty notice, the vita and the autograph's count chips.
The vita comes with the rows, so the same Author feeds both without a second read.
It answers the roll's rows notice and the workspace opening, so it takes the `Refine` ending.

## `internal void QOeuvreRefine()`

Lists the oeuvre afresh with its empty text.
It answers the oeuvre's rows notice and the workspace opening, so it took the `Refine` ending.

## `internal void QTallyRefine()`

Refreshes the colophon's tally whenever the oeuvre rows change.
It also answers the workspace opening, so a fresh workspace shows no stale tally.

## `private void QGuildModeUpdate()`

Writes every visibility and enablement off the panel's verdicts.
The two nested pages are pulled by contract ID, since their drivers own only what lies inside them.
The voyage and chronicle pairs follow the viewer and scribe verdicts.

## `private void QMusterObserve(object sender, TextChangedEventArgs e)`

Hands the typed roll search to the query gate.

## `private void QCombObserve(object sender, TextChangedEventArgs e)`

Hands the typed oeuvre search to the oeuvre's query gate.

## `private void QEchelonObserve(object sender, RoutedEventArgs e)`

Hands the picked ordering to the order gate, then closes the dropper.

## `private void QLouverObserve(object sender, RoutedEventArgs e)`

Hands the ticked kinds to the filter gate, then shows the filter mark.

## `private void QRollObserve(object sender, RoutedEventArgs e)`

Hands the clicked Author's id to the gate, which records the station.

## `private void QOeuvreObserve(object sender, RoutedEventArgs e)`

Hands the clicked Source's id to the gate.

## `private void QGuildFreshObserve(object sender, RoutedEventArgs e)`

Asks the gate for a new Author.

## `private void QGuildViewerObserve(object sender, RoutedEventArgs e)`

Asks the mode gate for the reading side.

## `private void QGuildScribeObserve(object sender, RoutedEventArgs e)`

Asks the mode gate for the writing side.

## `private void QGuildStoreObserve(object sender, RoutedEventArgs e)`

Asks the session to save.

## `private void QGuildBinObserve(object sender, RoutedEventArgs e)`

Asks the gate to delete the chosen Author.

## `private async void QGuildPressObserve(object sender, ExecutedRoutedEventArgs e)`

Prints the Source read in the colophon on the printer the gate asks for.
The gate words the page with the Source legend.

## `internal void QGuildVoyageShow(bool past, bool future)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the panel only shows what it is told.

## `private void QGuildRetreatObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail back one station.

## `private void QGuildAdvanceObserve(object sender, RoutedEventArgs e)`

Steps the navigation's trail forward one station.

## `private void QGuildUndoObserve(object sender, RoutedEventArgs e)`

Walks the autograph desk back one step, with the caret kept where it was.

## `private void QGuildRedoObserve(object sender, RoutedEventArgs e)`

Walks the autograph desk forward one step, the mirror of the undo.

## `private void QGuildChronicleUpdate()`

Lights the two chronicle buttons only while the desk has a step to walk.
It runs whenever the desk reports its state again.
