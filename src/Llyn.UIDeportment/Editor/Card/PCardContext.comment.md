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
The engine's clerk skips a wording the card already shows, so what the field shows is what the engine holds.

## `internal string PCardContextText`

What is standing in the entry.

## `internal int PCardContextPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardContextShow(IReadOnlyList<CSituationDraft> drafts)`

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

Empties the entry after a commit or a pick.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCardContextRefine(string rest)`

Puts the situation gate's answer in the entry.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a Situation as a typed one does.

## Inline notes

### `private void PCardContextUpdate()`

The hint belongs to the empty field, not to the entry.
Once a card carries a Situation the entry sits beside it.
Prompting again would read as a second, unfilled field.
