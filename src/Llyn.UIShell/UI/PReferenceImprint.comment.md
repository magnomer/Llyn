# PReferenceImprint.cs

## `public partial class PReference`

The edit area of the Source panel: the five stated fields, the save, the discard, and the delete.
The credits are a responsibility of their own and live in `PReferenceAuthor.cs`.
The panel owns its edit area, so it needs no dispatcher pair as the shared `PEditor` does.

## Inline notes

### `private bool _pImprintTitleUnreadable;`

Whether the title stands recorded as unknown rather than merely blank.
One flag per field is the only per-field state the edit area holds, as the built panels do it.

## `private void PImprintMarkClear(ref bool unreadable, TextBox field, ToggleButton mark)`

Typing into a field drops its unknown mark, because a typed value is a stated one.
The guard keeps a programmatic fill from reading as a user edit.

## `private void PImprintUnknownHandle(object sender, RoutedEventArgs e)`

Records one field as unknown, or takes that record back.
It is the same control over five fields rather than five different ones.
The value is cleared with the mark, because a value is written only while the state is specified.

## `private void PImprintApply(LReference? reference)`

Fills the whole edit area from one stored Source, or empties it for a new one.
The mark beside each field is set from the field's state, so the three states arrive distinct.

## `private void PImprintCountShow(LReference? reference)`

Keeps the citation figure visible while editing.
A user correcting a title must see how many places that correction reaches without leaving the area.

## `private LReference PImprintRead()`

Reads the edit area back into a record.
The author state comes from the credits region, because it is a column on the same row.

## `private bool PImprintChangeCheck()`

Whether anything in the edit area differs from what the store holds.
The credit list is never compared, because credits are written when they are made.

## `private void PImprintStoreHandle(object sender, RoutedEventArgs e)`

Writes the Source and returns to the reading over what was stored.
Saving changes neither the identifier nor where the Source is cited.

## `private void PImprintRemovalHandle(object sender, RoutedEventArgs e)`

Deletes an uncited Source, or asks before detaching every citation and deleting a cited one.
The detaching and the delete are one store call, because between two steps the citation count can change.
