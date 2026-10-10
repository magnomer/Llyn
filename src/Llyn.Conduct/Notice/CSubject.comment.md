# CSubject.cs
Hash: `25c25fef7483041c`

## `public enum CSubject`

The kind of stored data a notice speaks about, as a driver names it when it attaches an observer.
It mirrors the engine's subject member for member, and `CCatalog.LCatalogSubjectRead` maps it down.
The map goes by name, never by cast, so a reordered member cannot shift a meaning.
`CSubjectFold` speaks about one entry's saved card folds, reading list and box states.
So the editor and the display of that entry repaint after any of those writes.
`CSubjectStemFold` speaks about one series member's own fold, which only the series page shows.
