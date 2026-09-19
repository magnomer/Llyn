# LLocalizationLoader.cs

## `public static class LLocalizationLoader`

Opens the embedded interface-language file of one language.
The two files are embedded in this project, so `LLocalization` in the application ring opens no resource of its own.
The engine opens through here and hands the reader to `LLocalization.LLocalizationLoad`, which parses and keeps the catalog.

## `public static TextReader LLocalizationLoaderOpen(string language)`

A reader over the embedded file, which the caller disposes once it is parsed.
A language the build does not embed throws `InvalidDataException`.
