# LLanguageLoaderHypothesis.cs

## `public static partial class LLanguageLoader`

The `hypothesis` side of the pack loader: the reconstruction tables a Han language pack declares.
The tables become one [LHypothesis](../../Llyn.Core/Pronunciation/LHypothesis.comment.md), or `null` without them.
The tables live in their own file beside `source.json`, since a language may grow a system of its own.

## `private static LHypothesis? LLanguageHypothesisRead(string language, JsonElement root)`

Reads the `hypothesis` key: a file name opens that file in the pack folder, an object is read in place.
Any other value, or no key, yields `null`.

## `private static LHypothesis? LLanguageHypothesisLoad(string language, string file)`

Opens the named file in the language's pack folder and reads its tables.
The name must be a bare file name, so a pack cannot point outside its folder.
A missing or unreadable file yields `null`, and the fanqie box prints the placements alone.

## `private static LHypothesis? LLanguageHypothesisScan(JsonElement section)`

Reads the `initial` and `final` tables, the `tone` rules and the `place` list from one object.
An object missing either table yields `null`, since nothing could be resolved from it.

## `private static IReadOnlyDictionary<string, string> LLanguageTableRead(JsonElement section, string key)`

One table of string to string, keys trimmed, rows with a non-string value skipped.
A missing table reads as empty.

## `private static IReadOnlyDictionary<string, IReadOnlyList<LHypothesisTone>> LLanguageToneScan(JsonElement section)`

The `tone` object: each key a rime-book tone character, each value the class rows of that tone in order.
A tone with no readable row is left out.

## `private static LHypothesisTone? LLanguageToneRead(JsonElement element)`

One class row: `onset` the pattern over the onset, `rewrite` the rules in respelling form, `class` the label key.
A missing onset matches every onset, a missing rewrite leaves the syllable as joined.
A row that is not an object, or whose onset pattern is broken, reads `null`.

## `private static IReadOnlyList<LHypothesisPlace> LLanguagePlaceScan(JsonElement section)`

The `place` array: the articulatory places in file order, each read by the row reader below.
A missing or malformed array reads as no places.

## `private static LHypothesisPlace? LLanguagePlaceRead(JsonElement element)`

One place: `name` its key and `initial` the initials it gathers, blanks and repeats dropped.
A row without a name or without any initial reads `null`.
