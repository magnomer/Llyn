# LMentionPort.cs
Hash: `c4b6fa24e46c66b9`

## `public interface LMentionPort`

The slice of the engine a deportment sees when it opens a Mention or a link.
`LMentionFacade` implements it.

## `LMentionResult LEngineMentionFind(long exampleId, int offset);`

The Mention under the offset of a stored Example's text.

## `LMentionResult LEngineMentionFind(LEntryDraft shown, long sentence, int offset);`

The Mention under the offset of one sentence of the shown entry.

## `LMentionResult LEngineEtymologyFind(LEntryDraft shown, int offset);`

The Mention under the offset of the shown entry's etymology.

## `long? LEngineLinkRead(long? link);`

The entry a clicked link names, or null when it names none.
A link names its entry only through a stored id, so an empty id names nothing.
