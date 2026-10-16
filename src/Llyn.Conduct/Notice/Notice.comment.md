# Notice

This folder holds what a driver's observer receives about stored data.
It stands apart from the engine's notices, because values are copied so the rules stay in the engine.

## `CBulletin`

One notice as a driver's observer receives it, copied from the engine's notice by the controller.
It carries no subject, because the observer attached for one subject already knows it.
Its stored flag is copied from the engine, so the rule stays there.
Its refusal is the engine's, null unless a queued draft request was refused, and the desk shows it.

## `CSubject`

The kind of stored data a notice speaks about, named by a driver when it attaches an observer.
It mirrors the engine's subject member for member.
`CCatalog.LCatalogSubjectRead` maps it down by name and never by cast, so a reordered member cannot shift a meaning.
`CSubjectFold` speaks about one entry's saved card folds, reading list and box states.
So the editor and the display of that entry repaint after any of those writes.
`CSubjectStemFold` speaks about one series member's own fold, which only the series page shows.
`CSubjectDeck` speaks about any deck, folder or cue write, which the study and statistics panels follow.

## Stamp

Commit: `121691f60459`
- `CBulletin.cs` `6ccb743a020f94a6`
- `CSubject.cs` `07270ad74e5d029b`
