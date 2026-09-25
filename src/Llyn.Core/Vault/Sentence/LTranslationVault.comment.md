# LTranslationVault.cs

## `public interface LTranslationVault`

The persistence port for the Translation rows the engine reads and writes.
It lists exactly what the engine asks of translation storage, and nothing about how rows are kept.
`LTranslationArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LTranslation> LTranslationMeaningRead(long meaningId);`

Reads the links a Meaning makes, in the order that Meaning gives them.

## `IReadOnlyList<LTranslation> LTranslationCollocationRead(long collocationId);`

Reads the links a Collocation makes, in the order that Collocation gives them.

## `void LTranslationMeaningSave(long meaningId, IReadOnlyList<LTranslation> translations);`

Writes a Meaning's whole Translation line, replacing whatever it carried.
Ids are trimmed, blanks and repeats are dropped, and what survives is numbered from zero.

## `void LTranslationCollocationSave( long collocationId, IReadOnlyList<LTranslation> translations);`

The same write for a Collocation.

## `IReadOnlyList<LTranslationTarget> LTranslationTargetRead(IReadOnlyList<long> ids);`

Reads the headword and language of every Entry named by `ids`, in the order the ids were given.
A whole card's links resolve in one query rather than one query each.
An id no Entry answers is skipped rather than reported, because a caller wants what it can show.

## `IReadOnlyList<LUsage> LTranslationIncomingRead(long entryId);`

Reads the Meanings and Collocations that point at this Entry.
Each one is a Usage, so an Entry shows what renders it the way an Example shows what quotes it.
Meanings come before Collocations, each group ordered by headword and then by place on its Entry.
