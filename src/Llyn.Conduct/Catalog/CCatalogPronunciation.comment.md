# CCatalogPronunciation.cs

## `public sealed record CCatalogPronunciation(`

One entry as the phonology inventory lists it, with the pronunciation it is looked up by.

**Parameters**

- `CCatalogPronunciationEntry`: the entry, with its displayed name and chosen mark.
- `CCatalogPronunciationSound`: the IPA of the primary pronunciation, empty where none is stored.
- `CCatalogPronunciationText`: the sound ready to show in brackets, an empty pair where none is stored.
