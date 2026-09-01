# LNote.cs

## `public sealed record LNote(`

The single Note an entry owns: subordinate free text hanging from the entry itself, with no id and no order. An entry carries *at most one* Note — the owning entry id is the identity — so saving a Note replaces whatever Note the entry had.

TODO: `PNoteContents` edits WYSIWYG Markdown, but the serialized format is not yet prescribed. `LNoteText` therefore stores whatever the editor produced, as-is; pin the format down and normalize on write once it is decided.

**Parameters**

- `LNoteEntryId` — Owning entry id — the Note's identity, one per entry.
- `LNoteText` — The note text, stored exactly as the editor produced it.
