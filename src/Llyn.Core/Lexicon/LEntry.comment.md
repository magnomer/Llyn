# LEntry.cs

## `public sealed record LEntry(`

A lexical entry, the headword record.
It is the root that owns its written forms and parts of speech.
It owns every other subordinate lexical structure too.
`LEntryId` is the identity, an opaque program-generated id.
The headword is never identity, so two entries with the same headword stay distinct records.

**Parameters**

- `LEntryId` — Opaque, program-generated stable id.
- `LEntryHeadword` — Main headword, which is display text and not an identifier.
- `LEntryLanguage` — Language identifier the entry belongs to.
- `LEntryProficiency` — Optional proficiency information.
- `LEntryFrequency` — Optional frequency information.
- `LEntryAddedUtc` — Optional creation timestamp, ISO 8601 UTC.
- `LEntryUpdatedUtc` — Optional last-modification timestamp, ISO 8601 UTC.
