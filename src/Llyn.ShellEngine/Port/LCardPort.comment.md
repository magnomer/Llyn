# LCardPort.cs
Hash: `1dbaa2554458f5f4`

## `public interface LCardPort`

The engine slice for a shown entry's card links and stored card folds.
`LCardFacade` implements it.

## `IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);`

Incoming links include both card kinds, with epithets chosen by settings.

## `IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);`

Top-level meanings and collocations each receive a dictionary key, even without resolved translation targets.
Missing targets are omitted rather than rendered as blank chips.

## `LEtymologyResult LEngineEtymologyRead(LEntryDraft draft);`

Resolved source links and the narrative verdict travel together, so the reading view needs one etymology answer.

## `IReadOnlySet<long> LEngineFoldRead(long entryId);`

One entry-scoped read covers folded Meanings and Collocations together.

## `void LEngineFoldSave(long entryId, long cardId);`

Card folds remain outside lexical edits and undo.
Normal completion announces a fold bulletin for `entryId`, even when a nonpositive card id writes nothing.
Views of that entry can then re-read their folds.

## `void LEngineFoldDelete(long entryId, long cardId);`

Unfolding leaves lexical data untouched and announces the same entry-scoped fold bulletin on normal completion.
