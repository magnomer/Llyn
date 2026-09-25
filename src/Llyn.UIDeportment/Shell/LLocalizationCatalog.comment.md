# LLocalizationCatalog.cs

## `public static class LLocalizationCatalog`

The interface language read by key, so the deportment words what it shows without the veneer.

## `public static string? LLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
The plain read answers the key itself on a miss, which a caller cannot tell from a translation.

## `public static string LLocalizationTextRead(string key)`

The localized text under `key`, or the key itself when no locale declares it.
