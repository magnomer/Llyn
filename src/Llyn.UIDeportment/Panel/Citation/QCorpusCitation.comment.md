# QCorpusCitation.cs
Hash: `46bbcb3158218f70`

## `internal sealed partial class QCorpus`

The citation field of the Examples page, written the way the card row writes its own.
The Source is typed, and the Sources answering the typed line stand in the drawer below.
A pick, or an Enter on a chosen row, cites it.
An Enter with no chosen row hands the typed line to one gate.
That gate may mint a Source titled with it.
The key handlers run in subscription order, one per role, as the card row's handlers do.

## `private void QCitationRefine()`

Writes the cited line the transcript last showed into the field, with the text handler unsubscribed.
A fill is not a keystroke, so it must not open the drawer.
The line arrives ready on the Example, so the field looks nothing up.

## `private void QCitationShutRefine()`

Shuts the drawer and shows the cited line again.

## `private void QCitationTextObserve(object sender, TextChangedEventArgs e)`

Hands the typed line to the drawer read and has the drawer paint the offer it answers.
Which rows, how many, how each is split and whether the drawer opens are the engine's to say.
A failing search answers an empty offer, so the drawer shuts rather than showing an error on every keystroke.

## `private void QCitationKeyRefine(object sender, KeyEventArgs e)`

While the drawer is shown, Escape shuts it and the arrows step its chosen row.
It only moves the look, so no gate is involved.

## `private void QCitationPickObserve(object sender, KeyEventArgs e)`

An Enter on the drawer's chosen row cites that Source by id through one gate.
The drawer's list holds no row while it is shut, so a shut drawer leaves Enter to the commit.

## `private void QCitationCommitObserve(object sender, KeyEventArgs e)`

An Enter the pick left unhandled hands the typed line to the typed-citation gate.
An empty line drops the citation but leaves the Source standing.

## `private void QCitationEscapeRefine(object sender, KeyEventArgs e)`

An Escape the drawer left unhandled drops the typed line for the cited line.

## `private void QCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field drops an uncommitted line and shows the cited line again.
A typed line that was never entered is not a citation.

## `private void QCitationPressObserve(object sender, MouseButtonEventArgs e)`

A press on a drawer row cites its Source by id through one gate.
It answers the pointer going down, since the popup takes the window's activation when pressed.
A pick of the citation already held raises no bulletin, so the field is redrawn here too.
