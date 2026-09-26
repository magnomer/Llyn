# TTag.cs

## `public sealed class TTag`

Covers Tags as the entry editor writes them, through the cards of a saved entry.
A Tag line is saved on either card kind and read back through the entry's load.
It covers the catalog's own Tag create as well.

## `public void TagCreate_WordingNoCardCarries_ListsItInTheCatalog()`

The taxonomy panel's New makes a Tag no card carries yet, trimmed, listed at once and browsing to no entry.

## `public void TagCreate_WordingAlreadyStored_ReturnsTheStoredRow()`

A wording already stored answers the stored row rather than a second one.

## `public void TagSave_KnownId_LinksThatRowWhateverTheText()`

A chip naming a stored Tag links that row, so the stored wording wins over the chip's text.

## `public void TagSave_DroppedFromEveryCard_KeepsRowInCatalog()`

A Tag no card carries any more stays in the catalog and browses to no entry.

## `private static LEntry TTagEntryCreate(LEngine engine, IReadOnlyList<string> meaningTags, IReadOnlyList<string> collocationTags)`

One saved entry with a Meaning card and a Collocation card, each carrying the Tag line given.
