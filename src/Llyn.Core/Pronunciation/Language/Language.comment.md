# Language

This folder holds the loaded language pack record and the schemes, varieties and bands it declares.

## `LLanguage`

A loaded language pack, read from `languages/<Lang>/source.json`.
Separate source lists support transcription lookup, audio harvesting, frequencies and morphology.
A pack that declares a section carries its records, and one that omits it carries an empty list or null.
Absent sections turn their rows or boxes off, and nothing is fetched or shown for them.

## Flag, fonts and typography in `LLanguage`

The flag is an ISO 3166-1 alpha-2 country code, or null when the pack declares none.
A code names no shipped image.
The engine downloads the flag from the flag-icons set on demand and caches it in the workspace.
A flag ending in `.svg` names a file in the pack folder, for a language no country flag stands for.
The headword, example and gloss typographies are separate.
A headword is read as a specimen and an example as prose.
A gloss takes the pack of the sentence it renders, since it is read beside that sentence.
A pack that declares a typography none carries a blank record, and the theme's own typography stands.

## Sources and schemes in `LLanguage`

The lookup and harvest source lists are independent.
A site good for transcriptions need not serve audio, and either list may stand empty.
Schemes appear in the order the form shows them, and an empty list turns the transcription rows off.
The scheme list seeds new rows only, and a stored row keeps its scheme after the pack changes.
Frequencies are asked from every declared source, and one row is stored per source that answered.
A fetched figure earns the name of the first band of its source that matches.
Each morphology reading names a slot key, either one decimal code or ascending codes joined with `+`.

## Writing and varieties in `LLanguage`

The `separator` key states whether words are spaced, and a pack that omits it is read as separated.
Japanese, Mandarin and Cantonese declare `none`, so a word there is a run of one script.
A reading names its variety by one of the declared names, and a pack with none leaves every reading untagged.
The record defaults to flagged varieties, but the loader requires `varieties.shown` to explicitly name `flag`.
The `tonal` key makes the UI draw a tone contour under each IPA reading.
The contour comes from the tone marks the reading carries.
The `silent` key hides the pronunciation rows, so no reading is typed, looked up or downloaded.
The `phonemic` key shows a respelled reading between slashes, since such a pack's respelling groups collapse allophones.
The brackets stay square while the original reading is shown, and in a pack that omits the key.

## Rewrites in `LLanguage`

Cleanup groups always run on a reading, after the built-in cleanup and before the trove caches it.
They exist for site quirks the built-in step cannot know, such as a source writing `ɡ` as `g`.
Respelling groups recast a cleaned transcription into the pack's preferred symbol convention.
They run only when the user switches respelling on.

## Han, fanqie and reflex sections in `LLanguage`

The glyph section turns the glyph row on for a language written in Han characters.
Script styles give one row each above the first meaning, in written order.
Fanqie books give one block each under the script box, in written order.
A book holding a ranked row moves ahead, as `LFanqieRow.LFanqieRowSort` declares.
The hypothesis makes the fanqie box print each placement's reading before the placement.
Reflex rules fill an entry with no reflex rows once, one rule per borrowing language.
Their written order is the fetch order only and never the order rows are shown in.
Anatomy rules and tone descent rows are empty for every pack but Classical Chinese.
Its files `anatomy.json` and `anatomy_tone.json` hold them.
The inflection book predicts the regular forms of a paradigm and marks where a stored form departs from them.
It is null for a pack without the key or without a usable kind or stem.
Such a pack's forms keep the paradigm's own `regular` rules.

## `LLanguageReflexOrder`

The declared reflex order, never null.
A pack without an `order` key sorts by the empty order, so rows list by language name.

## `LLanguageLayout`

How the inflection box shows this pack's forms, taken from its rule book.
Null when the pack has no book or its book gives no layout.

## `LScheme`

One transcription scheme a language pack declares, such as Jyutping or Pinyin.
A transcription row names its scheme by this name, and the pack lists where that scheme can be looked up.
A scheme declared as a bare name carries an empty source list and is typed by hand.
Its lookup finds nothing.

## `LVariety`

One regional variety a language pack declares, such as British or American.
A reading names its variety by this name, and the UI draws it by this flag.
The flag is null when the pack declares none, and the name is shown as text instead.

## `LBand`

One frequency band a language pack declares for a figure the shared ladder cannot grade.
A numeric figure with a word interval is graded by `LFrequency` and never reaches these bands.
The pack lists bands under each frequency source in order, and the first regex matching the raw figure wins.
So a pack labels only codes and levels, such as HSK 5 or S1, and a rank of unranked.
The name must be one of the four ladder names, so a level reads the same as a graded figure.

## Stamp

Commit: `c2f335c331fe`
- `LBand.cs` `6f106042a7418ad3`
- `LLanguage.cs` `e8ecf4b137a508ad`
- `LScheme.cs` `fac16e5145ab1d73`
- `LVariety.cs` `5db7d2bc4073274d`
