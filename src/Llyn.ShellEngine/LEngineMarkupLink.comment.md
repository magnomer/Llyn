# LEngineMarkupLink.cs

## `public sealed partial class LEngine`

The links a markup file makes by name, and the sense pass that closes them after the writes.
A file names an entry by headword and language and a sense by position path.
A reference is named by title and year.
The import itself stands in `LEngineMarkup.cs`.

## `private IReadOnlyList<LEntry> LEngineMarkupFind(string headword, string language)`

The stored entries with this headword and language, both letter case folded, through the archive's own query.
The file dialog asks once per file entry and the import once per link.
A table scan here would not do.

## `private LExampleDraft LEngineMarkupResolve(LMarkupExample example, int line, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions, List<(LMarkupMention, int)> held)`

An example with its mentions and reference settled.
A mention outside the text, of no length, or over an earlier mention is dropped and named by its offset.
A mention with no headword is kept as a span with no entry.
A mention whose entry cannot be found is dropped with an omission.
A mention that names a sense is held for the sense pass under a negative draft id.
The id is the negative of its place in `held`, so the update's identity map answers the stored mention id.

## `private static bool LEngineMarkupCheck(LMarkupMention mention, int length, IReadOnlyList<LMentionDraft> kept)`

Whether the span fits a text of `length` code points and clears every span already kept.
The sums are taken in `long`, since an offset near `int.MaxValue` would wrap negative and pass.
The archive refuses the same shapes with an exception, so the check keeps a bad file from failing the import.

## `private void LEngineMarkupSettle(IReadOnlyList<(LMarkupMention, int)> held, IReadOnlyDictionary<long, long> identity, IReadOnlyDictionary<(string, string), long> prepared, List<LMarkupOmission> omissions)`

The sense pass, run once every entry of the file is stored.
Each held mention that reached the store is pointed at the sense its path names.
The path is walked down the target's tree as it now stands.
A path that names nothing leaves the mention on its entry and reports the path at the entry's line.
A held mention the store never took, such as one in a sentence Append already had, is passed over.

## `private long LEngineMarkupResolve(string headword, string language, IReadOnlyDictionary<(string, string), long> prepared)`

The entry a headword and language name, or 0.
The file's own entries are asked first, and a headword the file holds twice answers 0.
The workspace is asked next, and only exactly one hit counts.

## `private long LEngineMarkupResolve(long entryId, string sense)`

A sense path such as `1.2` walked down the target's meaning tree, one-based at each step.

## `private long LEngineMarkupResolve(LMarkupReference reference)`

A stored reference with the same normalized title and year, else one created with its authors.
A reference with no title is matched by url instead, and one with neither is always created.
A blank key would otherwise match the first blank reference in the store.

## `private bool LEngineMarkupResolve(string location, List<LMarkupOmission> omissions)`

Whether the location may be kept, by the rule `LEngineLocationResolve` states.
A refused location is reported and answers false, so the caller drops or blanks it.
A web address is never checked, and a local file that is missing is reported but kept.
