# TLocalizationLoader.cs
Hash: `aef25aefe3308c74`

## `public sealed class TLocalizationLoader`

The embedded catalog files.
A shipped language reads as raw pairs, and one the build lacks is refused.

## `public void LocalizationLoaderRead_EveryCatalog_MatchesDefaultKeys()`

Every embedded catalog must carry exactly the keys of the default one.
Markup binds texts as dynamic resources, so a key one catalog lacks draws a blank control, not an error.
