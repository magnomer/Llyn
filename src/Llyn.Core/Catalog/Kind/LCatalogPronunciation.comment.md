# LCatalogPronunciation.cs
Hash: `95f3f55ce3adfe3d`

## `public sealed record LCatalogPronunciation(LEntry LCatalogPronunciationEntry, string LCatalogPronunciationSound, string LCatalogPronunciationName = "", string LCatalogPronunciationEpithet = "", bool LCatalogPronunciationChosen = false)`

One entry as a phonology row: the stored entry and the primary pronunciation stored for it.
The sound travels with the row because two of the orderings read it and the row shows it.
An entry with nothing stored carries an empty sound.
Only the pending ordering lists such rows, and it lists nothing else.

**Parameters**

- `LCatalogPronunciationEntry` — The stored entry the row stands for.
- `LCatalogPronunciationSound` — The shown reading of the entry's first pronunciation, empty where none is stored.
  It is the respelling where the clerk was told to respell and one is stored, the IPA otherwise.
- `LCatalogPronunciationName` — The headword with its twin number, empty until the vista find fills it.
- `LCatalogPronunciationEpithet` — The entry's epithet, empty where none shows or until the vista find fills it.
- `LCatalogPronunciationChosen` — True on the row of the entry the vista stands on, false until the vista find fills it.

## `public string LCatalogPronunciationText`

The sound in square brackets, the way a phonetic transcription is written.
An empty sound answers an empty pair of brackets, so a pending row still shows a slot.

## `public static LCatalogPronunciation LCatalogPronunciationCreate(LEntry entry, string? sound)`

Builds the row from the entry and whatever pronunciation is stored against it.

## `public static IReadOnlyList<LCatalogPronunciation> LCatalogPronunciationSort(IReadOnlyList<LCatalogPronunciation> rows, LCatalogOrder order)`

Orders the rows under one ordering, and under the headword where the ordering is not one a row answers to.
The pending ordering goes by headword, since its rows all hold an empty sound.
A sound is compared ordinally, because an IPA string is a sequence of symbols and not a word.
Rows still equal go by the entry tie rule in `LCatalogEntry`, so storage order never decides a place.
