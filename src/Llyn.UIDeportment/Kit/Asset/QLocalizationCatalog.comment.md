# QLocalizationCatalog.cs

## `public sealed class QLocalizationCatalog`

The interface language read by key, so the deportment words what it shows without the veneer.
It is also a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.

## `public static string QLocalizationTextRead(string key)`

The localized text under `key`, or the key itself when no locale declares it.

## `public static string? QLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
The plain read answers the key itself on a miss, which a caller cannot tell from a translation.

## `internal static void QLocalizationCatalogApply(ResourceDictionary resources, IReadOnlyDictionary<string, string> texts)`

Copies every text of a loaded catalog into the resources and announces the change.
The caller loads the catalog, through the engine or, before one exists, through the localization port.

## Inline notes

### `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.
