# PLocalizationCatalog.cs

## `public sealed class PLocalizationCatalog`

The interface language read as a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.

## `internal static string PLocalizationTextRead(string key)`

The localized text under `key`, or the key itself when no locale declares it.

## `internal static string? PLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
The plain read answers the key itself on a miss, which a caller cannot tell from a translation.

## `internal static void PLocalizationCatalogApply(ResourceDictionary resources, string language)`

Loads the language below the shell, copies every text into the resources and announces the change.

## Inline notes

### `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.
