# TCorpusMention.cs
Hash: `ae284a008fb2ec07`

## `public sealed class TCorpusMention`

Covers the corpus transcript's mention gates and reads over a held Example on a real workspace.
A linked Mention offers its Entry's Meanings, and the sense gate narrows it to one.
A silent Mention may be unlinked but offers no sense.
A Mention drops by the selection over it and by its chip's id.
The chip line names a linked headword and keys a silent chip.
A corpus holding no transcript answers no chips and no Meanings.
A failed chip line or Meaning read reports `Mention.FindFailed` once and answers nothing.
A failed Meaning read hands the envoy the `Notice.Unexpected` ledger notice.
A link over an Entry keeps the selection's span and carries no sense.
The Meaning read leads with the whole Entry, maps each row, and asks the engine with the unknown key.
A transcript word is searched in the Example's language, and the pick links it.
An excerpt click with no chosen Example finds nothing, and a kept leave asks once and finds nothing.
A failed word find reports `Mention.FindFailed` and offers nothing.
One found Entry opens at once and leaves an empty menu, and several stay offered under the found word.

## `private static CCorpus TMentionPrepare(LEngine engine, CAtelier atelier, CEnvoy envoy)`

Stores an Example holding the sentence, opens it and switches the corpus to its transcript.

## `private static IReadOnlyList<LMention> TMentionRead(CCorpus corpus)`

The Mentions the held transcript's draft carries.

## `private static List<long> TMentionLibraryAdd(CAtelier atelier)`

Registers a Library tab whose arrivals the test reads, so an Entry open is observable.
