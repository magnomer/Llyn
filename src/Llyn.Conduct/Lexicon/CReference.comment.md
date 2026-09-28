# CReference.cs

## `public sealed record CReference(`

The stated fields of the Source draft, as the source editor writes them into its fields.
It copies the engine's edit sheet, so every text and key is already decided.

**Parameters**

- `CReferenceTitle`: the title as written.
- `CReferenceTitleHint`: the resource key of the title field's hint.
- `CReferenceYear`: the year as written.
- `CReferenceYearHint`: the resource key of the year field's hint.
- `CReferenceUrl`: the address as written.
- `CReferenceUrlHint`: the resource key of the address field's hint.
- `CReferenceNote`: the note as written.
- `CReferenceNoteHint`: the resource key of the note field's hint.
- `CReferenceKindKey`: the resource key of the kind's name.
- `CReferenceKindTag`: the kind's tag, which marks its entry in the kind menu.
