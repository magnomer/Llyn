# PCardTag.cs

## `internal sealed partial class PCard`

The Tags a card carries, held as the items of one field rather than as a line of text.
The collection is a run of committed Tags followed by one open entry.
That entry is the caret the user types into.
So the field can be drawn as boxed Tags with a cursor after them.
A Tag can hold spaces and punctuation without a separator being guessed at inside it.
Only a comma ends a Tag.

Duplicates are refused as they are committed rather than repaired afterwards.
So what the field shows is what the store will hold.

## `internal void PCardTagShow(IReadOnlyList<string> texts)`

Replaces the Tags with the stored ones of the card being loaded and reopens an empty entry after them.

## `internal IReadOnlyList<string> PCardTagRead()`

What the card says its Tags are.
That is the committed Tags, and the text still standing in the open entry after them.
So a Tag typed but never followed by a comma is not lost on save.
Reading does not change the field.
An unsaved-change check runs this while the user is still typing.
Closing a half-typed word into a Tag there would edit the card behind them.

## `internal void PCardTagRemove(PTagChip chip)`

Drops one Tag the user closed.

## `internal void PCardTagRemove()`

Drops the last Tag, which is what a backspace at the start of an empty entry means.
The caret is already past every Tag, so the one before it is the one being reached for.
Does nothing while the entry still holds text.

## `internal void PCardTagCommit()`

Closes what is standing in the entry into a Tag and clears the entry.
That is what leaving the field or pressing enter means.

## Inline notes

### `private void PCardTagChange(object? sender, PropertyChangedEventArgs arguments)`

The comma is read off the entry's text rather than off a keystroke.
So a comma arriving by paste ends a Tag exactly as a typed one does.
A paste of several commas leaves several Tags.
Everything before the last comma is committed and the remainder stays in the entry.
That is what makes the field reactive as it is typed into.

The guard around the rewrite is there because the rewrite sets the property being answered.
Without it the handler would answer itself.

### `private void PCardTagUpdate()`

The hint belongs to the empty field, not to the entry.
Once a card carries a Tag the entry sits beside it.
Prompting again would read as a second, unfilled field.
