# PImprintField.cs

## `public partial class PImprint`

The stated fields of the Source edit area: the fields, the save, the discard, and the delete.
The credits are a responsibility of their own and live in `PImprintAuthor.cs`.
The control owns its edit area, so it needs no dispatcher pair as the shared `PEditor` does.
What is typed here reaches the held draft through `PImprintHold.cs`, which is where every push and every answer lives.
The area no longer keeps a copy of the stored Source to compare itself against.

## Inline notes

### `private bool _pImprintTitleUnreadable;`

Whether the title stands recorded as unknown rather than merely blank.
One flag per field is the only per-field state the edit area holds, as the built panels do it.

## `private void PImprintMarkClear(ref bool unreadable, TextBox field, ToggleButton mark)`

Typing into a field drops its unknown mark, because a typed value is a stated one.
The guard keeps a programmatic fill from reading as a user edit.
Every edit that passes the guard starts the wait that ends in a push.

## `private void PImprintUnknownHandle(object sender, RoutedEventArgs e)`

Records one field as unknown, or takes that record back.
It is the same control over every text field rather than one control each.
The value is cleared with the mark, because a value is written only while the state is specified.
Marking a field is an edit like any other, so it too is pushed once the typing stops.

## `private void PImprintApply(LReference? reference)`

Fills the whole edit area from the held Source, or empties it when no draft is held.
The mark beside each field is set from the field's state, so the three states arrive distinct.
The delete control and the citation figure follow the stored Source the draft names.
Neither means anything before a store.
The buttons are settled last, so the area and what they say never drift apart.

## `private void PImprintCountShow(string? stored)`

Keeps the citation figure visible while editing.
A user correcting a title must see how many places that correction reaches without leaving the area.

## `private LReference PImprintRead(LReference held)`

Reads the edit area back onto the Source being held.
Identity comes from the held Source rather than from the area, which knows nothing about it.
The author state comes from the credits region, because it is a column on the same row.

## `private void PImprintDiscardHandle(object sender, RoutedEventArgs e)`

Throws the held Source away and opens a fresh one on the same stored Source.
Restarting rather than refilling leaves nothing of the discarded work on disk.

## `private void PImprintStoreHandle(object sender, RoutedEventArgs e)`

Commits the held Source and asks the panel to read over what was stored.
Typing still waiting to be pushed is pushed first, so the commit carries the last keystrokes.
The engine announces the commit and the shelf is read again on that.
The stored Source refreshes its own citation figure.
The panel no longer re-reads straight after its own commit, because it hears about the commit like everything else.
Saving changes neither the identifier nor where the Source is cited.

## `private void PImprintRemovalHandle(object sender, RoutedEventArgs e)`

Deletes an uncited Source, or asks before detaching every citation and deleting a cited one.
The detaching and the delete are one store call, because between two steps the citation count can change.
