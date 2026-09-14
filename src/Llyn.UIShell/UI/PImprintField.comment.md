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

## `private void PImprintMarkClear(ref bool unknown, TextBox field)`

Typing into a field drops its unknown mark, because a typed value is a stated one.
The guard keeps a programmatic fill from reading as a user edit.
Every edit that passes the guard starts the wait that ends in a push.

## `private string PImprintHintRead(TextBox field, bool unknown)`

The placeholder a field shows while empty.
A never-written field asks for its value, and an unknown one reads the unknown mark until typing clears it.
No control records a field unknown, as no other edit area offers one.
The mark only arrives with the stored value.

## `private void PImprintKindHandle(object sender, RoutedEventArgs e)`

A kind chosen from the chip's list, written into the chip and pushed like any other edit.
The list closes on any choice, and a choice already standing changes nothing.

## `private void PImprintApply(LDraft? draft)`

Fills the whole edit area from the held draft, or empties it when none is held.
The credits come from the same draft, so the area is drawn from one reading.
The mark beside each field is set from the field's state, so the three states arrive distinct.
The tally chip follows the stored Source the draft names, which is cited nowhere before a store.
The rail's save is settled last, so the area and what it says never drift apart.

## `private void PImprintShow(LDraft draft)`

Redraws the area from the held draft only where a control says something else.
This is what the draft bulletin does, so a keystroke echoed back never moves the caret.

## `private void PImprintKindShow(LReferenceKind kind)`

Writes the kind into its chip and marks it in the list, building the list on first use.
The rows read their names by resource, so a language change renames them without a rebuild.

## `private void PImprintFieldShow(TextBox field, LStateValue? value, ref bool held, bool differing = false)`

Writes one field and its held mark from a value.
With `differing` set it first checks whether the field already reads that value, and leaves it alone when it does.

## `internal void PImprintTallyShow()`

Keeps the citation count visible while editing.
A user correcting a title must see how many places that correction reaches without leaving the area.
The panel calls it after every shelf fill, so a citation added elsewhere shows at once.

## `private LRequestReferenceBody PImprintRead(long draft)`

The edit area as written, for the engine to read into the Source it holds.
Each text field travels with its mark, so the area resolves no state.
The engine keeps the id of the Source it holds, so the area never learns or carries one.
The author state comes from the credits region, because it is a column on the same row.

## `internal void PImprintStoreRun()`

Commits the held Source and asks the panel to read over what was stored.
The rail's save calls it through the panel, because the area carries no save of its own.
There is no discard: leaving the area through the mode toggle asks about the draft instead.
Typing still waiting to be pushed is pushed first, so the commit carries the last keystrokes.
The engine announces the commit and the shelf is read again on that.
The stored Source refreshes its own citation figure.
The panel no longer re-reads straight after its own commit, because it hears about the commit like everything else.
Saving changes neither the identifier nor where the Source is cited.
A form halted by a failed flush commits nothing.
The draft would otherwise be stored missing the edits the flush dropped.

