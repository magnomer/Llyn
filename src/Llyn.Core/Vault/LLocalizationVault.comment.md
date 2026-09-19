# LLocalizationVault.cs

## `public interface LLocalizationVault`

The port for interface-language catalogs, the texts the program shows in one language.
`LLocalizationLoader` in Infrastructure is its adapter over the catalogs embedded in the build.
The engine and the bootstrap read a catalog through it and never learn where it is kept.

## `IReadOnlyList<string> LLocalizationScan();`

The languages the build carries a catalog for, in ordinal order.

## `TextReader LLocalizationOpen(string language);`

A reader over the catalog of `language`, which the caller disposes.
A language the build carries no catalog for raises `InvalidDataException`.
