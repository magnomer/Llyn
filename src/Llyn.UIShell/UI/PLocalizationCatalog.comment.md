# PLocalizationCatalog.cs

## `public sealed class PLocalizationCatalog`

The interface language read as a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.

## Inline notes

### `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.
