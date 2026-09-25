# LImageDraft.cs

## `public sealed record LImageDraft(`

One image row a card is holding before it is stored.

A card points at an image rather than owning one, and two cards may point at the same picture.
Carrying the stored row id beside the location is what lets a citation stay a citation.
Without it a file that declared one picture twice would import as two rows.
The situation and the register drafts carry their row id for the same reason.

**Parameters**

- `LImageDraftLocation` — Where the picture is read from, a file path or a web address.
- `LImageDraftId` — The stored row this draft stands for, empty when the row is new.

## Inline notes

### `public bool LImageDraftEmpty`

A row is empty when it names no picture.

## `public LImageDraft LImageDraftNormalize()`

The same row with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
