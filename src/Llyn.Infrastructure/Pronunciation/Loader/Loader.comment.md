# Loader

This folder reads a language pack from `languages/<Lang>/source.json` into the language record.
Packs resolve against the application's base directory, so adding or editing a language needs no recompile.
`LLanguageLoader` composes the record, and one narrow loader handles each pack section.
The loaders of the sections only a Han language declares sit in the `Han` subfolder.
It stands apart from Pack because those helpers are shared readers, and these loaders each own a section.

## `LLanguageLoader`

A missing or malformed pack yields a language with both source lists empty rather than throwing.
So one bad pack never breaks the app.
The loaded `LLanguage` carries every language-specific fact onward.
The sections are split across the narrow loaders.
  - `LSourceLoader` reads one declared source, its attempts and its readings.
  - `LFanqieLoader`, `LShengfuLoader`, `LReflexLoader`, `LScriptLoader` and `LHypothesisLoader` read the Han sections.
  - `LAnatomyLoader` reads the `anatomy` rules and `LDescentLoader` the `tone` rows.
  - `LRespellingLoader` reads rewrite rules and `LSchemeLoader` the transcription schemes.
  - `LFontLoader` reads typography and `LGlyphLoader` the `glyph` section.
  - `LInflectionLoader` reads the inflection book.
  - `LEnvelope` reads the posted form and request headers.
  - `LEnsignLoader` downloads flags and caches them in the workspace.

### Construction and port methods

The constructor checks the workspace root and the client, then hands both to the `LEnsignLoader` it builds.
The packs themselves are read beside the program, not under the root.
The flag read and find, the listing, the pack read and the name check each answer the port.
Each does so through a shared method.

### `LLanguageLoaderScan`

It scans the `languages/` folder and returns the name of every language that has a `source.json`.
A pack whose `listed` key is `false` is left off, though it still loads by name.
The set of languages is whatever is on disk.
So adding a pack folder makes it selectable with no code change.
`LLanguageListedCheck` opens one file to ask.
A file that cannot be opened or parsed counts as listed, and the loader reports it as blank later.
`LLanguageListedRead` is true unless `listed` is a literal `false`.

### `LLanguageSort`

English always heads the language list, and every other pack follows in ordinal order.
The order is a rule about the language set, so it is settled here, not in the UI.

### `LLanguageNameValidate`

A language name must be a plain folder name a pack could sit under.
A separator, a rooted path, a dot name or a character no file name may hold fails.
A name comes from an entry, and an entry can come from an imported file.
Every loader that joins the name onto `languages/` asks here first, so no name walks out of it.

### `LLanguageLoaderLoad`

A name the validation refuses reads as a blank pack, so no name opens a file outside `languages/`.
The file is read anew on every call, so any caching belongs to the caller.

### `LLanguageRead`

The top-level `tonal` key switches the tone contour on only for a literal `true`.
The `silent` key hides the pronunciation rows.
The `phonemic` key puts a respelled reading between slashes.
Each acts only for a literal `true`.
The `anatomy`, `tone`, `order` and `inflection` keys are read by their own loaders.

### Pack options

`LLanguageEmblemRead` reads the `flag` value as a country code.
A value ending in `.svg` reads instead as the full path of that file in the pack folder.
A classical language has no country, so its pack ships its own emblem.
The varieties sit under `varieties.list`, each with a name and a flag code.
The flag is an ISO 3166-1 alpha-2 code resolved through the same workspace path as the language flag.
A missing block reads as no varieties, and a repeated name keeps its first row.
`LLanguageFlaggedCheck` reads `varieties.shown`, `flag` for the flag image or `text` for the name.
The default is `text`, so a pack that lists varieties without choosing shows names.
`LLanguageSeparatorRead` is on unless the pack writes `none`.
A language whose words carry no spaces turns the reading separator off there.

## `LEnsignLoader`

It fetches a language pack's flag image and caches it under the workspace.
A network fetch is a different job from parsing a pack, so it sits apart from `LLanguageLoader`.
The flags come from the flag-icons set, served over jsDelivr's CDN of the repo.
The 4x3 SVGs match the flag box's aspect, and the leaf is the lowercased ISO 3166-1 alpha-2 code.
`LLanguageLoader` keeps its own `.svg` key for a pack-shipped emblem, because each type owns its own key.

