# LDiweiArchive.cs

## `public sealed class LDiweiArchive`

Derives and reads the 音韻地位 categories of a language from its stored fanqie rows.
Nothing here is typed by the user: the rows follow the fanqie store and the hypothesis.

## `private const string LDiweiArchiveTally =`

The count of entries in a category: entries of the language whose headword holds a linked character.
A fanqie row is per character, so entries are found by the character they are written with.

## `public void LDiweiApply(string language, string character, LHypothesis? hypothesis)`

Rebuilds the links of one character after its fanqie rows were stored.
Categories left without a link in the language are dropped afterwards.

## `public void LDiweiRebuild(string language, LHypothesis? hypothesis)`

Drops every category of the language and derives them again from every stored character.
Run when the engine binds a workspace, since the tone classes depend on the hypothesis file as it now reads.

## `public IReadOnlyList<LDiwei> LDiweiRead(string language, string kind)`

The categories of one kind with their entry counts, in the order they were first made.

## `public LDiwei? LDiweiFind(string language, string kind, string key)`

The category with this key, with its count, or `null` when no placement carries it.

## `public IReadOnlyList<long> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds)`

The entries whose headword holds a character with one fanqie row linked to every given category at once.
So an initial and a rime together name one cell of the rime table, not two separate sets.
Empty ids give nothing.

## `public IReadOnlyList<LFanqieRow> LDiweiFanqieRead(long diweiId)`

Every stored fanqie row linked to one category, in storage order, each carrying its character.

## `private static LDiwei LDiweiRowRead(SqliteDataReader reader)`

One category row with its count, as the read queries select them.

## `private static void LDiweiCharacterApply(`

Reads the character's fanqie rows with their ids, clears each row's links and writes them afresh.
The hypothesis is run once per row here.
What it gives is stored on the row and keyed into the links.
So the reading view prints a stored reading and never runs the hypothesis itself.

## `private static void LDiweiReadingSave(LDatabaseSession session, long fanqieId, LHypothesisSound? sound)`

Writes the derived reading and tone class onto the fanqie row, or blanks both when the hypothesis gave none.

## `private static IEnumerable<(string, string)> LDiweiKeyScan(LFanqieRow row, LHypothesisSound? sound)`

The category keys one fanqie row carries: its initial, its rime without the 重紐 letter, and its tone class.
The tone class comes from the sound the hypothesis derived.
A row it could not resolve links to no tone.
An empty part is skipped.

## `private static long LDiweiRowCreate(LDatabaseSession session, string language, string kind, string key)`

The id of the category, made when it is not there yet.

## `private static void LDiweiLinkCreate(LDatabaseSession session, long fanqieId, long diweiId)`

One link from a fanqie row to a category, ignored when it already stands.

## `private static void LDiweiOrphanClear(LDatabaseSession session, string language)`

Drops the categories of the language that no link points to any more.
