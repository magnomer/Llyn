# PCardLabel.cs

## `internal sealed partial class PCard`

The Tags a card shows, held as the items of one field.
The collection is a run of Tag chips with one open entry among them, which is the caret.
The engine holds the chips, the card renders them by id, and every commit or removal is a request.
The engine's clerk skips a text the card already holds, and hands every text ready trimmed.

## `internal event Action<PCard, string>? PCardLabelNotice;`

Where the entry's text goes as it is typed, so the editor can offer matching Tags.
It carries the card, since the editor subscribes one handler for every card it builds.

## `internal string PCardLabelText`

What is standing in the entry.

## `internal int PCardLabelPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardLabelShow(IReadOnlyList<CTagDraft> drafts)`

Makes the chips show the engine's Tags, matched by id, replacing a chip whose text changed.

## `internal PLabelChip? PCardLabelFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardLabelMove(int step)`

Steps the entry one place along the field and reports whether there was anywhere left to go.

## `internal void PCardLabelClear()`

Empties the entry after a commit or a pick.
The empty text reaches the gate as any edit does, and an empty text adds nothing.

## `internal void PCardLabelRefine(string rest)`

Puts the tag gate's answer in the entry, then tells the editor what now stands there.
The editor's text observer hands every edit to the gate, which reads the comma.
So a pasted comma ends a tag as a typed one does.

## Inline notes

### `private void PCardLabelUpdate()`

The hint belongs to the empty field and goes once a chip stands beside the entry.
