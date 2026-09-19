# LDiweiArchive.cs

## `public sealed class LDiweiArchive`

Derives and reads the 音韻地位 categories of a language from its stored fanqie rows.
The categories follow the fanqie store and the hypothesis, never the user.
The entries a category counts follow the anchors the user drew from reflex rows to fanqie rows.

## `private const string LDiweiArchiveTally =`

The count of entries in a category: entries owning a reflex row anchored to a linked fanqie row.
An entry with no anchored row is not counted, whatever its headword.

## `public void LDiweiApply(string language, string character, LHypothesis? hypothesis)`

Rebuilds the links of one character after its fanqie rows were stored.
Categories left without a link in the language are dropped afterwards.

## `public void LDiweiRebuild(string language, LHypothesis? hypothesis)`

Drops every category of the language and derives them again from every stored character.
Run when the engine binds a workspace, since the tone classes depend on the hypothesis file as it now reads.

## `public IReadOnlyList<LDiwei> LDiweiRead(string language, string kind)`

The categories of one kind with their entry counts, in the order they were first made.

## `public LDiwei? LDiweiRead(long diweiId)`

One category by its id, with its entry count, or null when none is stored under it.

## `public LDiwei? LDiweiFind(string language, string kind, string key)`

The category with this key, with its count, or `null` when no placement carries it.

## `public IReadOnlyList<long> LDiweiEntryScan(string language, IReadOnlyList<long> diweiIds)`

The entries owning a reflex row anchored to one fanqie row linked to every given category at once.
So an initial and a rime together name one cell of the rime table, not two separate sets.
Empty ids give nothing.

## `public IReadOnlyList<LFanqieRow> LDiweiFanqieRead(long diweiId)`

The fanqie rows linked to one category that some reflex row is anchored to, in storage order.
Each carries its character and id.
A placement nobody anchored is left out, so the page lists no character without an anchored reading.

## `private static LDiwei LDiweiRowRead(SqliteDataReader reader)`

One category row with its count, as the read queries select them.

## `private static void LDiweiCharacterApply(`

Reads the character's fanqie rows with their ids, clears each row's links and writes them afresh.
The hypothesis is run once per row here.
What it gives is stored on the row and keyed into the links.
So the reading view prints a stored reading and never runs the hypothesis itself.

## `private static void LDiweiReadingSave(LDatabaseSession session, long fanqieId, LHypothesisSound? sound)`

Writes the derived reading and tone class onto the fanqie row, or blanks both when the hypothesis gave none.
A row already holding the same pair is not written, so a rebuild at every open dirties nothing.

## `private static IEnumerable<(string, string)> LDiweiKeyScan(LFanqieRow row, LHypothesisSound? sound)`

The category keys one fanqie row carries: its initial, its rime keyed with division and 開合, and its tone class.
The tone class comes from the sound the hypothesis derived.
A row it could not resolve links to no tone.
An empty part is skipped.

## `private static long LDiweiRowCreate(LDatabaseSession session, string language, string kind, string key)`

The id of the category, made when it is not there yet.

## `private static void LDiweiLinkCreate(LDatabaseSession session, long fanqieId, long diweiId)`

One link from a fanqie row to a category, ignored when it already stands.

## `private static void LDiweiOrphanClear(LDatabaseSession session, string language)`

Drops the categories of the language that no link points to any more.
