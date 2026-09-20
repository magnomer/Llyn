# LLocalization.cs

## `public static class LLocalization`

The interface language catalog held below the shell.
It keeps one parsed language file and answers every text key from it.
The shell copies the loaded dictionary into its resources, so markup keeps binding by key.
The engine reads it directly where a formatted answer needs a localized pattern.

## `public const string LLocalizationDefault = "en";`

The language every launch starts from before the settings are read.

## `private static readonly object LLocalizationGate = new();`

Guards the current dictionary, since a load and a read may run on different threads.

## `private static IReadOnlyList<string> LLocalizationListed = [LLocalizationDefault];`

The languages the build carries a catalog for, as the localization port lists them.
Until the port is asked, only the default is listed.

## `public static void LLocalizationLanguageSet(IReadOnlyList<string> languages)`

Records the languages the build embeds, read off the localization port by whoever holds it.
The bootstrap sets it before the first catalog is applied, and the engine sets it again on construction.
No language is named in code, so a new catalog file is listed without an edit here.

## `public static string LLocalizationNormalize(string? language)`

The stored language when it is one the catalog ships, else the default.

## `public static bool LLocalizationDefaultCheck(string? language)`

True when the stored language needs no second load after the default.

## `private static bool LLocalizationListedCheck(string language)`

Whether the build embeds a catalog for `language`, under the gate the list is set under.

## `public static CultureInfo LLocalizationCultureRead(string language)`

The culture a language's terms are cased under, which is the predefined culture of that name.
A name no culture answers to is refused.
It does not consult the listed languages, so a reader can be loaded before any vault was scanned.

## `public static IReadOnlyDictionary<string, string> LLocalizationLoad(LLocalizationVault vault, string language)`

Lists the languages the port carries, then reads the catalog of `language` through it as the current one.
The bootstrap loads the default this way before any engine exists.
The engine loads the chosen one the same way.

## `public static IReadOnlyDictionary<string, string> LLocalizationLoad(IReadOnlyDictionary<string, string> raw, string language)`

Resolves the raw catalog pairs handed in and makes the result the current one.
The engine reads the embedded file through `LLocalizationLoader` in the infrastructure.
A test hands in pairs it parsed the same way.
The JSON never reaches this ring, so it opens, parses and closes nothing.

## `public static string LLocalizationTextRead(string key)`

The text of a key, or the key itself when the catalog lacks it.

## `public static string? LLocalizationTextFind(string key)`

The text of a key, or null when the catalog lacks it.
