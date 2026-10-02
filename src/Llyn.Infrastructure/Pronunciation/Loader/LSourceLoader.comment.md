# LSourceLoader.cs

## `internal static class LSourceLoader`

The source side of the pack loader: how one declared source is read out of the JSON.
A source is a name and a list of attempts.
Each attempt is a set of URLs and the readings to extract from the page.
The same shape serves the `pronunciation`, `audio`, `frequency` and per-scheme `sources` lists.
So a list is read by key, and the key alone says what the sources are for.
A source with no usable attempt is dropped rather than failing the pack.
So is an attempt with no URL or no reading.
`LLanguageLoader`, `LGlyphLoader` and `LSchemeLoader` call it.

## `public const string LSourceSources = "sources";`

The key of the source list a scheme or a glyph section carries.
`LSchemeLoader` and `LGlyphLoader` read it.

## `private const string LSourceReadings = "readings";`

The key of an attempt's list of readings.

## `private const string LSourceFollow = "follow";`

The key of an attempt's follow reading.

## `private const string LSourceBands = "bands";`

The key of a source's pattern bands.

## `private const string LSourceOnce = "once";`

The key of a source's object of once figures.

## `private const string LSourceDecode = "decode";`

The key of an attempt's decode switch.

## `private const string LSourceUnit = "unit";`

The key of a frequency source's unit label.

## `public static IReadOnlyList<LSourceSpec> LSourceSpecScan(JsonElement root, string key, IReadOnlyList<LRespellingRule> spelling)`

The pack declares its sources under `pronunciation`, `audio` and `frequency`, and this reads one of those lists.
No list is required, and a missing one simply reads as empty.
A source declares no kind, because the list it sits in already says what it is for.
The `frequency` list has the same shape, so a figure is fetched the way a transcription is.
`normalize` defaults off there as everywhere, so the raw figure survives untouched.

## `private static LSourceSpec? LSourceSpecRead(JsonElement source, IReadOnlyList<LRespellingRule> spelling)`

One source row: its `name` and its `attempts`.
A row without a name or without a usable attempt reads as nothing.
A frequency source also carries its `once` figures, its `unit` and any pattern `bands` for figures no interval grades.

## `private static double? LSourceFigureRead(JsonElement rule, string key)`

One positive number under the source's `once` object, or `null` when the key is absent or not positive.
The keys are `total`, `factor` and `base`, see [LSourceSpec](../../../Llyn.Core/Pronunciation/Source/LSourceSpec.comment.md).

## `private static IReadOnlyList<LBand> LSourceBandScan(JsonElement source)`

The source declares its pattern bands under `bands`, in the order they are tried.
A missing block reads as no bands, and a figure the ladder cannot grade then shows without a label.
Order is kept because the first matching band wins.

## `private static LBand? LSourceBandRead(JsonElement row)`

A band row carries a `name` and a `match` regex.
A numeric ceiling or floor is not a band, since the shared ladder grades every figure with an interval.
A row with a blank name, no regex, or a regex that fails to compile is skipped.
The regex is compiled once here so one typo never blanks the pack.

## `private static LSourceAttempt? LSourceAttemptRead(JsonElement row)`

One attempt: its `urls`, its readings, and the optional `confirm`, `headers`, `prefix`, `follow` and `decode` keys.
An attempt needs at least one URL and one reading, or it is dropped.

## `private static IReadOnlyList<LSourceReading> LSourceReadingScan(JsonElement row)`

An attempt declares its extractions either as a `readings` list or as flat keys on the attempt itself.
The flat form is the legacy shape and reads as one reading with an empty variety tag.
Audio attempts always use the flat form, so the harvest path sees one untagged reading.
A reading row without a strategy is dropped, and an attempt with no readings left is dropped too.

## `private static LSourceReading? LSourceReadingRead(JsonElement element)`

One reader serves both the flat attempt and a row of its `readings` list, because the keys are the same.
`variety` names a variety the pack declares, or is absent for an untagged reading.
`skip` counts earlier matches to pass over, so a page can yield its second variety from its second span.
`every` takes every match from there on instead of the first alone, for a page listing one reading per etymology.
All three default to empty, zero and off.

## `private static LSourceReading? LSourceFollowRead(JsonElement row)`

An attempt may carry a `follow` object shaped like one reading.
It names how the page's pointer to another headword is captured.
The captured value is a headword and never a transcription.
So IPA normalization is switched off on it whatever the pack wrote.
An attempt without it, or with a follow row lacking a strategy, never follows.
