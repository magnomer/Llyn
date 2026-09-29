# PCardRegister.cs

## `internal sealed partial class PCard`

The Registers a card shows, held as the items of one field the way the Situations are.
The collection is a run of Register chips with one open entry among them, which is the caret.
The engine holds the chips, the card renders them by id, and every commit or removal is a request.
The engine's clerk skips a Register the card already holds, by id and by wording alike.

## `internal event Action<PCard, string>? PCardRegisterNotice;`

Where the entry's text goes as it is typed, so the editor can offer matching Registers.
It carries the card, since the editor subscribes one handler for every card it builds.

## `internal string PCardRegisterText`

What is standing in the entry.

## `internal int PCardRegisterPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardRegisterShow(IReadOnlyList<CRegisterDraft> drafts)`

Makes the chips show the engine's Registers, matched by id, replacing a chip whose name changed.

## `internal PRegister? PCardRegisterFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardRegisterMove(int step)`

Steps the entry one place along the field and reports whether there was anywhere left to go.

## `internal void PCardRegisterClear()`

Empties the entry after a commit or a pick.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCardRegisterRefine(string rest)`

Puts the register gate's answer in the entry, then tells the editor what now stands there.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a name as a typed one does.

## Inline notes

### `private void PCardRegisterUpdate()`

The hint belongs to the empty field and goes once a chip stands beside the entry.
