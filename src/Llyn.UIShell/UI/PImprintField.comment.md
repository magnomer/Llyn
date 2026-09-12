# PImprintField.cs

## `public partial class PImprint`

The stated fields of the Source edit area: the fields, the save, the discard, and the delete.
The credits are a responsibility of their own and live in `PImprintAuthor.cs`.
The control owns its edit area, so it needs no dispatcher pair as the shared `PEditor` does.
What is typed here reaches the held draft through `PImprintHold.cs`, which is where every push and every answer lives.
The area no longer keeps a copy of the stored Source to compare itself against.

## Inline notes

### `private bool _pImprintTitleUnknown;`

Whether the title stands recorded as unknown rather than merely blank.
One flag per field is the only per-field state the edit area holds, as the built panels do it.

## `private void PImprintMarkClear(ref bool unknown, TextBox field, ToggleButton mark)`

Typing into a field drops its unknown mark, because a typed value is a stated one.
The guard keeps a programmatic fill from reading as a user edit.
Every edit that passes the guard starts the wait that ends in a push.

## `private void PImprintUnknownHandle(object sender, RoutedEventArgs e)`

Records one field as unknown, or takes that record back.
It is the same control over every text field rather than one control each.
The value is cleared with the mark, because a value is written only while the state is specified.
Marking a field is an edit like any other, so it too is pushed once the typing stops.

## `private void PImprintApply(LDraft? draft)`

Fills the whole edit area from the held draft, or empties it when none is held.
The credits come from the same draft, so the area is drawn from one reading.
The mark beside each field is set from the field's state, so the three states arrive distinct.
The delete control and the citation figure follow the stored Source the draft names.
Neither means anything before a store.
The buttons are settled last, so the area and what they say never drift apart.

## `private void PImprintShow(LDraft draft)`

Redraws the area from the held draft only where a control says something else.
This is what the draft bulletin does, so a keystroke echoed back never moves the caret.

## `private static void PImprintFieldShow(`

Writes one field and its mark from a value.
With `differing` set it first checks whether the field already reads that value, and leaves it alone when it does.

## `private void PImprintCountShow(long? stored)`

Keeps the citation figure visible while editing.
A user correcting a title must see how many places that correction reaches without leaving the area.

## `private LRequestReferenceBody PImprintRead(long draft)`

The edit area as written, for the engine to read into the Source it holds.
Each text field travels with its mark, so the area resolves no state.
The engine keeps the id of the Source it holds, so the area never learns or carries one.
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
