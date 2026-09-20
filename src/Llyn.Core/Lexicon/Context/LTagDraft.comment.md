# LTagDraft.cs

## `public sealed record LTagDraft(`

One Tag as a card draft carries it.
It holds the id of the Tag the row links and the text the row shows.
A positive id names a stored Tag, and the text is what was read back from it.
A zero id means the text was typed and no Tag has been resolved for it yet.
The save resolves such a row to the Tag reading the same, or creates one.

**Parameters**

- `LTagDraftId` — The id of the Tag the row links, zero until one is resolved.
- `LTagDraftText` — The text the row shows.

## `public static LTagDraft LTagDraftCreate(string text)`

A text typed with no Tag resolved for it yet.
