# LStemClerk.cs

## `public sealed class LStemClerk`

The read side of the phonetic series the xiesheng panel browses by.
It writes nothing: the links are made where a series is stored, in `LShengfuClerk`.

## `public LStemClerk(LRig rig)`

Keeps the series store and the entry store it answers over.

## `public IReadOnlyList<LStem> LStemClerkRead(string language)`

The stored series of the language, each with the count of entries it reaches.

## `public LStem? LStemClerkRead(long? id)`

The series row that identity names, or null when nothing was chosen.

## `public LStem? LStemClerkFind(string language, string key)`

The series row of that language and key, or null while the series was never stored.

## `public IReadOnlyList<long> LStemClerkScan(string language, IReadOnlyList<long> stemIds)`

The identities of the entries the series reach.

## `public IReadOnlyList<LEntry> LStemEntryScan(string language, IReadOnlyList<long> stemIds, string query)`

The entries the series reach, narrowed by the query the list was typed into.

## `public LStemPage LStemPageRead(LStem stem)`

The page of one series: its language, its key and the characters linked to it.
