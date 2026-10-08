# TTranscriptMention.cs
Hash: `2fef1d74f0e5c94c`

## `public sealed class TTranscriptMention`

Covers `CTranscript`'s Mention gates and reads, reached through `CCorpusTranscript`, over a held Example on a real workspace.
A linked Mention offers its Entry's Meanings, and the sense gate narrows it to one.
A silent Mention may be unlinked but offers no sense.
A Mention drops by the selection over it and by its chip's id.
The chip line names a linked headword and keys a silent chip.
A corpus holding no Example answers no chips and no Meanings.
A selected word is searched in the Example's language, and the pick links it.
A link over an Entry keeps the selection's span and carries no sense.

## `internal static CCorpus TMentionPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Stores an Example holding the sentence, opens it and switches the corpus to its transcript.
`TCorpusMention` calls it for its shared Mention reads.

## `private static IReadOnlyList<LMention> TMentionRead(CCorpus corpus)`

The Mentions the held transcript's draft carries.
