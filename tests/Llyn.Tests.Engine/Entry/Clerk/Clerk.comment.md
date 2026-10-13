# Clerk

This folder holds the tests of the entry, entry query and translation clerks.
They run each clerk alone over a rig of fakes.

## `TEntryClerk.cs`

It covers the entry clerk on its own, over a rig of fakes and no engine.
An entry added to the vault is read back under the id the vault gave it.
A delete drops the row, files a tombstone under a new revision and moves the workspace row onto it.
The revision carries one change naming the deleted headword.
A delete of id zero is an argument error before any store is touched.

## `TEntryClerkSaving.cs`

It covers the entry clerk's create over a rig of fakes.
A draft without a language loads no pack and judges no paradigm.
A draft with only a headword becomes a stored entry and one revision holding one create change.
The workspace row points at that revision.
Two saves, the first carrying a reading, store distinct entries and record one revision per save.
The reading lands on the first entry only, and its create change follows the entry's.
A full draft records the entry's create first and then each child's create, in the order the save writes them.
The rig fakes answer no meaning, so that case runs through a workspace engine.

## `TEntryQueryClerk.cs`

It covers the entry query clerk on its own, over a rig of fakes and no engine.
The ordering is applied over what the vault answered, so a reverse order lists the alphabetically later headword first.

## `TTranslationClerk.cs`

It covers the translation clerk's search and resolve over the entry fake, with no engine and no SQLite.
The fake answers every entry to any query, so the ordering and the exclusions are the clerk's own.
The entry whose whole headword reads as the query leads, and the containing one follows.
The clerk ranks a padded query as its trimmed text, so padding never changes the order.
A card may not translate its own entry, so the search drops the id it is given.
The word read answers the typed text trimmed, and null when only blanks were typed.
Each quill search asks it first, so no card sends a padded or empty word to the search.
Two entries reading the same word, case folded, are a question, so the resolve answers null.
One exact headword resolves, padding trimmed, and a prefix of it resolves to nothing.

## Stamp

Commit: `c2f335c331fe`
- `TEntryClerk.cs` `16a67fe2e7556e44`
- `TEntryClerkSaving.cs` `4da0af21547a1615`
- `TEntryQueryClerk.cs` `7dbfdddb283eb430`
- `TTranslationClerk.cs` `a03bf991367dc4f1`
