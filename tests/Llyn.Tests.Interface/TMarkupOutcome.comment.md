# TMarkupOutcome.cs
Hash: `3ad813ad243474aa`

## `internal sealed record TMarkupOutcome(IReadOnlyList<LEntry> TMarkupOutcomeEntry, IReadOnlyList<LMarkupOmission> TMarkupOutcomeOmission)`

What a test import produced, as the import tests read it.
The engine reports only what the import left behind.
The stored entries are found again by headword, so a test can open what it imported.

**Parameters**

- `TMarkupOutcomeEntry` — The stored entries in the order of the file.
- `TMarkupOutcomeOmission` — What the read and the import skipped, in file order.
