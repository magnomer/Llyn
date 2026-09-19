# LLocalizationLoader.cs

## `public sealed class LLocalizationLoader : LLocalizationVault`

Opens the embedded interface-language file of one language.
The two files are embedded in this project, so `LLocalization` in the application ring opens no resource of its own.
The engine opens through here and hands the reader to `LLocalization.LLocalizationLoad`, which parses and keeps the catalog.

## `public IReadOnlyList<string> LLocalizationScan()`

The languages the build embeds a catalog for, read off the manifest resource names in ordinal order.
The list is what `LLocalization` tests a chosen language against, so no language is named in code.

## `public TextReader LLocalizationOpen(string language)`

A reader over the embedded file, which the caller disposes once it is parsed.
A language the build does not embed throws `InvalidDataException`.
