# PCardLabel.cs

## `internal sealed partial class PCard`

The Tags a card shows, held as the items of one field.
The collection is a run of Tag chips with one open entry among them, which is the caret.
The engine holds the chips, the card renders them by id, and every commit or removal is a request.
Duplicates are refused before a request is sent.

## `internal Action<string>? PCardLabelNotice { get; set; }`

Where the entry's text goes as it is typed, so the editor can offer matching Tags.

## `internal Func<string, bool>? PCardLabelDispatcher { get; set; }`

Where a typed tag goes to become a chip, answering whether a request went out.

## `internal string PCardLabelText`

What is standing in the entry.

## `internal int PCardLabelPosition`

How many chips stand before the caret, which is the place a new chip is asked for.

## `internal void PCardLabelShow(IReadOnlyList<LTagDraft> drafts)`

Makes the chips show the engine's Tags, matched by id, replacing a chip whose text changed.

## `internal PLabelChip? PCardLabelFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardLabelMove(int step)`

Steps the entry one place along the field and reports whether there was anywhere left to go.

## `internal void PCardLabelClear()`

Empties the entry without reading it as a further edit.

## `internal bool PCardLabelMatch(long? id)`

Whether the card already carries the stored Tag.

## `internal bool PCardLabelCheck(string text)`

Whether the card already carries a Tag with this text.

## Inline notes

### `private void PCardLabelChange(object? sender, PropertyChangedEventArgs arguments)`

The comma is read off the entry's text, so a pasted comma ends a tag as a typed one does.
Everything before the last comma is dispatched and the remainder stays in the entry.
The guard around the rewrite keeps the handler from answering itself.

### `private void PCardLabelUpdate()`

The hint belongs to the empty field and goes once a chip stands beside the entry.
