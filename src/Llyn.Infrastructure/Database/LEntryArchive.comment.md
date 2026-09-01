# LEntryArchive.cs

## `public sealed class LEntryArchive`

Persists entries and their two owned child structures — written forms and parts of speech — in the workspace database. An entry's id and timestamps are assigned here on creation; its forms and POS are written as ordered child rows, so reordering rewrites `position` only and never touches the entry id. Deleting an entry removes its forms and POS through the foreign-key cascade.

Every method runs inside a `LDatabaseSession`, so a caller that opens one of its own around several stores gets one transaction across all of them.

## `public LEntryArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Inserts `entry` with a fresh opaque id and creation/modification timestamps, writing `forms` and `speeches` as ordered child rows (their entry id and position are assigned from list order). Returns the stored entry with its id and timestamps filled in. The whole write is one transaction.

## `public LEntry? LEntryRead(string id)`

Reads the entry row for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword, or every entry when `query` is empty or holds nothing but whitespace. Matching is a case-insensitive contains and nothing more; anything cleverer — prefix weighting, forms, accent folding — waits for a stated requirement rather than being guessed at here.

Case is folded over the whole of Unicode, not just ASCII: `Ä` finds `ä`, and Turkish, Greek or Cyrillic headwords match in either case. That is what `lfold()` is for — the invariant .NET fold registered on every connection (`LDatabase.LDatabaseConnectionRead`), standing in for SQLite's `lower()`, which folds ASCII only. Scripts without case (Korean, Japanese, Chinese) are unaffected either way.

The query is trimmed before it is matched, so trailing space left by typing does not narrow the result and a query of spaces alone lists everything, exactly as an empty box does.

## `public IReadOnlyList<LForm> LEntryFormRead(string id)`

Reads the entry's written forms, ordered by position.

## `public IReadOnlyList<LSpeech> LEntrySpeechRead(string id)`

Reads the entry's part-of-speech assignments, ordered by position.

## `public void LEntryUpdate(LEntry entry)`

Updates the headword and metadata of the entry identified by `entry`'s id and stamps a fresh modification timestamp. The id, forms, and POS are untouched. Throws when no entry carries that id, rather than reporting success for a write that reached nothing.

## `public void LEntryFormSet(string id, IReadOnlyList<LForm> forms)`

Replaces the entry's forms with `forms` in list order: existing form rows are cleared and the new set written, so reordering rewrites positions while the entry id stays fixed.

## `public void LEntrySpeechSet(string id, IReadOnlyList<LSpeech> speeches)`

Replaces the entry's part-of-speech assignments with `speeches` in list order, clearing the existing rows first. Reordering rewrites positions; the entry id stays fixed.

## `public void LEntryDelete(string id)`

Deletes the entry identified by `id` and everything it owns. The foreign-key cascade carries away its forms and parts of speech, its inflections and their features, its meanings (each with its inline definition field), the relations originating from those meanings, its single pronunciation with its syllables and representations, its collocations, its single note, and every association row hanging from the entry, its meanings, or its collocations. The independent Examples, Tags, Situations, References, and Authors those associations pointed at are left standing — only the rows linking them to this entry disappear.

Guarded against the one thing a cascade must not decide on its own: a lexical link from *another* entry pointing at this entry or at one of its meanings — a relation target or a collocation synonym. While any such link exists nothing is deleted and an `InvalidOperationException` is thrown; remove those links first. Links pointing here from inside this entry are cleared as part of the delete, since they are owned by rows that are going anyway. The guard and the delete share one transaction, so nothing can slip between them.

## Inline notes

### `private static void LEntryLinkValidate(SqliteConnection connection, string id)`

Counts the lexical links that reach this entry from outside it: relations elsewhere whose target is this entry or one of its meanings, and collocation synonyms elsewhere pointing at either. The schema declares those columns without a cascade precisely so they cannot be swept away silently; this turns the resulting foreign-key error into a message that names the reason.

### `private static void LEntryLinkClear(SqliteConnection connection, string id)`

Clears the links this entry owns before the entry row goes. They would cascade anyway — a relation's target row hangs from the relation, a synonym from its collocation — but a link that points back at this same entry would be checked against a row already being deleted, and the order the cascade visits tables in is not ours to rely on. Removing them first makes the delete deterministic.

### `private static LEntry LEntryRowRead(SqliteDataReader reader)`

The entry row shape every read here selects, in one place: a single read and a find would otherwise drift apart column by column.

### `bool declared = !string.IsNullOrWhiteSpace(speech.LSpeechValueId);`

A row names a declared preset or carries typed text, never both: the table's CHECK says so, and a caller handing over both would otherwise be written as a row that resolves one way and displays another. The id wins, because a name matching a preset is that preset.
