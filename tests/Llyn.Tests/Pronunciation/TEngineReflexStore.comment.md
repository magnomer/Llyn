# TEngineReflexStore.cs

## `public sealed class TEngineReflexStore`

Covers the engine filling and keeping the reflex rows of a stored entry.
A start on an entry with nothing stored fetches, stores the rows and raises the reflex bulletin.
A start on an entry with rows stored fetches nothing.
A rebuild drops the stored rows and fetches them again.
An entry every page was not found for is asked once per session and stores nothing.
A held draft of the entry with no rows takes the stored rows and stays unchanged against the entry.
The epithet is derived when the rows are stored and kept on the entry.
It reads back blank once the rows are dropped.
The setting hides it from the read without touching what is stored.
A workspace rebuilt from an older schema has its blank epithets and respellings derived once on open.
The find that reads rows off the pages is covered in `TEngineReflex.cs`.
The packs, pages and row readers the two share live in `TReflexFixture.cs`.
