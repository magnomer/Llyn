# PTranslationItem.cs

## `internal sealed class PTranslationItem`

One row of the dropdown that opens when typed text matches more than one Entry.
Most rows stand for an Entry that already exists, so they carry its id.
The last row stands for the Entry the typed word would create, and carries no id yet.
That row is the only one marked fresh.
The flag is derived from the language so the dropdown reads the same as every other headword list.
