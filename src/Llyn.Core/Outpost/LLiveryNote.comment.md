# LLiveryNote.cs
Hash: `82844d79aa7a10fe`

## `public sealed record LLiveryNote(string LLiveryNoteBody, IReadOnlyList<LParcel> LLiveryNoteParcel);`

One entry rendered for Joplin, with the pictures its body names.
The body and its parcels travel together, so no note links to a resource nobody uploads.

**Parameters**

- `LLiveryNoteBody` — The note's content, HTML that Joplin keeps inside Markdown.
- `LLiveryNoteParcel` — One parcel per distinct picture, each named in the body by its id.
