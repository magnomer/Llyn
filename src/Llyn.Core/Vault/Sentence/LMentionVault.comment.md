# LMentionVault.cs

## `public interface LMentionVault`

The persistence port for the Mention rows the engine reads and writes.
It lists exactly what the engine asks of mention storage, and nothing about how rows are kept.
`LMentionArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LMention> LMentionExampleRead(long exampleId);`

Every Mention of the Example identified by `exampleId`, ordered by where each starts.

## `IReadOnlyList<long> LMentionExampleSave(long exampleId, IReadOnlyList<LMention> mentions);`

Rewrites the Mention list of one Example and answers the written ids in input order.
A span past the end of the stored text is refused, and so is a pair of spans that overlap.
The refusal names the offending span and throws before any row is touched.
Existing rows are shelved by shifting their start far up, so the unique (example, start) index never trips.
A row whose positive id survives is rewritten in place, the rest are inserted, and the shelf is deleted.
This is the same shape as `LSentenceOwnerSave`.

## `IReadOnlyList<long> LMentionSenseClear(long meaningId);`

Drops the sense from every Mention citing `meaningId`, so each lands on its entry alone.
The answer names each affected Example once, so a caller can report what lost its sense.
The markup Replace path uses it before the target's senses are rewritten.

## `bool LMentionSenseSet(long mentionId, long senseId);`

Points the Mention `mentionId` at the sense `senseId`, answering whether a row changed.
A Mention with no entry is left alone, since a sense without an entry breaks the row's check.
The markup import uses it once every entry of a file is stored and the cited senses exist.
