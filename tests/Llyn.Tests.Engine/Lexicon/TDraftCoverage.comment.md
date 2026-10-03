# TDraftCoverage.cs
Hash: `9dba35a561f37211`

## `public sealed class TDraftCoverage`

Keeps every draft field on every side that carries a draft.
A new record field must be named by the portrait, the markup and the exemplar, or waived by name.
Naming is a word match over the side's sources, so a positional argument never counts.
The settings and every waiver reason live in `TDraftCoverageSetting`.

## `public void DraftCoverage_Portrait_NamesEveryProperty()`

The portrait side: the `Portrait` folders of the core and the application, and the engine's portrait facade.

## `public void DraftCoverage_Markup_NamesEveryProperty()`

The markup side: the `Markup` folder of the core and the application's markup clerk.

## `public void DraftCoverage_Exemplar_NamesEveryProperty()`

The exemplar side: the one file that builds the shared exemplar entry.

## `public void DraftCoverage_Setting_MatchesTheRecords()`

Every listed type must be a record in the core, and every shared waiver must still be a property.

## `private static void TDraftCheck(string side, IReadOnlyList<string> include, IReadOnlyList<string> waiver)`

A setting that lists no draft type has nothing to hold, so the check returns at once.
A shared waiver is skipped outright.
A side waiver that the side names anyway is stale, as is one naming no property.
The report lists the unnamed properties by record, then the stale waiver lines.

## `private static Dictionary<string, IReadOnlyList<string>> TDraftRead()`

Reads the listed records from the loaded core assembly, so the compiled shape is the truth.
A type counts as a record only when it carries the compiler's equality contract.
Only primary constructor parameters count, so a derived property such as `LCardDraftEmpty` never enters the test.
The copy constructor is skipped, since its one parameter is the record itself.

## `private static string TSourceResolve()`

Walks up from the test binaries to the folder holding `Llyn.slnx`, since the sides are read as sources.

## `private static IReadOnlyList<string> TSourceFind(string root, IReadOnlyList<string> include)`

A pattern with a star searches its folder and every subfolder, as a git pathspec star also crosses `/`.
A pattern without a star names one file.
Build output under `bin` or `obj` is never a side.
The files come back in ordinal path order, so the joined text is stable.

## `private static string TSourceRead(string root, IReadOnlyList<string> include)`

Joins every matched file, refusing an empty match as a stale pattern.

## `private static bool TSourceMatch(string text, string name)`

True when the name stands as a whole word somewhere in the text.
