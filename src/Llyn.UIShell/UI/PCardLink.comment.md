# PCardLink.cs

## `internal sealed partial class PCard`

The Translations a card shows, held as the items of one field the way the Tags are.
The collection is a run of link chips with one open entry among them, which is the caret.

A link holds only the id of the Entry it points at.
The headword, the language and the flag beside it are drawn from that Entry, not stored.
So a chip is a pointer the field happens to be able to read aloud.

Typed text is not a link until an Entry answers it.
The card cannot ask, because the search belongs to the engine and the card holds no engine.
So a word standing in the entry is handed to the editor, which resolves it and sends the request.
The engine holds the links, and the card renders them by id from the targets the editor read.

## `internal Func<string, bool, bool>? PCardLinkDispatcher { get; set; }`

Where a typed word goes to be turned into a link.
The editor sets it when it builds the card.
A card with none takes no new links, which is what an unattached card should do.
The second argument says whether the editor may open a dropdown for a word it cannot settle.
The answer says whether the word became a link, so the card knows to empty the entry.

## `internal Action<string>? PCardLinkNotice { get; set; }`

Where the entry's text goes as it is typed, so the editor can offer matching Entries.

## `internal string PCardLinkText`

What is standing in the entry.

## `internal int PCardLinkPosition`

How many chips stand before the caret, which is the place a new link is asked for.

## `internal void PCardLinkShow(IReadOnlyList<LTranslationTarget> targets)`

Makes the chips show the engine's links, matched by id.
Each target carries the headword and language its id stands for, read once for the whole card.
A chip whose word or language changed is replaced, since a chip is immutable.

## `internal PLinkChip? PCardLinkFind(int step)`

The chip standing one step from the entry, before it or after it, or null.

## `internal bool PCardLinkMove(int step)`

Steps the entry one place along the field and reports whether there was anywhere left to go.

## `internal void PCardLinkCommit()`

Hands what is standing in the entry to the editor to be resolved, dropdown allowed.
That is what pressing enter means.
The entry is emptied only when the word became a link.
Otherwise it is left standing, because the word may still be waiting on a choice.

## `internal void PCardLinkClear()`

Empties the entry once the word standing in it became a link.
The rewrite is marked so the comma reader does not answer it.

## `internal bool PCardLinkCheck(long id)`

Whether the card already links the Entry, which keeps a second pick from becoming a second request.

## `internal void PCardFlagUpdate()`

Redraws the chips whose flag was missing when they were built.
Flags are loaded after the window opens, so an early chip can be drawn without one.
A chip that already has its flag is left alone.

## Inline notes

### `private void PCardLinkChange(object? sender, PropertyChangedEventArgs arguments)`

The comma is read off the entry's text rather than off a keystroke.
So a comma arriving by paste ends a word exactly as a typed one does.
Everything before the last comma is resolved without a dropdown, and what settles becomes a link.
A word that does not settle goes back into the entry beside the remainder.
No dropdown opens here, because several words would each want one and only the last would survive.

The guard around the rewrite is there because the rewrite sets the property being answered.
Without it the handler would answer itself.

### `private void PCardLinkUpdate()`

The hint is read from the localization resources, so it speaks the language the window does.
The hint belongs to the empty field, not to the entry.
Once a card carries a link the entry sits beside it.
