# LLocalizationVault.cs

## `public interface LLocalizationVault`

The port for interface-language catalogs, the texts the program shows in one language.
`LLocalizationLoader` in Infrastructure is its adapter over the catalogs embedded in the build.
The engine and the bootstrap read a catalog through it and never learn where it is kept.

## `IReadOnlyList<string> LLocalizationScan();`

The languages the build carries a catalog for, in ordinal order.

## `IReadOnlyDictionary<string, string> LLocalizationRead(string language);`

The raw pairs of the catalog of `language`, `terms.*` keys beside the text keys, references still unresolved.
The adapter owns the file format, so the ring above resolves the pairs and never sees the JSON.
A language the build carries no catalog for raises an exception of the adapter's own.
