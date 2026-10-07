# LEntry.cs
Hash: `ee052ee9cffdf320`

## `public sealed record LEntry(long LEntryId, string LEntryHeadword, string LEntryLanguage, int LEntryGrasp, string? LEntryAddedUtc, string? LEntryUpdatedUtc, LUnit LEntryUnit = LUnit.LUnitEmpty)`

A lexical entry, the headword record.
It is the root that owns its written forms and parts of speech.
It owns every other subordinate lexical structure too.
`LEntryId` is the identity, an opaque program-generated id.
The headword is never identity, so two entries with the same headword stay distinct records.

**Parameters**

- `LEntryId` — Opaque, program-generated stable id.
- `LEntryHeadword` — Main headword, which is display text and not an identifier.
- `LEntryLanguage` — Language identifier the entry belongs to.
- `LEntryGrasp` — Half-step count of how well the user knows the entry, zero when unrated, see [LGrasp](LGrasp.comment.md).
- `LEntryAddedUtc` — Optional creation timestamp, ISO 8601 UTC.
- `LEntryUpdatedUtc` — Optional last-modification timestamp, ISO 8601 UTC.
- `LEntryUnit` — The lexical unit the entry stands for, see [LUnit](LUnit.comment.md).
