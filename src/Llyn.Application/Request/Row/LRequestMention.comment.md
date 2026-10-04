# LRequestMention.cs
Hash: `c201672700398580`

The field requests key `LRequestKey` by type and row id, so a deferred edit replaces only its own row.
The Mention requests, one per gesture a shown sentence offers on one of its words.
Every one names the card and the sentence by id, real or minted.
`LRequestCardId` 0 and `LRequestSentenceId` 0 together name the draft's own Example instead, the one the corpus panel holds.
So one set of three records serves the card editor and the corpus panel alike.

## `public sealed record LRequestMentionAddition(long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, int LRequestOffset, int LRequestLength, long LRequestEntryId, long LRequestSenseId)`

Links the word at `LRequestOffset` of `LRequestLength` code points to the Entry named.
`LRequestEntryId` 0 marks the word as standing for nothing, and `LRequestSenseId` 0 chooses no Meaning.
A span overlapping an existing Mention replaces it.
So the form has one gesture and the engine decides whether it is new or a change.
A span past the end of the text is refused.
So is an Entry no row carries, or a Meaning of another Entry.

## `public sealed record LRequestMentionRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestMentionId)`

Drops one Mention by id.

## `public sealed record LRequestMentionSense(long LRequestDraftId, long LRequestCardId, long LRequestSentenceId, long LRequestMentionId, long LRequestSenseId)`

Narrows one Mention to the Meaning named, or clears the choice with `LRequestSenseId` 0.
A Mention standing for nothing has no Entry to narrow, so the request is refused on it.