### `LEnsignFileRead`

It downloads the flag into the workspace's `flags` folder on first use and serves the cached copy thereafter.
It returns `null` when the download fails or the client times out, so a missing flag never blocks the UI.
It returns `null` as well when the flag folder or file cannot be made, read or written.
The caller then draws the name without its flag.
Any other fault still reaches the caller.
A cancellation the caller requested still propagates.

### `LEnsignFileFind`

It answers the path the read would answer, when that file already exists, else `null`.
It creates no folder and downloads nothing.

### `LEnsignCodeNormalize`

The code is made safe as one lowercased file name segment, the leaf the flag set names its files by.

## `LFontLoader`

It reads the typography blocks a pack declares, each naming a family, a size and a style.
The word, example and gloss blocks share one reader, since they carry the same shape.
A block the pack omits reads as blank, and the theme's own typography stands.
`LFontBlank` stands for no typography, and `LLanguageLoader` also uses it for a pack it cannot read.

## `LInflectionLoader`

It reads the book that predicts the regular forms of a paradigm and lays out the inflection box.
The book becomes one book, or null without one.
Books may be inline objects or separate files referenced by the pack.
`LInflectionPackRead` opens a file name through `LPackFile`, and reads an object in place.
The file holds either the book itself or an object wrapping it under `inflection`.
The stamp hashes the whole raw book before unwrapping, so any edit to it invalidates stored analyses.
Missing keys, missing files, unsupported values, and books without usable kinds or stems return null.
A null book leaves the pack's forms to the paradigm's own `regular` rules.
Kinds are `name` and `match` rows in written order.
Rows with empty names, empty patterns or invalid regexes are skipped.
Stem rows hold their `values`, `templates` keyed by kind name, and one ending per column.
Non-string endings omit their columns without shifting later endings.
Template keys remain ordinal identifiers, and their correspondence to declared kinds is not validated.
A row whose ending count differs from the column count is skipped.
Its endings would land in the wrong cells.
A row without values, templates or a single ending is skipped as well.
Rules and folds are read in written order, since each runs on the output of the one before.
A row holds two or three strings, preserving an optional kind identifier without validating that it exists.
Prediction enforces kind identifiers, while comparison folds do not.
A row of another shape, with an empty pattern, or with a broken regex is skipped.
The `layout` object names the part of speech it serves and its `collapsed` and `expanded` sheets.
A layout without a positive part or without both sheets reads null, and no box is drawn.
Those two sheets are the default pair, shown when custom analysis is off.
The custom pair comes from `custom` and targets the same part.
Without a usable `custom`, the default pair stands in for it.
A sheet keeps its `headers` as written, each a localization key, and a non-string header reads empty.
Its `columns` are the cells a line takes when it lists none of its own.
Lines flatten the groups in written order.
The group label rides on its first line only, and the lines after it carry an empty group.
Each cell unions line values with explicit cell values, falling back to sheet columns when cells cannot be read.
The codes of a cell are distinct and ascending, so a cell compares with a slot's codes directly.
A line without valid `values` is skipped.

## `LRespellingLoader`

It reads `cleanup`, `respelling`, `spelling` and the `rewrite` arrays of a row.
The two rewrite blocks and the spelling list share one rule reader.
A row's rewrite array has its own rewrite reader.
`LAnatomyLoader` and `LHypothesisLoader` call its rule reader.
`LReflexLoader` and `LScriptLoader` call its rewrite reader.

### `LRespellingPackScan`

One reader serves `cleanup` for the always-on groups and `respelling` for the switched ones.
The blocks carry the same shape, a list of groups each with optional varieties and rules.
A missing block reads as an empty list.
When the pack declares varieties, a group without a non-empty `varieties` list is dropped.
It is dropped the same way a broken regex is, so nothing shared can load for English.
A pack without varieties keeps such groups, so Spanish works without the key.

### Group rows

A group's `rules` rows are two-string arrays, and a row of any other shape is skipped.
An absent or empty `varieties` list applies the group to every reading, in a pack without varieties only.
A group with no valid rule is dropped.
A group whose regex fails to compile is dropped too, so one typo never blanks the pack.
The record constructor throws on the bad pattern, and the rule scan catches it and voids that group's list.

