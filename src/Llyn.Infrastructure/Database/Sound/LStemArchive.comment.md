# LStemArchive.cs

## `public sealed class LStemArchive : LStemVault`

The workspace-database adapter for the phonetic series the engine browses by.
A series row is shared, so it is made as a character first names it and dropped once none does.
Membership runs through the links, and entries are reached by the characters those links carry.

## `private const string LStemArchiveTally`

The count of entries of the series language whose headword carries one of its characters.
It is spliced into every read, so a series is never listed without its count.

## `public LStemArchive(LDatabase database)`

Keeps the database the sessions are started on.

## `public void LStemApply(string language, string character, IReadOnlyList<string> keys)`

Relinks the character to the series named and clears whatever series then holds nothing.
A character with no stored Shengfu row is left alone, since there is nothing to link from.

## `public void LStemRebuild(string language, string separator)`

Drops the series of the language and links every stored Shengfu row again.
It is the path an older workspace takes, where series were fetched before they were linked.

## `public IReadOnlyList<LStem> LStemRead(string language)`

The series of the language in the order they were first made, each with its count.

## `public LStem? LStemRead(long stemId)`

The series row of that identity, or null when none carries it.

## `public LStem? LStemFind(string language, string key)`

The series row of that language and key, or null while nothing links to it.

## `public IReadOnlyList<string> LStemCharacterRead(long stemId)`

The characters linked to the series, in the order their Shengfu rows were stored.

## `public IReadOnlyList<long> LStemEntryScan(string language, IReadOnlyList<long> stemIds)`

The entries of the language whose headword carries a character of every series named.
Counting the distinct series matched keeps the answer an intersection, as a cell of the rime table is.

## `private static void LStemCharacterApply(...)`

Clears the links of one character's Shengfu row and writes one link per series key.

## `private static long? LStemShengfuRead(LDatabaseSession session, string language, string character)`

The identity of the character's stored Shengfu row, or null while it has none.

## `private static long LStemRowCreate(LDatabaseSession session, string language, string key)`

The identity of the series row of that language and key, made first when it was not stored.

## `private static void LStemLinkCreate(LDatabaseSession session, long shengfuId, long stemId)`

Links one character's Shengfu row to one series, leaving a link already held as it is.

## `private static void LStemOrphanClear(LDatabaseSession session, string language)`

Drops every series of the language nothing links to any more.

## `private static LStemRowRead(SqliteDataReader reader)`

Reads one series row with its counted entries off the reader.
