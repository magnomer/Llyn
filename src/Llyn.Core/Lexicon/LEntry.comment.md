# LEntry.cs

## `public sealed record LEntry(`

A lexical entry: the headword record and the root that owns its written forms, parts of speech, and every other subordinate lexical structure. `LEntryId` is the identity — an opaque program-generated id — and the headword is never identity, so two entries with the same headword stay distinct records.

**Parameters**

- `LEntryId` — Opaque, program-generated stable id.
- `LEntryHeadword` — Main headword; display text, not an identifier.
- `LEntryLanguage` — Language identifier the entry belongs to.
- `LEntryProficiency` — Optional proficiency information.
- `LEntryFrequency` — Optional frequency information.
- `LEntryAddedUtc` — Optional creation timestamp, ISO 8601 UTC.
- `LEntryUpdatedUtc` — Optional last-modification timestamp, ISO 8601 UTC.
