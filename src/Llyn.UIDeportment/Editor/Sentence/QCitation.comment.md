# QCitation.cs
Hash: `7dee7a05a62658f5`

## `internal sealed class QCitation`

The citation field of an Example row.
It decides what a typed line becomes, and how rows show the Source they cite.
A citation is either a stored Source or nothing, so the field never keeps a line that matches no Source.
One list of Sources serves every row on the form.

## `internal QCitation(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation, QSentence sentence, QProffer proffer)`

Holds the two card lists, the sentence driver that finds a row's card, and the dropdown.

## `internal ObservableCollection<QCitationItem> QCitationCatalog { get; }`

The Sources offered, which every card is built with.

## `internal void QCitationIntroduce(CCard card, CSentence sentence, CAtelier atelier)`

Holds the card facet, whose gates find and set a citation.
The list is refilled when the sentence facet's Sources change and when a workspace opens.

## `internal void QCitationApply(FrameworkElement container)`

Hooks a row's citation field.
Its typing goes to this driver's field refine.
The dropdown's key handlers come first, then this driver's commit and escape, and its leave.
Each event is hooked once, so a refill leaves one handler.

## `private void QCitationFieldRefine(object sender, TextChangedEventArgs e)`

Finds the row's card and opens the Source dropdown with the Sources the card gate finds for the typed text.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.

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
