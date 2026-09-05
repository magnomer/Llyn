# PLocalizationBinder.cs

## `public sealed class PLocalizationBinder`

One localized text, read as an ordinary binding rather than a resource reference.
A multi binding accepts only bindings, so text that meets a converter cannot arrive as a dynamic resource.
This carries the key into the catalog and follows the language for the life of the element.

## Inline notes

### `Source = PLocalizationCatalog.PLocalizationCatalogCurrent;`

Every instance reads the one catalog, so one announcement refreshes them all.
