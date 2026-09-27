# CCatalogPronunciation.cs

## `public sealed record CCatalogPronunciation(CVistaRow CCatalogPronunciationEntry, string CCatalogPronunciationSound);`

One entry as the phonology inventory lists it, with the pronunciation it is looked up by.

**Parameters**

- `CCatalogPronunciationEntry`: the entry, with its displayed name and chosen mark.
- `CCatalogPronunciationSound`: the IPA of the primary pronunciation, empty where none is stored.
