# LEntryArchive.cs

## `public sealed class LEntryArchive`

Persists entries and their two owned child structures — written forms and parts of speech — in the workspace database.
An entry's id and timestamps are assigned here on creation.
Its forms and POS are written as ordered child rows.
So reordering rewrites `position` only and never touches the entry id.
Deleting an entry removes its forms and POS through the foreign-key cascade.

Every method runs inside a `LDatabaseSession`.
So a caller that opens one of its own around several stores gets one transaction.

## `public LEntryArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LEntry LEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Inserts `entry` with a fresh opaque id and creation and modification timestamps.
It writes `forms` and `speeches` as ordered child rows.
Their entry id and position are assigned from list order.
Returns the stored entry with its id and timestamps filled in.
The whole write is one transaction.

## `public LEntry? LEntryRead(long id)`

Reads the entry row for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or holds nothing but whitespace.
Matching is a case-insensitive contains and nothing more.
Anything cleverer waits for a stated requirement rather than being guessed at here.
Prefix weighting, forms and accent folding are such things.

Case is folded over the whole of Unicode, not just ASCII.
`Ä` finds `ä`, and Turkish, Greek or Cyrillic headwords match in either case.
That is what `lfold()` is for.
It is the invariant .NET fold registered on every connection (`LDatabase.LDatabaseConnectionRead`).
It stands in for SQLite's `lower()`, which folds ASCII only.
Scripts without case (Korean, Japanese, Chinese) are unaffected either way.

The query is trimmed before it is matched.
So trailing space left by typing does not narrow the result.
A query of spaces alone lists everything, exactly as an empty box does.

## `public IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword)`

Every Entry of one language whose headword is `headword`, letter case folded, in headword order.
This is the candidate list for a clicked word, and zero rows is a legitimate answer.

## `public IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text)`

Every Entry of one language whose headword appears inside `text`, longest headword first.
One query serves a language without word separators, where the engine tries each hit against the click.

## `private static IReadOnlyList<LEntry> LEntryListRead(SqliteCommand command)`

The rows a prepared query yields, in the order the query states.

## `public IReadOnlyList<LEntry> LEntryTagFind(long tagId)`

Returns the entries carrying the Tag `tagId` names, ordered by headword.
It returns every entry when `tagId` is zero.
A tag is held on a meaning or a collocation, never on the entry itself.
So both sides are asked and the entries they name are unioned.
An entry tagged on several of its cards is still one row.

## `public IReadOnlyList<LForm> LEntryFormRead(long id)`

Reads the entry's written forms, ordered by position.

## `public IReadOnlyList<LSpeech> LEntrySpeechRead(long id)`

Reads the entry's part-of-speech assignments, ordered by position.

## `public void LEntryUpdate(LEntry entry)`

Updates the headword and metadata of the entry identified by `entry`'s id and stamps a fresh modification timestamp.
The id, forms, and POS are untouched.
Throws when no entry carries that id, rather than reporting success for a write that reached nothing.

## `public void LEntryFormSet(long id, IReadOnlyList<LForm> forms)`

Replaces the entry's forms with `forms` in list order.
Existing form rows are cleared and the new set written.
So reordering rewrites positions while the entry id stays fixed.

## `public void LEntrySpeechSet(long id, IReadOnlyList<LSpeech> speeches)`

Replaces the entry's part-of-speech assignments with `speeches` in list order, clearing the existing rows first.
Reordering rewrites positions.
The entry id stays fixed.

## `public void LEntryDelete(long id)`

Deletes the entry identified by `id` and everything it owns.
The foreign-key cascade carries away its forms and parts of speech.
It carries away its inflections and their features.
It carries away its meanings, each with its inline definition field.
It carries away its pronunciations with their syllables and recordings, and its transcriptions.
It carries away its collocations and its single note.
It carries away every association row hanging from the entry, its meanings, or its collocations.
The independent Examples, Tags, Situations, References, and Authors those associations pointed at are left standing.
Only the rows linking them to this entry disappear.

## Inline notes

### `private static LEntry LEntryRowRead(SqliteDataReader reader)`

The entry row shape every read here selects, in one place.
A single read and a find would otherwise drift apart column by column.

### `bool declared = speech.LSpeechValueId is > 0;`

A row links a declared value or carries typed text, never both.
The table's CHECK says so.
A caller handing over both would otherwise write a row that resolves one way and displays another.
The id wins, because a name matching a value is that value.
