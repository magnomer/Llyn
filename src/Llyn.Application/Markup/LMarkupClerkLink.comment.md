# LMarkupClerkLink.cs
Hash: `c278f6c613892669`

## `public sealed class LMarkupClerkLink`

The links an import resolves: entries by headword, senses by path, references by title and locations by path.
The draft resolver and the intake both lean on it.

## `public LMarkupClerkLink(LRig rig, LReferenceClerk references, LAuthorClerk authors, LTrailClerk trail)`

Reads the entry and meaning ports out of `rig` and keeps the reference, author and trail clerks.

## `public IReadOnlyList<LEntry> LMarkupEntryFind(string headword, string language)`

The stored entries with the headword in the language, both trimmed.

## `public LExampleDraft LMarkupExampleResolve(LMarkupExample example, int line, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int, long)> held, List<(LMarkupReference, long)> cited)`

An example draft from the markup, its mentions checked and their targets resolved.
A mention out of range, overlapping another, or whose headword finds no entry is omitted.
A mention with a sense is held for settling once the entries stand, its draft id a negative slot.
A referenced example is cited under its own negative draft id, its source left unspecified.
The source is found or created only once the example is stored, so a dropped example leaves no reference.
Both holds draw from one counter, so their slots never collide in the shared identity map.

## `public LEtymologyDraft LMarkupEtymologyResolve(LMarkupEntry entry, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions)`

The etymology draft from the markup, both shapes resolved against the entries the import knows.
A source link that names no entry is omitted, and a repeated one is taken once.
A span out of range, overlapping another, or naming no entry is omitted.
The engine keeps only one shape when it writes, so both may be read.

## `public long LMarkupEntryResolve(string headword, string language, IReadOnlyDictionary<(string, string), long> prepared)`

The entry the import is creating under that name, else the single stored match, else zero.

## `public long LMarkupSenseResolve(long entryId, string sense)`

The meaning a dotted position path names under the entry, or zero.

## `public bool LMarkupLocationResolve(string location, List<LMarkupOmission> omissions)`

Whether a media location can stand.
An unresolvable location is omitted and refused.
A missing file is omitted but kept.

## `public long LMarkupReferenceResolve(LMarkupReference reference)`

The stored reference with the same title and year, or with no title the same address.
Else a fresh one is created with its authors.
The intake calls it only for an example it stored, so no reference is created without a citing example.

## `private static bool LMarkupMentionCheck(LMarkupMention mention, int length, IReadOnlyList<LMentionDraft> kept)`

Whether the mention lies inside the text and beside every mention already kept.
