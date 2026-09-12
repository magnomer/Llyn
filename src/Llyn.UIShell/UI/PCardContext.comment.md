# PCardContext.cs

## `internal sealed partial class PCard`

The Situations a card shows, held as the items of one field rather than as a stack of rows.
The collection is a run of Situation chips with one open entry among them.
That entry is the caret the user types into, and it moves between chips as a caret does.
So the field can be drawn as boxed Situations with a cursor after them, the way Tags are.
A Situation can hold spaces and punctuation without a separator being guessed at inside it.
Only a comma ends a Situation.

The engine holds the chips.
The card renders them by id, and every commit or removal is a request the editor sends.
Where the caret stands is the card's own, and it is the position a new chip is asked for.
Duplicates are refused before a request is sent, so what the field shows is what the engine will hold.

## `internal Action<string>? PCardContextNotice { get; set; }`

Where the entry's text goes as it is typed, so the editor can offer matching Situations.

## `internal Func<string, bool>? PCardContextDispatcher { get; set; }`

Where a typed wording goes to become a chip.
The editor sets it when it builds the card and answers whether a request went out.

## `internal string PCardContextText`

What is standing in the entry.

## `internal int PCardContextPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)`

Makes the chips show the engine's Situations, matched by id.
A chip whose title changed is replaced, since a chip is immutable.
The caret stays where it was.

## `internal PContext? PCardContextFind(int step)`

The chip standing one step from the entry, before it or after it, or null.
That is what a backspace at the start, or a delete at the end, reaches for.

## `internal bool PCardContextMove(int step)`

Steps the entry one place along the field, past the Situation on that side.
A Situation is passed over as a single character would be.
Reports whether there was anywhere left to go.

## `internal void PCardContextClear()`

Empties the entry without reading it as a further edit.

## `internal bool PCardContextMatch(long? id)`

Whether the card already carries the stored Situation, which is what keeps the dropdown from offering it twice.

## `internal bool PCardContextCheck(string text)`

Whether the card already carries a Situation with this wording.
An unreadable Situation shows no wording, so it is never counted as a duplicate.
Two of them on one card stand for two Situations the store could not read.

## Inline notes

### `private void PCardContextChange(object? sender, PropertyChangedEventArgs arguments)`

Every edit of the entry is reported.
The editor can offer the Situations the wording matches as it is typed.
The card cannot read the workspace itself, so it says what was typed and the editor answers.

The comma is read off the entry's text rather than off a keystroke.
So a comma arriving by paste ends a Situation exactly as a typed one does.
A paste of several commas leaves several Situations.
Everything before the last comma is dispatched and the remainder stays in the entry.

The guard around the rewrite is there because the rewrite sets the property being answered.
Without it the handler would answer itself.

### `private void PCardContextUpdate()`

The hint belongs to the empty field, not to the entry.
Once a card carries a Situation the entry sits beside it.
Prompting again would read as a second, unfilled field.
