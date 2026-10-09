# LStemClerk.cs
Hash: `8ecbbb642cd1943d`

## `public sealed class LStemClerk`

The read side of the phonetic series the xiesheng panel browses by.
It writes nothing.
The links are made where a series is stored, in `LShengfuClerk`.

## `public LStemClerk(LRig rig)`

Keeps the series store and the entry store it answers over.

## `public IReadOnlyList<LStem> LStemClerkRead(string language)`

The stored series of the language, each with the count of entries it reaches.

## `public LStem? LStemClerkRead(long? id)`

The series row that identity names, or null when nothing was chosen.

## `public LStem? LStemClerkFind(string language, string key)`

The series row of that language and key, or null while the series was never stored.

## `public IReadOnlyList<LStem> LStemClerkFind(string language, string query, LCatalogOrder order)`

The series of the language as the xiesheng column lists them, narrowed by the query and sorted by the order.
It delegates to the shared catalog rule in `LCatalogClerk`, so the engine only marks the chosen row.

## `public IReadOnlyList<LEntry> LStemEntryScan(string language, IReadOnlyList<long> stemIds, string query, LCatalogOrder order)`

The entries the series reach, narrowed by the query the list was typed into.
They come back in `order` through `LCatalogEntry.LCatalogEntrySort`, the owner every entry list shares.
The archive reads in storage order, so no caller may show its rows unsorted.

## `public LStemPage LStemPageRead(LStem stem)`

The page of one series, holding its language, its key and the characters linked to it.
The characters follow `LGlyphOrder`, by code point, so every view lists them alike.
The store hands them in the order their Shengfu rows were saved, which is only fetch order.
