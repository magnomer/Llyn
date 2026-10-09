# LInflectionLoader.cs
Hash: `b1eca5438465a8cd`

## `internal static class LInflectionLoader`

The `inflection` side of the pack loader.
Its book predicts the regular forms of a paradigm and lays out the inflection box.
The book becomes one [LInflectionBook](../../../Llyn.Core/Lexicon/Inflection/LInflectionBook.comment.md), or null without one.
Books may be inline objects or separate files referenced by the pack.
`LLanguageLoader` calls it.

## `private const string LInflectionKey = "inflection";`

The key under which the pack names the file, and the key the file may wrap its book in.

## `public static LInflectionBook? LInflectionPackRead(string language, JsonElement root)`

Reads the `inflection` key.
A file name opens that file in the pack folder through `LPackFile`, and an object is read in place.
The file holds either the book itself or an object wrapping it under `inflection`.
The stamp hashes the whole raw book before unwrapping, so any edit to it invalidates stored analyses.
Missing keys, missing referenced files, unsupported values, and books without usable kinds or stems return null.
A null book leaves the pack's forms to the paradigm's own `regular` rules.

## `private static IReadOnlyList<LInflectionKind> LInflectionKindScan(JsonElement root)`

The `kinds` rows in written order, each a `name` and a `match` regex.
Names are trimmed, and rows with empty names, empty patterns, or invalid regexes are skipped.

## `private static IReadOnlyList<LInflectionStem> LInflectionStemScan(JsonElement root, IReadOnlyList<IReadOnlyList<long>> columns)`

The `stems` rows in written order.
Each row holds its `values`, its `templates` keyed by kind name, and one ending per column.
Non-string endings omit their columns without shifting later endings.
Template keys remain ordinal identifiers, and their correspondence to declared kinds is not validated.
A row whose ending count differs from the column count is skipped.
Its endings would land in the wrong cells.
A row without values, templates or a single ending is skipped as well.

## `private static IReadOnlyList<LInflectionRule> LInflectionRuleScan(JsonElement root, string key)`

The rules or folds under `key`, in written order, since each runs on the output of the one before.
Rows contain two or three strings, preserving an optional kind identifier without validating that it exists.
Prediction enforces kind identifiers, while comparison folds do not.
A row of another shape, with an empty pattern, or with a broken regex is skipped.

## `private static LInflectionLayout? LInflectionLayoutRead(JsonElement root)`

The `layout` object, naming the part of speech it serves and its `collapsed` and `expanded` sheets.
A layout without a positive part or without both sheets reads null, and no box is drawn.
Those two sheets are the default pair, shown when custom analysis is off.
The custom pair comes from `custom`, and without one the default pair stands in for it.

## `private static LInflectionLayout? LInflectionCustomRead(JsonElement layout, long code)`

The `custom` object of a layout, holding its own `collapsed` and `expanded` sheets.
It targets the same part as the default pair.
A missing `custom`, or one without both sheets, reads null.

## `private static LInflectionSheet LInflectionSheetRead(JsonElement sheet)`

One view of the box.
The `headers` are kept as written, each a localization key, and a non-string header reads empty.
The `columns` are the cells a line takes when it lists none of its own.

## `private static IReadOnlyList<LInflectionLine> LInflectionLineScan(JsonElement groups, IReadOnlyList<IReadOnlyList<long>> columns)`

Flattens the groups into lines, in written order.
The group label rides on its first line only, and the lines after it carry an empty group.
Each cell unions line values with explicit cell values, falling back to sheet columns when cells cannot be read.
The codes of a cell are distinct and ascending, so a cell compares with a slot's codes directly.
A line without valid `values` is skipped.
