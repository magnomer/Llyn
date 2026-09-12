# LNote.cs

## `public sealed record LNote(`

The single Note an entry owns: subordinate free text hanging from the entry itself, with no id and no order.
An entry carries *at most one* Note, and the owning entry id is the identity.
So saving a Note replaces whatever Note the entry had.

The text is Markdown in the dialect `LMarkdown` reads.
The engine normalizes it on write, so a stored Note is always in canonical form.

**Parameters**

- `LNoteEntryId` — Owning entry id — the Note's identity, one per entry.
- `LNoteText` — The note text as normalized Markdown.
