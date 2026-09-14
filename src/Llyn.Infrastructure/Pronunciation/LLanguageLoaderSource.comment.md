# LLanguageLoaderSource.cs

## `public static partial class LLanguageLoader`

The source half of the pack loader: how one declared source is read out of the JSON.
A source is a name and a list of attempts.
Each attempt is a set of URLs and the readings to extract from the page.
The same shape serves the `pronunciation`, `audio`, `frequency` and per-scheme `sources` lists.
So a list is read by key, and the key alone says what the sources are for.
A source with no usable attempt is dropped rather than failing the pack.
So is an attempt with no URL or no reading.

## `private static LSourceSpec? LLanguageSourceRead(JsonElement source)`

One source row: its `name` and its `attempts`.
A row without a name or without a usable attempt reads as nothing.

## `private static LSourceAttempt? LLanguageAttemptRead(JsonElement row)`

One attempt: its `urls`, its readings, and the optional `confirm`, `headers`, `prefix` and `follow` keys.
An attempt needs at least one URL and one reading, or it is dropped.

## `private static IReadOnlyDictionary<string, string>? LLanguageHeaderRead(JsonElement row)`

The request headers an attempt declares under `headers`, string values only.
An absent or empty block reads as null, and the client's default headers alone are sent.

## `private static IReadOnlyList<LSourceSpec> LLanguageSourceScan(JsonElement root, string key)`

The pack declares its sources under `pronunciation`, `audio` and `frequency`, and this reads one of those lists.
No list is required, and a missing one simply reads as empty.
A source declares no kind, because the list it sits in already says what it is for.
The `frequency` list has the same shape, so a figure is fetched the way a transcription is.
`normalize` defaults off there as everywhere, so the raw figure survives untouched.

## `private static IReadOnlyList<LSourceReading> LLanguageReadingScan(JsonElement row)`

An attempt declares its extractions either as a `readings` list or as flat keys on the attempt itself.
The flat form is the legacy shape and reads as one reading with an empty variety tag.
Audio attempts always use the flat form, so the harvest path sees one untagged reading.
A reading row without a strategy is dropped, and an attempt with no readings left is dropped too.

## `private static LSourceReading? LLanguageReadingRead(JsonElement element)`

One reader serves both the flat attempt and a row of its `readings` list, because the keys are the same.
`variety` names a variety the pack declares, or is absent for an untagged reading.
`skip` counts earlier matches to pass over, so a page can yield its second variety from its second span.
`every` takes every match from there on instead of the first alone, for a page listing one reading per etymology.
All three default to empty, zero and off.

## `private static LSourceReading? LLanguageFollowRead(JsonElement row)`

An attempt may carry a `follow` object shaped like one reading.
It names how the page's pointer to another headword is captured.
The captured value is a headword and never a transcription.
So IPA normalization is switched off on it whatever the pack wrote.
An attempt without it, or with a follow row lacking a strategy, never follows.

## `private static LGlyph? LLanguageGlyphRead(JsonElement root)`

The `glyph` section: its `name`, its `language`, and its optional `sources` list and `font` block.
A section missing either the name or the language reads as null, and the glyph row stays off.
The sources list has the shape of a scheme's, so the traditional form is looked up as a transcription is.
A blank font block reads as none, so the chips fall back to the example typography.
