# LStemClerk.cs
Hash: `a24a6739e7f3a138`

## `public sealed class LStemClerk`

The read side of the phonetic series the xiesheng panel browses by.
It writes nothing.
The links are made where a series is stored, in `LShengfuClerk`.
A member's fold is written by `LFoldClerk`, and only read here.

## `public LStemClerk(LRig rig, LReflexClerk reflex)`

Keeps the series, entry, fanqie and reflex stores it answers over.
It takes the reflex clerk for the pack's reflex order, so members sort their rows as the entry page does.
It keeps the fold store too, to read each member's own fold.

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

## `public LStemPage LStemPageRead(LStem stem, bool fold)`

The page of one series, holding its language, its key and the characters linked to it.
The characters follow `LGlyphOrder`, by code point, so every view lists them alike.
The store hands them in the order their Shengfu rows were saved, which is only fetch order.
Each character comes with its member, built by `LStemMemberScan`.
`fold` says whether members read their own fold.
The series page asks, the Outpost sheet does not.
Without it no fold store is queried, and every member reads closed.

## `public long? LStemEntryFind(LStem stem, string character)`

The entry of one member character, matched as the page matches it, or null for a bare member.
The engine finds the entry a member's fold is written on here.

## `private IReadOnlyList<LStemMember> LStemMemberScan(LStem stem, IReadOnlyList<string> characters, bool fold)`

The members of the series, one per character, in the characters' order.
The entries the series reaches are read in one batch by `LStemEntryRead`.
They are matched to the characters by headword through `LStemMember.LStemMemberFind`.
It only reads, so a character without an entry stays bare and no entry is made for it.
A series with no character or a blank language reads no store and gives every character bare, through `LStemMember.LStemBareScan`.
It hands `fold` on to each member read.

## `private IReadOnlyList<LEntry> LStemEntryRead(LStem stem)`

The entries the series reaches, in id order, or none for a blank language.
The page and the fold write share it, so both match a character to the same entry.

## `private LStemMember LStemMemberRead(LStem stem, string character, long? entry, bool fold)`

One member: the representative readings of the character and the reflex rows of its entry.
It also carries the member's own fold, read from the fold store by entry and series key.
A bare member reads closed, and so does every member when `fold` is false, with no fold query.
The entry id stays local to this read, so the member keeps no entry id.
The reflex rows are read alone, not the whole entry.
They are then put in the pack's order and stripped of unwritten rows.
That is the order and the filter the entry page's reflex table shows.
