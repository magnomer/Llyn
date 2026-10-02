# LEntryArchiveFind.cs
Hash: `41bbfca1a9cb55c5`

## `public sealed partial class LEntryArchive`

The finding side of the Entry store.
It holds every reader that answers a query with a list of Entries.
A headword match, a text scan, and the owner finds over Tag, Register, Situation, Example and Reference live here.
Each list comes back ordered by headword so a panel can show it as read.

## `public IReadOnlyList<LEntry> LEntryFind(string query)`

Returns the entries whose headword matches `query`, ordered by headword.
It returns every entry when `query` is empty or holds nothing but whitespace.
Matching is `lmatch()`, the catalog text match registered on every connection (`LDatabase.LDatabaseConnectionRead`).
It is a case-insensitive contains, and `*` and `?` in the query act as wildcards.
Anything cleverer waits for a stated requirement rather than being guessed at here.
Prefix weighting, forms and accent folding are such things.

Case is folded with the invariant .NET lowercase, not SQLite's `lower()`, which folds ASCII only.
`Ä` finds `ä`, and Greek or Cyrillic headwords match in either case.
Scripts without case (Korean, Japanese, Chinese) are unaffected either way.

The query is trimmed before it is matched.
So trailing space left by typing does not narrow the result.
A query of spaces alone lists everything, exactly as an empty box does.

## `public IReadOnlyList<LEntry> LEntryHeadwordFind(string language, string headword)`

Every Entry whose language and headword equal the given ones, letter case folded, in headword order.
The fold is `lfold()`, the invariant lowercase registered beside `lmatch()`.
This is the candidate list for a clicked word, and zero rows is a legitimate answer.

## `public IReadOnlyList<LEntry> LEntryHeadwordScan(string language, string text)`

Every Entry of one language whose headword appears inside `text`, longest headword first.
One query serves a language without word separators, where the engine tries each hit against the click.

## `public IReadOnlyList<LEntry> LEntryScan(IReadOnlyList<long> ids, string query)`

The entries among `ids` whose headword matches `query`, in id order, in one session.
The ids are sent in batches, so a long list never outruns the parameter limit.
The cell of a rhyme table reads its entries this way instead of one read per id.

## `public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)`

The epithet of every entry among `ids` that has one, keyed by id, in one session.
An entry with no epithet has no key, so a caller falls back to nothing without a lookup per row.
A catalog reads its epithets this way instead of one session per row.

## `private static IEnumerable<IReadOnlyList<long>> LEntryListDivide(IReadOnlyList<long> ids)`

Cuts the ids into batches of five hundred, well under what one statement may bind.

## `private static string LEntryListFormat(SqliteCommand command, IReadOnlyList<long> ids)`

Binds one parameter per id on the command and answers the list to put inside the `IN`.

## `private static IReadOnlyList<LEntry> LEntryListRead(SqliteCommand command)`

The rows a prepared query yields, in the order the query states.

## `public IReadOnlyList<LEntry> LEntrySituationFind(long situationId)`

Returns the entries referencing the Situation `situationId` names, ordered by headword.
It returns every entry when `situationId` is zero.
A situation is held on a meaning or a collocation, so both sides are unioned into one row per entry.

## `public IReadOnlyList<LEntry> LEntryExampleFind(long exampleId)`

Returns the entries quoting the Example `exampleId` names, ordered by headword.
It returns every entry when `exampleId` is zero.
An example is quoted on a meaning or a collocation, so both sides are unioned into one row per entry.

## `public IReadOnlyList<LEntry> LEntryReferenceFind(long referenceId)`

Returns the entries citing the Source `referenceId` names, ordered by headword.
It returns every entry when `referenceId` is zero.
A card cites a Source by holding an Example that cites it, so the walk goes through the example row.

## `private IReadOnlyList<LEntry> LEntryOwnerFind(long ownerId, string statement)`

Runs one of the owner-shaped finds above, binding the owner id as `$owner`.
The three statements differ only in the link tables they walk, so they share the reading.

## `public IReadOnlyList<LEntry> LEntryTagFind(long tagId)`

Returns the entries carrying the Tag `tagId` names, ordered by headword.
It returns every entry when `tagId` is zero.
A tag is held on a meaning or a collocation, never on the entry itself.
So both sides are asked and the entries they name are unioned.
An entry tagged on several of its cards is still one row.

## `public IReadOnlyList<LEntry> LEntryRegisterFind(long registerId)`

Returns the entries carrying the Register `registerId` names, ordered by headword.
It returns every entry when `registerId` is zero.
A register is held on a meaning or a collocation, so both sides are unioned as the Tag find does.

## `public long LEntryCountRead()`

How many entries the database stores, counted in one statement.
The status bar prints it, so nothing is loaded that the count alone answers.
