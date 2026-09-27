# QCorpusCitation.cs

## `internal sealed partial class QCorpus`

The citation field of the Examples page, written the way the card row writes its own.
The Source is typed, and the Sources answering the typed line stand in the drawer below.
A pick, or an Enter on a chosen row, cites it.
An Enter with no chosen row hands the line to the deportment.
The deportment may mint a Source titled with it.

## `private void QCitationFind()`

Reads the whole shelf of Sources the citation may point at, for the names the page shows.
Nothing here creates, changes or deletes a Source.

## `private string QCitationNameRead(long id)`

The name a cited Source is shown under, falling back to its id when it names itself nowhere.
The field and the excerpt both read a Source through this, so both agree.

## `private void QCitationUpdate()`

Writes the cited name into the field with the text handler unsubscribed.
A fill is not a keystroke, so it must not open the drawer.

## `private void QCitationTextHandle(object sender, TextChangedEventArgs e)`

Hands the typed line to the deportment, which answers with the rows the drawer shows.
Which rows, how many and how each is split are the deportment's to say.
A failing search shuts the drawer rather than showing an error on every keystroke.

## `private void QCitationKeyHandle(object sender, KeyEventArgs e)`

Reads the key, the drawer's state and its chosen row, and hands all three to one run.

## `private bool QCitationKeyRun(Key key, bool shown, long? chosen)`

The keys the field answers, as the entry editor's candidate list answers them.
While the drawer is shown, Escape shuts it, the arrows step it, and Enter cites the chosen row.
Otherwise Enter commits the typed line and Escape drops it for the cited name.
The drawer's state arrives as parameters, so no driver field decides a request.

## `private void QCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field drops an uncommitted line and shows the cited name again.
A typed line that was never entered is not a citation.

## `private void QCitationPressHandle(object sender, MouseButtonEventArgs e)`

A press on a drawer row cites its Source.
It answers the pointer going down, since the popup takes the window's activation when pressed.

## `private void QCitationSet(long reference)`

The pick goes to the text gate, and the draft bulletin redraws the field.
A pick of the citation already held raises no bulletin, so the field is redrawn here too.

## `private void QCitationCommit()`

Hands the typed line to the deportment, which resolves it against the held draft in one request.
An empty line drops the citation but leaves the Source standing.

## `private void QCitationHide()`

Shuts the drawer and shows the cited name again.
