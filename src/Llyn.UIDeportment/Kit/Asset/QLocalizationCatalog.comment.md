# QLocalizationCatalog.cs
Hash: `444b1b62ac35c4ec`

## `public sealed class QLocalizationCatalog`

The interface language read by key, so the deportment words what it shows without the veneer.
It is also a bindable lookup rather than a resource reference.
A binding cannot carry a dynamic resource, so text inside a multi binding would freeze at load.
This one object stands in for the catalog and announces every key at once when the language changes.

## `private static readonly ConditionalWeakTable<ResourceDictionary, ResourceDictionary> QLocalizationCatalogMerged =`

The catalog dictionary last merged into each resource dictionary, so the next apply can find and replace it.
A weak table lets a dropped resource dictionary go without holding its catalog alive.

## `public static QLocalizationCatalog QLocalizationCatalogCurrent { get; } = new();`

The one instance every binding names, so one change event reaches them all.

## `public string this[string key] => QLocalizationTextFind(key) ?? string.Empty;`

Answers empty on a miss, unlike `QLocalizationTextRead`, so a binding never shows a raw key.

## `public static string QLocalizationTextRead(string key)`

The localized text under `key`, or the key itself when no locale declares it.

## `public static string? QLocalizationTextFind(string key)`

The localized text under `key`, or null when no locale declares it.
The plain read answers the key itself on a miss, which a caller cannot tell from a translation.

## `internal static void QLocalizationCatalogApply(ResourceDictionary resources, IReadOnlyDictionary<string, string> texts)`

Publishes the texts of a loaded catalog into the resources and announces the change.
Each resource write walks the whole live tree, so one write per key froze launch and every settings toggle.
It returns untouched when every text already equals its resource, since an unchanged catalog needs no walk.
Otherwise it fills a fresh dictionary off the tree with the previous texts and the new ones.
Carrying the previous texts keeps a key the new catalog lacks on its old text.
Swapping that dictionary into the merged list costs exactly one walk.
It stays the last merged dictionary, so it beats the theme dictionaries merged before it.
No locale text lives in veneer XAML, so no own key of the resources shadows it.
The caller loads the catalog, through the engine or, before one exists, through the localization port.

## Inline notes

### `new PropertyChangedEventArgs("Item[]")`

The name WPF listens for when an indexer changes and no single index is named.
