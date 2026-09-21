# LStemVault.cs

## `public interface LStemVault`

The persistence port for the phonetic series the engine browses by.
`LStemArchive` in Infrastructure is its adapter over the workspace database.
It is handed the series keys already cut, so it holds no language fact of its own.

## `void LStemApply(string language, string character, IReadOnlyList<string> keys)`

Links the character to every series named, dropping the links it held before.
Series rows are made as they are first named and cleared once nothing links to them.

## `void LStemRebuild(string language, string separator)`

Builds the series of the language again from every Shengfu row it holds.
The separator is the one the language rule prints joined series with.

## `IReadOnlyList<LStem> LStemRead(string language)`

The stored series of the language, each with the count of entries it reaches.

## `LStem? LStemRead(long stemId)`

The series row of that identity, or null when none carries it.

## `LStem? LStemFind(string language, string key)`

The series row of that language and key, or null while the series was never stored.

## `IReadOnlyList<string> LStemCharacterRead(long stemId)`

The characters linked to the series, ordered as their Shengfu rows were stored.

## `IReadOnlyList<long> LStemEntryScan(string language, IReadOnlyList<long> stemIds)`

The entries of the language whose headword carries a character of every series named.
