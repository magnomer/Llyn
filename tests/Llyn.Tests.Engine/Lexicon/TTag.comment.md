# TTag.cs
Hash: `0200895230f0ca8f`

## `public sealed class TTag`

Covers Tags as the entry editor writes them, through the cards of a saved entry.
A Tag line is saved on either card kind and read back through the entry's load.
It covers the catalog's own Tag create as well.

## `public void TagSave_SameTextOnBothCardKinds_ReadsBackFromEach()`

A Meaning and a Collocation each read back the Tag line they were saved with.
The catalog lists each wording once, however many cards carry it.

## `public void TagSave_SpacingAndPunctuation_ReadsBackAsWritten()`

A Tag keeps its inner spacing and wording, loses its edge spaces, and a blank item is dropped.
The stored wording is what the catalog shows, so the save must not alter it further.

## `public void TagSave_SameTextTwiceOnOneCard_KeepsOneTag()`

A wording repeated on one card is one Tag, and the positions left are contiguous.
A repeat would otherwise show twice on the card and take a gap in the order.

## `public void TagCreate_WordingNoCardCarries_ListsItInTheCatalog()`

The taxonomy panel's New makes a Tag no card carries yet, trimmed, listed at once and browsing to no entry.

## `public void TagCreate_WordingAlreadyStored_ReturnsTheStoredRow()`

A wording already stored answers the stored row rather than a second one.

## `public void TagSave_TwoCardsSameText_LinkOneRow()`

Two cards carrying the same wording link one Tag row, not one each.
The catalog count of a Tag would otherwise be split across duplicates.

## `public void TagSave_KnownId_LinksThatRowWhateverTheText()`

A chip naming a stored Tag links that row, so the stored wording wins over the chip's text.

## `public void TagSave_DroppedFromEveryCard_KeepsRowInCatalog()`

A Tag no card carries any more stays in the catalog and browses to no entry.

## `private static LEntry TTagEntryCreate(LEngine engine, IReadOnlyList<string> meaningTags, IReadOnlyList<string> collocationTags)`

One saved entry with a Meaning card and a Collocation card, each carrying the Tag line given.
