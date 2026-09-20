# LImprint.cs

## `public sealed class LImprint`

The deportment of the source editor.
It holds the desk over one Source draft, the credit rows, and the byline under them.
The fields are the engine's, so a typed field crosses raw.
The draft notice then writes back what the engine holds.
The credit rows are the engine's too, and the only state kept here is where the one blank row sits.
The byline is the popup of Authors the typed credit may already name.
It keeps the word typed and which offered row is lit.
The veneer names the row it serves on each act.
The unreadable seam is the window's sweep question, passed on to the desk.

## `public event Action? LImprintChanged;`

The credit rows moved, so the veneer copies them again.

## `public event Action? LImprintFocused;`

A blank row was asked for, so the veneer puts the caret into it.

## `public event Action? LImprintReverted;`

The typed credit was dropped, so the veneer writes the held name back into the focused field.

## `public event Action? LBylineChanged;`

The byline opened, moved or closed, so the veneer reads its rows and its lit index again.

## `private int _lImprintBlankAt = -1;`

The credit index the blank row stands at, or minus one while no row is being typed into.

## `public int LImprintBlankAt`

Where the blank row stands, which the veneer splices into the rows it copies.
It is read after the rows, since the read settles it against their count.

## `private int _lImprintCount;`

How many credits the draft held at the last read, kept so no local carries the count into a compare.

## `private int _lBylineCount;`

How many rows the byline offered at the last read, written only from that read.
The byline is open while a word is typed and this count is above zero.

## `public void LImprintCancel()`

Drops the held draft and closes the byline, then raises both notices so the veneer shows nothing.

## `public void LImprintSave()`

Stores the draft only when it differs from what is stored, so an idle store keeps the editor open.

## `public LReference LImprintReferenceRead(LDraft draft)`

The Source inside a draft notice, which the veneer writes its fields from.
The draft arrives as a parameter, so the Source is never a local in the veneer.

## `public string LImprintTallyRead()`

The citation sentence for the held Source, composed from the usage the engine counts.
A fresh draft has no stored id and counts as uncited.

## `public void LImprintKindSet(string? tag)`

The kind chosen in the chip menu, sent at once so it takes one chronicle step.
The kind already held is not sent again, so a repeated click leaves the chronicle alone.

## `private bool LImprintKindCheck(string tag)`

Whether the held Source already has the kind the tag names.

## `public IReadOnlyList<LAuthorRow> LImprintCreditRead()`

The credit rows the draft composes, with the blank row settled against their count.

## `private void LImprintBlankSet()`

An empty credit list on a held draft opens the blank row.
A new Source thus has a field to type into.
A blank row past the end moves to the end after a removal.

## `public void LImprintCreditApply(string? action, int? position, long? id)`

One of the four row buttons, named by the tag the template put on it.
Add opens a blank row under the clicked one, or focuses the blank row itself.
Remove closes the blank row, or sends the removal of a credit.
Earlier and Later send a shift by one.

## `private void LImprintCreditCommit(int at, long author, string? text)`

Enter on a credit field commits it.
An empty name reverts, and any other is sent with the credit the field stood for.
The engine matches the name, creates an Author when none matches, and replaces the former credit.
The blank row closes first, since the row it stood for is now a credit.

## `public bool LImprintKeyApply(string key, int? position, long? id, string? text, long? chosen)`

A key pressed in a credit field, named as the veneer reads it.
While the byline is open, Escape closes it and Up and Down move the lit row.
Enter then picks the lit row.
Otherwise Enter commits the typed name and Escape reverts it.
The lit row's Author arrives from the veneer, so no offered list is kept here.

## `public void LBylineWordSet(string? text, bool? focused)`

The text of a credit field changed.
A field without the keyboard is being written by the veneer, not the user, so it opens nothing.

## `public IReadOnlyList<LAuthor> LBylineRowsRead()`

The Authors the typed word matches, read from the engine on every notice and never stored.
The engine leaves out what the draft already credits, reading the draft by the desk's id.
An engine failure offers nothing rather than a dialog under a keystroke.

## `private void LBylineAdjust(int delta)`

Moves the lit row by one, wrapping at either end.
With no row lit, Down lights the first and Up lights the last.

## `public void LBylineSelect(long? id, int? position, long? held)`

A click on an offered row, with the row of the field behind it as the veneer read it.
A click beside a row, or one with no field behind it, only closes the byline.

## `private void LBylineCommit(int at, long author, long picked)`

Picking the Author the field already credits only reverts the typed text.
Any other is sent with the former credit, which the engine replaces.
