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

## `public static string LLocalizationNormalize(string? language)`

The stored language when it is one the catalog ships, else the default.

## `public static bool LLocalizationDefaultCheck(string? language)`

True when the stored language needs no second load after the default.

## `public static CultureInfo LLocalizationCultureRead(string language)`

The culture a language's terms are cased under.
A language the catalog does not ship is refused.

## `public static IReadOnlyDictionary<string, string> LLocalizationLoad(TextReader reader, string language)`

Parses a catalog from the reader handed in and makes it the current one.
The engine opens the embedded file through `LLocalizationLoader` in the infrastructure, and a test hands in its own text.
The reader stays the caller's to dispose, so this ring opens and closes nothing.

## `public static string LLocalizationTextRead(string key)`

The text of a key, or the key itself when the catalog lacks it.

## `public static string? LLocalizationTextFind(string key)`

The text of a key, or null when the catalog lacks it.
