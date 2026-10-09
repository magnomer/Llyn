# LEntryQueryClerk.cs
Hash: `0887fc2f28fecdb4`

## `public sealed class LEntryQueryClerk`

The lookups over stored entries, apart from the lifecycle that writes them.
It reads the entry port of one rig and writes nothing.
The facades, the vista, the courier and the markup intake all search through here.

## `public LEntryQueryClerk(LRig rig)`

Reads the entry port out of `rig`.

## `public IReadOnlyList<LEntry> LEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or all whitespace.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.

## `public IReadOnlyList<LEntry> LEntryHeadwordFind(string headword, string language)`

The stored entries with the headword in the language, both trimmed.

## `public IReadOnlyList<LEntry> LEntryFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`.
The store answers which entries match, and the ordering is applied over what it returned.

## `public IReadOnlyList<LEntry> LEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The ordered search with the entries in a hidden language left out.
The library panel lists through this, so the filter is applied here and never in the shell.

## `private static string? LEntryLanguageRead(LEntry entry)`

The language an entry is filtered by.
Both filtered lookups share it, so the filter never reads the language two ways.

## `public IReadOnlyList<LEntry> LEntryFind(LTag tag)`

Returns the entries carrying `tag`, matched by its id, ordered by headword.
A zero id stands for no tag chosen and returns every entry.
The overload takes a tag rather than text so the two searches cannot be confused.

## `public IReadOnlyList<LEntry> LEntryFind(LRegister register)`

Returns the entries marked with `register`, matched by its id, ordered by headword.

## `public IReadOnlyList<LEntry> LEntryFind(LSituation situation)`

Returns the entries referencing `situation`, matched by its id, ordered by headword.
A zero id stands for no situation chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryFind(LExample example)`

Returns the entries quoting `example`, matched by its id, ordered by headword.
A zero id stands for no example chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryFind(LReference reference)`

Returns the entries citing `reference`, matched by its id, ordered by headword.
A card cites a Source through the Example it holds, so the walk goes through that Example.
A zero id stands for no source chosen and returns every entry.

## `public static IReadOnlyList<LEntry> LEntryMatch(IReadOnlyList<LEntry> entries, string query)`

The entries whose headword matches `query` by the catalog match.
An empty or blank `query` narrows nothing.
The engine's vista narrows a child list the same way, so it calls here.

## `public static IReadOnlyList<LEntry> LEntryMatch(IReadOnlyList<LEntry> entries, LCatalogFilter filter, string query, LCatalogOrder order)`

The entries left after the language filter, narrowed by the catalog match of `query`.
They are ordered by `order` with the entry catalog's sort, so equal headwords follow its tie rule.
The vista lists a catalog record's entries through this, so the shell holds no filter or order rule.

## `public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)`

The epithet of every listed entry that has one, keyed by id, in one session.

## `public long LEntryCountRead()`

How many entries the workspace holds.
