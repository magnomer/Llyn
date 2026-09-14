# PCorpusCitation.cs

## `public partial class PCorpus`

The citation field of the Examples page, written the way the card row writes its own.
The Source is typed, and the Sources answering the typed line stand in a list below.
A pick, or an Enter on a line that names a Source, cites it.
An Enter on a line no Source answers mints a Source titled with that line.

## `private void PCitationFind()`

Reads the whole shelf of Sources the citation may point at.
Nothing here creates, changes or deletes a Source.

## `private string PCitationNameRead(long id)`

The name a cited Source is shown under, falling back to its id when it names itself nowhere.
The catalog, the search and the display all read a Source through this, so all three agree.

## `private void PCitationUpdate()`

Writes the cited name into the field while a guard holds the text handler off.
A fill is not a keystroke, so it must not open the list.

## `private void PCitationTextHandle(object sender, TextChangedEventArgs e)`

Only a line the user is typing opens the list, never one the page put there.

## `private void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)`

Leaving the field drops an uncommitted line and shows the cited name again.
A typed line that was never entered is not a citation.

## `private void PCitationCommit()`

An empty line drops the citation.
The Source is left standing, because clearing a pointer is not deleting what it pointed at.
A line matching a known name, ignoring case, cites that Source rather than minting a twin.

## `private void PCitationShow(string text)`

Rows carry the `Author (Year)` byline and are ordered by how many rows already cite them.
A line equal to the cited name opens nothing, since the row already cites it.
The list stays shut when nothing matches, rather than standing empty.

## `private static CustomPopupPlacement[] PCitationPlace(Size popup, Size target, Point offset)`

The list hangs under the field's frame, and flips above it when the window ends first.
The shade is pulled back so the frame edges line up with the field.
