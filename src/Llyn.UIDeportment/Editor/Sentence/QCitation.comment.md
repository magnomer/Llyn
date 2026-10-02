# QCitation.cs
Hash: `3d9e7f3ce263259c`

## `internal sealed class QCitation`

The citation field of an Example row.
It decides what a typed line becomes, and how rows show the Source they cite.
A citation is either a stored Source or nothing, so the field never keeps a line that matches no Source.
One list of Sources serves every row on the form, Example and Situation alike.

## `internal QCitation(`

Holds the two card lists, the sentence driver that finds a row's card, and the dropdown.

## `internal ObservableCollection<QCitationItem> QCitationCatalog { get; }`

The Sources offered, which every card is built with.

## `internal void QCitationIntroduce(CEditor editor, CAtelier atelier)`

Holds the editor area.
The list is refilled when the Sources change and when a workspace opens.

## `internal void QCitationApply(FrameworkElement container)`

Hooks a row's citation field.
The dropdown's key handlers come first, then this driver's commit and escape, and its leave.
Each event is hooked once, so a refill leaves one handler.

## `internal void QCitationFieldRefine(PCard card, PSentence row, TextBox box)`

Opens the Source dropdown for the typed text with the Sources the card gate finds.

## `private void QCitationCatalogRefine()`

Refills the Source list and has every row show its cited Source again.

## `private void QCitationCommitObserve(object sender, KeyEventArgs e)`

Hears enter in the citation field and hands the typed line to the card gate, which shows its own failure.
The dropdown's own key handlers hear each key first, so a key the open dropdown takes never reaches here.

## `private void QCitationEscapeRefine(object sender, KeyEventArgs e)`

Puts the cited Source's text back in the field and shuts nothing else.

## `private void QCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)`

Shuts the dropdown and puts the cited Source's text back, so a typed line matching no Source does not stay.

## `private static void QCitationTextRefine(TextBox box)`

Shows the Source the row cites.