### Spelling and rule scans

`LRespellingSpellingScan` reads the pack's `spelling` list as ordered rewrite rules.
They recast a headword into the spelling the pack's sources key on, such as a Latin word without its macrons.
The rules are stamped on every source spec the pack declares, so one list serves every lookup kind.
`LRespellingRuleScan` reads `[pattern, replacement]` pairs and skips any row of another shape.
A pattern the regex engine rejects voids the whole list, so a broken pack rewrites nothing rather than half.
`LRespellingRewriteScan` reads the `rewrite` array, or another key such as `recast`.
A pair whose pattern does not compile is skipped rather than failing the pack.

## `LSchemeLoader`

It reads the `transcription` list.
The schemes read their sources the way a pronunciation source is read.
A scheme is either a bare name or an object carrying a `name` and a `sources` list.
A bare name carries no sources, and the row for it is typed by hand.
The schemes keep the order the form shows them.
A missing or empty list turns the transcription rows off for that language.
Blank and repeated names are dropped, so the form never shows a nameless or doubled scheme.

## `LSourceLoader`

It reads one declared source out of the JSON.
A source is a name and a list of attempts.
Each attempt is a set of URLs and the readings to extract from the page.
The same shape serves the `pronunciation`, `audio`, `frequency`, `morphology` and `sources` lists.
So a list is read by key, and the key alone says what the sources are for.
A source declares no kind, because the list it sits in already says what it is for.
No list is required, and a missing one reads as empty.
A source with no usable attempt is dropped rather than failing the pack.
So is an attempt with no URL or no reading.
`normalize` defaults off, so a raw frequency figure survives untouched.
`LSourceSources` is the key of the source list a scheme or a glyph section carries.

### Source rows

A row has a `name` and its `attempts`.
A row without a name or without a usable attempt reads as nothing.
A frequency source also carries its `once` figures, its `unit` and any pattern `bands` for figures no interval grades.
Each once figure is a positive number under `total`, `factor` or `base`, and otherwise `null`.
Bands are tried in the order declared, and the first matching band wins.
A missing block reads as no bands, and a figure the ladder cannot grade then shows without a label.
A band row carries a `name` and a `match` regex.
A numeric ceiling or floor is not a band, since the shared ladder grades every figure with an interval.
A row with a blank name, no regex, or a regex that fails to compile is skipped.
The regex is trial-compiled here so one typo never blanks the pack.

### Attempts and readings

An attempt has its `urls`, its readings, and the optional `confirm`, `headers`, `prefix`, `follow` and `decode` keys.
An attempt needs at least one URL and one reading, or it is dropped.
An attempt declares its extractions either as a `readings` list or as flat keys on the attempt itself.
The flat form is the legacy shape and reads as one reading with an empty variety tag.
Audio attempts always use the flat form, so the harvest path sees one untagged reading.
A reading row without a strategy is dropped, and an attempt with no readings left is dropped too.
One reader serves both the flat attempt and a row of its list, because the keys are the same.
`variety` names a variety the pack declares, or is absent for an untagged reading.
`skip` counts earlier matches to pass over, so a page can yield its second variety from its second span.
`every` takes every match from there on instead of the first alone, for a page listing one reading per etymology.
All three default to empty, zero and off.

### Follow

An attempt may carry a `follow` object shaped like one reading.
It names how the page's pointer to another headword is captured.
The captured value is a headword and never a transcription.
So IPA normalization is switched off on it whatever the pack wrote.
An attempt without it, or with a follow row lacking a strategy, never follows.

## Stamp

Commit: `c2f335c331fe`
- `LEnsignLoader.cs` `8fdf4e9ef84e2abd`
- `LFontLoader.cs` `24fdea5b38e44d4d`
- `LInflectionLoader.cs` `b1eca5438465a8cd`
- `LLanguageLoader.cs` `226c6f471d0c1b65`
- `LRespellingLoader.cs` `962f1ea399dc8d38`
- `LSchemeLoader.cs` `12783f0161d9cb8f`
- `LSourceLoader.cs` `79ce8c8c05a6e8fc`
