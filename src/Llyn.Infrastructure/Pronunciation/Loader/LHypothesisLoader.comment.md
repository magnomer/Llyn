# LHypothesisLoader.cs
Hash: `506d5de38b34cd71`

## `internal static class LHypothesisLoader`

The hypothesis side of the pack loader.
It reads the reconstruction tables a Han language pack declares.
The tables become one [LHypothesis](../../../Llyn.Core/Pronunciation/Hypothesis/LHypothesis.comment.md), or `null` without them.
The tables live in their own file beside `source.json`, since a language may grow a system of its own.
`LLanguageLoader` calls it.

## `private const string LHypothesisKey = "hypothesis";`

The key of the tables, holding a file name or the tables themselves.

## `public static LHypothesis? LHypothesisPackRead(string language, JsonElement root)`

Reads the `hypothesis` key.
A file name opens that file in the pack folder, and an object is read in place.
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

The `tone` object keys are rime-book tone characters.
Each value holds the class rows of that tone in order.
A tone with no readable row is left out.

## `private static LHypothesisTone? LHypothesisToneRead(JsonElement element)`

One class row.
The `onset` is the pattern over the onset.
The `rewrite` holds the rules in respelling form.
The `class` is the label key.
A missing onset matches every onset.
A missing rewrite leaves the syllable as joined.
A row that is not an object, or whose onset pattern is broken, reads `null`.

## `private static IReadOnlyList<LHypothesisLocus> LHypothesisLocusScan(JsonElement section)`

The `place` array holds the articulatory loci in file order.
Each is read by the row reader below.
A missing or malformed array reads as no loci.

## `private static LHypothesisLocus? LHypothesisLocusRead(JsonElement element)`

One locus.
The `name` is its key, and `initial` lists the initials it gathers, with blanks and repeats dropped.
A row without a name or without any initial reads `null`.
