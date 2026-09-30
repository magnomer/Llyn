# CNotationReading.cs

## `public sealed record CNotationReading(`

One found reading on a source row, ready to paint and to take.
The driver looks up the label and the flag, and decides nothing about them.

**Parameters**

- `CNotationReadingPhonetic`: the phonetic text the source gave, which taking writes.
- `CNotationReadingText`: the text the row shows, the respelling when the mark shows one.
- `CNotationReadingVariety`: the variety's name, key and flag key under the search's language.
  Its name is the raw variety that taking writes, blank when the source names none.
- `CNotationReadingFlagged`: whether the row shows the variety's flag.
- `CNotationReadingMark`: the respelling switch and the brackets the text stands between.
