# PCardRegister.cs

## `internal sealed partial class PCard`

The Registers a card shows, held as the items of one field the way the Situations are.
The collection is a run of Register chips with one open entry among them, which is the caret.
The engine holds the chips, the card renders them by id, and every commit or removal is a request.
Duplicates are refused before a request is sent, by id and by wording alike.

## `internal Action<string>? PCardRegisterNotice { get; set; }`

Where the entry's text goes as it is typed, so the editor can offer matching Registers.

## `internal Func<string, bool>? PCardRegisterDispatcher { get; set; }`

Where a typed name goes to become a chip, answering whether a request went out.

## `internal string PCardRegisterText`

What is standing in the entry.

## `internal int PCardRegisterPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardRegisterShow(IReadOnlyList<LRegisterDraft> drafts)`

Makes the chips show the engine's Registers, matched by id, replacing a chip whose name changed.

## `internal PRegister? PCardRegisterFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardRegisterMove(int step)`

Steps the entry one place along the field and reports whether there was anywhere left to go.

## `internal void PCardRegisterClear()`

Empties the entry without reading it as a further edit.

## `internal bool PCardRegisterMatch(long? id)`

Whether the card already carries the stored Register.

## `internal bool PCardRegisterCheck(string text)`

Whether the card already carries a Register with this wording.

## Inline notes

### `private void PCardRegisterChange(object? sender, PropertyChangedEventArgs arguments)`

The comma is read off the entry's text, so a pasted comma ends a name as a typed one does.
Everything before the last comma is dispatched and the remainder stays in the entry.
The guard around the rewrite keeps the handler from answering itself.

### `private void PCardRegisterUpdate()`

The hint belongs to the empty field and goes once a chip stands beside the entry.
