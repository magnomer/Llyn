# LLocalizationLoader.cs

## `public sealed class LLocalizationLoader : LLocalizationVault`

Reads the embedded interface-language file of one language into raw pairs.
The two files are embedded in this project, so `LLocalization` in the application ring opens no resource of its own.
The JSON shape is this adapter's alone.
The engine reads through here and hands the pairs to `LLocalization.LLocalizationLoad`, which resolves and keeps the catalog.

## `public IReadOnlyList<string> LLocalizationScan()`

The languages the build embeds a catalog for, read off the manifest resource names in ordinal order.
The list is what `LLocalization` tests a chosen language against, so no language is named in code.

## `public IReadOnlyDictionary<string, string> LLocalizationRead(string language)`

The raw pairs of the embedded file, parsed and closed before the call returns.
A language the build does not embed throws `InvalidDataException`.

## `public static IReadOnlyDictionary<string, string> LLocalizationLoaderParse(string text)`

Lifts one catalog off its JSON as a flat map, `terms.*` keys beside the text keys.
A file opens with `terms.*` entries and closes with one `texts` object, and any other shape is refused.
Every value must be a string and no key may repeat.
Public so a test can parse its own text the way the engine's rig does.

## `private static void LLocalizationLoaderScan(JsonElement element, Dictionary<string, string> pairs)`

Adds every text of the `texts` object, refusing one that wears the term prefix.

## `private static void LLocalizationLoaderAdd(JsonProperty property, Dictionary<string, string> pairs, string kind)`

Adds one string pair, naming its kind in the refusal when it is not a string or repeats.
