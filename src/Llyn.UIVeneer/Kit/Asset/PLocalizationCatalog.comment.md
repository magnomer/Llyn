# PLocalizationCatalog.cs

## `public sealed class PLocalizationCatalog`

The interface language read as a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.

## `internal static string PLocalizationTextRead(string key)`

The localized text under `key`, or the key itself when no locale declares it.
It hands the read to `LLocalizationCatalog`, which owns the lookup.

## `internal static string? PLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
It hands the find to `LLocalizationCatalog`, which owns the lookup.

## `internal static void PLocalizationCatalogApply(ResourceDictionary resources, IReadOnlyDictionary<string, string> texts)`

Copies every text of a loaded catalog into the resources and announces the change.
The caller loads the catalog, through the engine or, before one exists, through the localization port.

## Inline notes

### `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.
