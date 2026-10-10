# TFold.cs
Hash: `027d6fdeed161480`

## `public sealed class TFold`

Covers the engine's card fold seam for Meaning and Collocation cards.
A card is folded, read back per entry, and unfolded through `LEngine` relays alone.
It covers both card kinds, since each kind keeps its marks in a table of its own.
It covers the per-entry read returning only that entry's folded cards, so two entries keep apart.
It covers an entry save dropping a card, whose mark must go with it while the kept card stays folded.
It covers an id at or below zero, as an unsaved card carries.
Such an id must write nothing and throw nothing.
It covers the fold bulletin, raised on the entry so other open views of it re-read their folds.
It covers the "More readings" mark of an entry's reflex list, opened, closed and read back.
Two entries keep that mark apart, and an entry delete drops its row.
Nonpositive entry ids write nothing.
Accepted opening and closing writes raise the fold bulletin on their entry.

## `internal static LEntryDraft TFoldEntryCreate(LEngine engine, string headword, out long entryId)`

Saves an entry with two Meanings and one Collocation, then loads it back.
The loaded draft carries the stored card ids the fold calls need.
`TFoldBox` saves its entries through it too, so both fold suites share one entry shape.

## `public void FoldSave_StoredMeaning_AnswersFolded()`

A saved meaning fold is visible through the engine fold read.

## `public void FoldSave_StoredCollocation_AnswersFolded()`

A saved collocation fold is visible through the engine fold read.

## `public void FoldDelete_FoldedCards_AnswersUnfolded()`

Deleting both card folds leaves neither card folded.

## `public void FoldRead_TwoEntriesFolded_ReturnsOnlyEachEntrysOwnFoldedCards()`

Each entry read returns only its own folded card ids.

## `public void EntryUpdate_DroppedFoldedMeaning_DeletesItsFoldAndKeepsTheOthers()`

Updating away one folded meaning removes its mark and preserves retained folds.

## `public void EntryDelete_FoldedEntry_DropsItsCardFolds()`

Entry deletion removes its card-fold records.

## `public void FoldSave_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long cardId)`

Nonpositive card ids create no meaning or collocation fold records.

## `public void FoldSave_StoredCard_RaisesFoldBulletinOnItsEntry()`

A stored card fold publishes a Fold bulletin with its entry id.

## `public void ReflexSpread_OpenedEntry_AnswersOpened()`

Opening an entry reflex list persists its opening.

## `public void ReflexSpread_ClosedAfterOpened_AnswersClosedAndKeepsNoRow()`

Closing a previously opened reflex list removes its opening record.

## `public void ReflexSpread_TwoEntries_KeepsEachEntrysOwnState()`

Two entries retain independent reflex openings.

## `public void EntryDelete_SpreadEntry_DropsItsReflexFold()`

Entry deletion removes its reflex opening record.

## `public void ReflexSpread_IdAtOrBelowZero_WritesNothingAndThrowsNothing(long entryId)`

Nonpositive entry ids create no reflex opening records.

## `public void ReflexSpread_StoredEntry_RaisesFoldBulletinOnItsEntry()`

A reflex opening publishes a Fold bulletin with its entry id.
