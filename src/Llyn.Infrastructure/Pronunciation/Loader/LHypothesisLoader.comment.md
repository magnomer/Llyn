# LHypothesisLoader.cs

## `internal static class LHypothesisLoader`

The hypothesis side of the pack loader: the reconstruction tables a Han language pack declares.
The tables become one [LHypothesis](../../../Llyn.Core/Pronunciation/Hypothesis/LHypothesis.comment.md), or `null` without them.
The tables live in their own file beside `source.json`, since a language may grow a system of its own.
`LLanguageLoader` calls it.

## `private const string LHypothesisKey = "hypothesis";`

The key of the tables, holding a file name or the tables themselves.

## `public static LHypothesis? LHypothesisPackRead(string language, JsonElement root)`

Reads the `hypothesis` key: a file name opens that file in the pack folder, an object is read in place.
Any other value, or no key, yields `null`.
The named file is opened through `LPackFile`, and its root is the tables object itself, never wrapped under a key.
A missing or unreadable file yields `null`, and the fanqie box prints the placements alone.

## `private static LHypothesis? LHypothesisSectionScan(JsonElement section)`

Reads the `initial` and `final` tables, the `tone` rules and the `place` list from one object.
An object missing either table yields `null`, since nothing could be resolved from it.

## `private static IReadOnlyDictionary<string, string> LHypothesisTableRead(JsonElement section, string key)`

One table of string to string, keys trimmed, rows with a non-string value skipped.
A missing table reads as empty.

## `private static IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> LHypothesisToneScan(JsonElement section)`

The `tone` object: each key a rime-book tone character, each value the class rows of that tone in order.
A tone with no readable row is left out.

## `private static LHypothesisTone? LHypothesisToneRead(JsonElement element)`

One class row: `onset` the pattern over the onset, `rewrite` the rules in respelling form, `class` the label key.
A missing onset matches every onset, a missing rewrite leaves the syllable as joined.
A row that is not an object, or whose onset pattern is broken, reads `null`.

## `private static IReadOnlyList<LHypothesisLocus> LHypothesisLocusScan(JsonElement section)`

The `place` array: the articulatory loci in file order, each read by the row reader below.
A missing or malformed array reads as no loci.

## `private static LHypothesisLocus? LHypothesisLocusRead(JsonElement element)`

One locus: `name` its key and `initial` the initials it gathers, blanks and repeats dropped.
A row without a name or without any initial reads `null`.
