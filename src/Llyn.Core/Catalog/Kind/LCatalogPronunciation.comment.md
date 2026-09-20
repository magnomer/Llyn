# LCatalogPronunciation.cs

## `public sealed record LCatalogPronunciation(`

One entry as a phonology row: the stored entry and the primary pronunciation stored for it.
The sound travels with the row because two of the orderings read it and the row shows it.
An entry with nothing stored carries an empty sound, which is what the pending ordering looks for.

**Parameters**

- `LCatalogPronunciationEntry` — The stored entry the row stands for.
- `LCatalogPronunciationSound` — The IPA of the primary pronunciation, the first of the entry, empty where none is stored.
- `LCatalogPronunciationName` — The headword with its twin number, empty until the vista find fills it.
- `LCatalogPronunciationEpithet` — The entry's epithet, null where none shows or until the vista find fills it.
- `LCatalogPronunciationChosen` — True on the row of the entry the vista stands on, false until the vista find fills it.

## `public static LCatalogPronunciation LCatalogPronunciationCreate(LEntry entry, string? sound)`

Builds the row from the entry and whatever pronunciation is stored against it.

## `public static IReadOnlyList<LCatalogPronunciation> LCatalogPronunciationSort(IReadOnlyList<LCatalogPronunciation> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the headword where the ordering is not one a row answers to.
An entry with no pronunciation sorts last under the sound ordering.
It sorts first under the pending ordering, which is the one that looks for what is still missing.
A sound is compared ordinally, because an IPA string is a sequence of symbols and not a word.
