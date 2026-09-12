# PCardContext.cs

## `internal sealed partial class PCard`

The Situations a card carries, held as the items of one field rather than as a stack of rows.
The collection is a run of committed Situations with one open entry among them.
That entry is the caret the user types into, and it moves between Situations as a caret does.
So the field can be drawn as boxed Situations with a cursor after them, the way Tags are.
A Situation can hold spaces and punctuation without a separator being guessed at inside it.
Only a comma ends a Situation.

Duplicates are refused as they are committed rather than repaired afterwards.
So what the field shows is what the store will hold.

## `internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)`

Replaces the Situations with the stored ones of the card being loaded and reopens an empty entry after them.

## `internal IReadOnlyList<LSituationDraft> PCardContextRead()`

What the card says its Situations are.
That is the committed Situations and the entry text, each in the order the field shows them.
So a Situation typed but never followed by a comma is not lost on save.
Reading does not change the field.
An unsaved-change check runs this while the user is still typing.
Closing a half-typed wording into a Situation there would edit the card behind them.

## `internal void PCardContextApply(IReadOnlyList<LSituationDraft> stored)`

Writes the ids the engine minted back onto the chips, in the order the read listed them.
A caret holding unfinished text counted as a row in the read, so it is stepped over here.

## `internal bool PCardContextCommit(string id, string title)`

Attaches a stored Situation the user chose from the dropdown, under the id it is stored beneath.
A Situation the card already carries is refused, whether it is met by id or by wording.

## `internal void PCardContextClear()`

Empties the entry without reading it as a further edit.

## `internal bool PCardContextMatch(string id)`

Whether the card already carries the stored Situation, which is what keeps the dropdown from offering it twice.

## `internal void PCardContextRemove(PContext chip)`

Drops one Situation the user closed.

## `internal void PCardContextRemove(int step)`

Drops the Situation standing one step from the entry, before it or after it.
That is what a backspace at the start, or a delete at the end, reaches for.

## `internal bool PCardContextMove(int step)`

Steps the entry one place along the field, past the Situation on that side.
A Situation is passed over as a single character would be.
Reports whether there was anywhere left to go.

## `internal void PCardContextCommit()`

Closes what is standing in the entry into a Situation and clears the entry.
That is what leaving the field or pressing enter means.

## Inline notes

### `private void PCardContextChange(object? sender, PropertyChangedEventArgs arguments)`

Every edit of the entry is reported.
The editor can offer the Situations the wording matches as it is typed.
The card cannot read the workspace itself, so it says what was typed and the editor answers.

The comma is read off the entry's text rather than off a keystroke.
So a comma arriving by paste ends a Situation exactly as a typed one does.
A paste of several commas leaves several Situations.
Everything before the last comma is committed and the remainder stays in the entry.

The guard around the rewrite is there because the rewrite sets the property being answered.
Without it the handler would answer itself.

### `private bool PCardContextCheck(string text)`

An unreadable Situation shows no wording, so it is never counted as a duplicate.
Two of them on one card stand for two Situations the store could not read.

### `private void PCardContextUpdate()`

The hint belongs to the empty field, not to the entry.
Once a card carries a Situation the entry sits beside it.
Prompting again would read as a second, unfilled field.
